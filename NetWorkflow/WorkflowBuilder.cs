using System.Linq.Expressions;

namespace NetWorkflow
{
    public class WorkflowBuilder<TContext> : IWorkflowBuilder<TContext>
    {
        protected readonly TContext _context;

        protected WorkflowBuilder<TContext>? _next;

        public WorkflowBuilder(TContext context)
        {
            _context = context;
        }

        public IWorkflowBuilderNext<TContext, TOut> StartWith<TOut>(Expression<Func<IWorkflowStep<TOut>>> func)
        {
            _next = new WorkflowBuilder<TContext, TOut>(new WorkflowExecutor<TContext, TOut>(func, _context), _context);

            return (IWorkflowBuilderNext<TContext, TOut>)_next;
        }

        public IWorkflowBuilderNext<TContext, TOut> StartWith<TOut>(Expression<Func<TContext, IWorkflowStep<TOut>>> func)
        {
            _next = new WorkflowBuilder<TContext, TOut>(new WorkflowExecutor<TContext, TOut>(func, _context), _context);

            return (IWorkflowBuilderNext<TContext, TOut>)_next;
        }

        public virtual object? Run(object? args, CancellationToken token = default) => _next?.Run(args, token);
    }

    public class WorkflowBuilder<TContext, TOut> : WorkflowBuilder<TContext>, 
        IWorkflowBuilderNext<TContext, TOut>
    {
        internal readonly IWorkflowExecutor _executor;
        
        public WorkflowBuilder(IWorkflowExecutor executor, TContext context) : base(context)
        {
            _executor = executor;
        }

        public IWorkflowBuilderNext<TContext, TNext[]> Parallel<TNext>(Expression<Func<IEnumerable<IWorkflowStepAsync<TOut, TNext>>>> func)
        {
            _next = new WorkflowBuilder<TContext, TOut, TNext[]>(new WorkflowExecutor<TContext, TNext>(func, _context), _context);

            return (IWorkflowBuilderNext<TContext, TNext[]>)_next;
        }

        public IWorkflowBuilderNext<TContext, TNext[]> Parallel<TNext>(Expression<Func<TContext, IEnumerable<IWorkflowStepAsync<TOut, TNext>>>> func)
        {
            _next = new WorkflowBuilder<TContext, TOut, TNext[]>(new WorkflowExecutor<TContext, TNext>(func, _context), _context);

            return (IWorkflowBuilderNext<TContext, TNext[]>)_next;
        }

        public IWorkflowBuilderNext<TContext, TOut, TNext> Then<TNext>(Expression<Func<IWorkflowStep<TOut, TNext>>> func)
        {
            _next = new WorkflowBuilder<TContext, TOut, TNext>(new WorkflowExecutor<TContext, TNext>(func, _context), _context);

            return (IWorkflowBuilderNext<TContext, TOut, TNext>)_next;
        }

        public IWorkflowBuilderNext<TContext, TOut, TNext> Then<TNext>(Expression<Func<TContext, IWorkflowStep<TOut, TNext>>> func)
        {
            _next = new WorkflowBuilder<TContext, TOut, TNext>(new WorkflowExecutor<TContext, TNext>(func, _context), _context);

            return (IWorkflowBuilderNext<TContext, TOut, TNext>)_next;
        }

        public override object? Run(object? args, CancellationToken token = default)
        {
            object? result = _executor.Run(args, token);

            if (_executor.Stopped || _next == null) return result;

            return _next?.Run(result, token);
        }

        public IWorkflowBuilderConditional<TContext, TOut> If(Expression<Func<TOut, bool>> func)
        {
            _next = new WorkflowBuilderConditional<TContext, TOut>(new WorkflowExecutorConditional<TOut>(func), _context);

            return (IWorkflowBuilderConditional<TContext, TOut>)_next;
        }
    }

    public class WorkflowBuilder<TContext, TIn, TOut> : WorkflowBuilder<TContext, TOut>, IWorkflowBuilderNext<TContext, TIn, TOut>
    {
        public WorkflowBuilder(IWorkflowExecutor executor, TContext context) : base(executor, context) { }
    }

    public class WorkflowBuilderConditional<TContext, TIn> : 
        WorkflowBuilder<TContext>, IWorkflowBuilderConditional<TContext, TIn>, IWorkflowBuilderConditionalNext<TContext, TIn>,
        IWorkflowBuilderConditionalFinal<TContext, TIn>, IWorkflowBuilderConditionalFinalAggregate<TContext>
    {
        private readonly WorkflowExecutorConditional<TIn> _executor;

        public WorkflowBuilderConditional(WorkflowExecutorConditional<TIn> executor, TContext context) : base(context)
        {
            _executor = executor;
        }

        public IWorkflowBuilderConditionalNext<TContext, TIn> Do<TNext>(Expression<Func<IWorkflowStep<TIn, TNext>>> func)
        {
            _executor.Append(new WorkflowExecutor<TContext, TNext>(func, _context));

            return this;
        }

        public IWorkflowBuilderConditionalNext<TContext, TIn> Do<TNext>(Expression<Func<TContext, IWorkflowStep<TIn, TNext>>> func)
        {
            _executor.Append(new WorkflowExecutor<TContext, TNext>(func, _context));

            return this;
        }

        public IWorkflowBuilderConditional<TContext, TIn> ElseIf(Expression<Func<TIn, bool>> func)
        {
            _executor.Append(func);

            return this;
        }

        public IWorkflowBuilderConditionalFinal<TContext, TIn> Else()
        {
            _executor.Append(args => true);

            return this;
        }

        public IWorkflowBuilderNext<TContext, object> EndIf()
        {
            _next = new WorkflowBuilder<TContext, object>(new WorkflowExecutorNext(), _context);

            return (IWorkflowBuilderNext<TContext, object>)_next;
        }

        public IWorkflowBuilderConditionalNext<TContext, TIn> Stop()
        {
            _executor.SetStop();

            return this;
        }

        public IWorkflowBuilderConditionalNext<TContext, TIn> Throw(Expression<Func<Exception>> func)
        {
            _executor.SetExceptionToThrow(func);

            return this;
        }

        public override object? Run(object? args, CancellationToken token = default)
        {
            object? result = _executor.Run(args, token);

            if (_executor.Stopped || _next == null) return result;

            return _next?.Run(result, token);
        }

        IWorkflowBuilderConditionalFinalAggregate<TContext> IWorkflowBuilderConditionalFinal<TContext, TIn>.Do<TNext>(Expression<Func<IWorkflowStep<TIn, TNext>>> func)
        {
            _executor.Append(new WorkflowExecutor<TContext, TNext>(func, _context));

            return this;
        }

        IWorkflowBuilderConditionalFinalAggregate<TContext> IWorkflowBuilderConditionalFinal<TContext, TIn>.Do<TNext>(Expression<Func<TContext, IWorkflowStep<TIn, TNext>>> func)
        {
            _executor.Append(new WorkflowExecutor<TContext, TNext>(func, _context));

            return this;
        }

        IWorkflowBuilderConditionalFinalAggregate<TContext> IWorkflowBuilderConditionalFinal<TContext, TIn>.Stop()
        {
            _executor.SetStop();

            return this;
        }

        IWorkflowBuilderConditionalFinalAggregate<TContext> IWorkflowBuilderConditionalFinal<TContext, TIn>.Throw(Expression<Func<Exception>> func)
        {
            _executor.SetExceptionToThrow(func);

            return this;
        }
    }
}

