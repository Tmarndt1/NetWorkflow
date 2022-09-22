
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

        public IWorkflowBuilderNext<TContext, Tout> StartWith<Tout>(Expression<Func<WorkflowStep<Tout>>> func)
        {
            _next = new WorkflowBuilder<TContext, Tout>(new WorkflowExecutor<TContext, Tout>(func, _context), _context);

            return (IWorkflowBuilderNext<TContext, Tout>)_next;
        }

        public IWorkflowBuilderNext<TContext, Tout> StartWith<Tout>(Expression<Func<TContext, WorkflowStep<Tout>>> func)
        {
            _next = new WorkflowBuilder<TContext, Tout>(new WorkflowExecutor<TContext, Tout>(func, _context), _context);

            return (IWorkflowBuilderNext<TContext, Tout>)_next;
        }

        public virtual object? Run(object? args, CancellationToken token = default) => _next?.Run(token);
    }

    public class WorkflowBuilder<TContext, Tout> : WorkflowBuilder<TContext>, 
        IWorkflowBuilderNext<TContext, Tout>
    {
        internal readonly WorkflowExecutor<TContext> _executor;
        
        public WorkflowBuilder(WorkflowExecutor<TContext> executor, TContext context) : base(context)
        {
            _executor = executor;
        }

        public IWorkflowBuilderNext<TContext, TNext[]> Parallel<TNext>(Expression<Func<IEnumerable<WorkflowStepAsync<Tout, TNext>>>> func)
        {
            _next = new WorkflowBuilder<TContext, Tout, TNext[]>(new WorkflowExecutor<TContext, TNext>(func, _context), _context);

            return (IWorkflowBuilderNext<TContext, TNext[]>)_next;
        }

        public IWorkflowBuilderNext<TContext, TNext[]> Parallel<TNext>(Expression<Func<TContext, IEnumerable<WorkflowStepAsync<Tout, TNext>>>> func)
        {
            _next = new WorkflowBuilder<TContext, Tout, TNext[]>(new WorkflowExecutor<TContext, TNext>(func, _context), _context);

            return (IWorkflowBuilderNext<TContext, TNext[]>)_next;
        }

        public IWorkflowBuilderNext<TContext, Tout, TNext> Then<TNext>(Expression<Func<WorkflowStep<Tout, TNext>>> func)
        {
            _next = new WorkflowBuilder<TContext, Tout, TNext>(new WorkflowExecutor<TContext, TNext>(func, _context), _context);

            return (IWorkflowBuilderNext<TContext, Tout, TNext>)_next;
        }

        public IWorkflowBuilderNext<TContext, Tout, TNext> Then<TNext>(Expression<Func<TContext, WorkflowStep<Tout, TNext>>> func)
        {
            _next = new WorkflowBuilder<TContext, Tout, TNext>(new WorkflowExecutor<TContext, TNext>(func, _context), _context);

            return (IWorkflowBuilderNext<TContext, Tout, TNext>)_next;
        }

        public override object? Run(object? args, CancellationToken token = default)
        {
            object? results = _executor.Run(args, token);

            if (_next == null) return results;

            return _next?.Run(results);
        }

        public IWorkflowBuilderConditional<TContext, Tout> If(Expression<Func<Tout, bool>> func)
        {
            _next = new WorkflowBuilderConditional<TContext, Tout>(new WorkflowExecutorConditional<TContext, Tout>(func, _context), _context);

            return (IWorkflowBuilderConditional<TContext, Tout>)_next;
        }
    }

    public class WorkflowBuilder<TContext, Tin, Tout> : WorkflowBuilder<TContext, Tout>, IWorkflowBuilderNext<TContext, Tin, Tout>
    {
        public WorkflowBuilder(WorkflowExecutor<TContext> executor, TContext context) : base(executor, context) { }
    }

    public class WorkflowBuilderConditional<TContext, Tin> : WorkflowBuilder<TContext>, IWorkflowBuilderConditional<TContext, Tin>, IWorkflowBuilderConditionalNext<TContext, Tin>
    {
        private readonly WorkflowExecutorConditional<TContext, Tin> _executor;

        public WorkflowBuilderConditional(WorkflowExecutorConditional<TContext, Tin> executor, TContext context) : base(context)
        {
            _executor = executor;
        }

        public IWorkflowBuilderConditionalNext<TContext, Tin> Do<TNext>(Expression<Func<WorkflowStep<Tin, TNext>>> func)
        {
            _executor.Append(new WorkflowExecutor<TContext, TNext>(func, _context));

            return this;
        }

        public IWorkflowBuilderConditionalNext<TContext, Tin> Do<TNext>(Expression<Func<TContext, WorkflowStep<Tin, TNext>>> func)
        {
            _executor.Append(new WorkflowExecutor<TContext, TNext>(func, _context));

            return this;
        }

        public IWorkflowBuilderConditional<TContext, Tin> ElseIf(Expression<Func<Tin, bool>> func)
        {
            _executor.Append(func);

            return this;
        }

        public IWorkflowBuilderConditional<TContext, Tin> Else()
        {
            _executor.Append(args => true);

            return this;
        }

        public IWorkflowBuilderNext<TContext, object> EndIf()
        {
            _next = new WorkflowBuilder<TContext, object>(new WorkflowExecutorEmpty<TContext>(_context), _context);

            return (IWorkflowBuilderNext<TContext, object>)_next;
        }

        public override object? Run(object? args, CancellationToken token = default)
        {
            object? results = _executor.Run(args, token);

            if (_next == null) return results;

            return _next?.Run(results);
        }
    }
}

