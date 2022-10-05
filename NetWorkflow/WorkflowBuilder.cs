using System.Linq.Expressions;

namespace NetWorkflow
{
    public class WorkflowBuilder : IWorkflowBuilder
    {
        protected WorkflowBuilder? _next;

        public IWorkflowBuilderNext<TOut> StartWith<TOut>(Expression<Func<IWorkflowStep<TOut>>> func)
        {
            _next = new WorkflowBuilder<TOut>(new WorkflowExecutor<TOut>(func));

            return (IWorkflowBuilderNext<TOut>)_next;
        }

        public virtual object? Run(object? args, CancellationToken token = default) => _next?.Run(args, token);
    }

    public class WorkflowBuilder<TOut> : WorkflowBuilder, IWorkflowBuilderNext<TOut>
    {
        internal readonly IWorkflowExecutor _executor;
        
        public WorkflowBuilder(IWorkflowExecutor executor)
        {
            _executor = executor;
        }

        public IWorkflowBuilderNext<TNext[]> Parallel<TNext>(Expression<Func<IEnumerable<IWorkflowStepAsync<TOut, TNext>>>> func)
        {
            _next = new WorkflowBuilder<TOut, TNext[]>(new WorkflowExecutor<TNext>(func));

            return (IWorkflowBuilderNext<TNext[]>)_next;
        }

        public IWorkflowBuilderNext<TOut, TNext> Then<TNext>(Expression<Func<IWorkflowStep<TOut, TNext>>> func)
        {
            _next = new WorkflowBuilder<TOut, TNext>(new WorkflowExecutor<TNext>(func));

            return (IWorkflowBuilderNext<TOut, TNext>)_next;
        }

        public override object? Run(object? args, CancellationToken token = default)
        {
            object? result = _executor.Run(args, token);

            if (_executor.Stopped || _next == null) return result;

            return _next?.Run(result, token);
        }

        public IWorkflowBuilderConditional<TOut> If(Expression<Func<TOut, bool>> func)
        {
            _next = new WorkflowBuilderConditional<TOut>(new WorkflowExecutorConditional<TOut>(func));

            return (IWorkflowBuilderConditional<TOut>)_next;
        }
    }

    public class WorkflowBuilder<TIn, TOut> : WorkflowBuilder<TOut>, IWorkflowBuilderNext<TIn, TOut>
    {
        public WorkflowBuilder(IWorkflowExecutor executor) : base(executor) { }
    }

    public class WorkflowBuilderConditional<TIn> : 
        WorkflowBuilder, IWorkflowBuilderConditional<TIn>, IWorkflowBuilderConditionalNext<TIn>,
        IWorkflowBuilderConditionalFinal<TIn>, IWorkflowBuilderConditionalFinalAggregate
    {
        private readonly WorkflowExecutorConditional<TIn> _executor;

        public WorkflowBuilderConditional(WorkflowExecutorConditional<TIn> executor)
        {
            _executor = executor;
        }

        public IWorkflowBuilderConditionalNext<TIn> Do<TNext>(Expression<Func<IWorkflowStep<TIn, TNext>>> func)
        {
            _executor.Append(new WorkflowExecutor<TNext>(func));

            return this;
        }

        public IWorkflowBuilderConditional<TIn> ElseIf(Expression<Func<TIn, bool>> func)
        {
            _executor.Append(func);

            return this;
        }

        public IWorkflowBuilderConditionalFinal<TIn> Else()
        {
            _executor.Append(args => true);

            return this;
        }

        public IWorkflowBuilderNext<object> EndIf()
        {
            _next = new WorkflowBuilder<object>(new WorkflowExecutorNext());

            return (IWorkflowBuilderNext<object>)_next;
        }

        public IWorkflowBuilderConditionalNext<TIn> Stop()
        {
            _executor.SetStop();

            return this;
        }

        public IWorkflowBuilderConditionalNext<TIn> Throw(Expression<Func<Exception>> func)
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

        IWorkflowBuilderConditionalFinalAggregate IWorkflowBuilderConditionalFinal<TIn>.Do<TNext>(Expression<Func<IWorkflowStep<TIn, TNext>>> func)
        {
            _executor.Append(new WorkflowExecutor<TNext>(func));

            return this;
        }

        IWorkflowBuilderConditionalFinalAggregate IWorkflowBuilderConditionalFinal<TIn>.Stop()
        {
            _executor.SetStop();

            return this;
        }

        IWorkflowBuilderConditionalFinalAggregate IWorkflowBuilderConditionalFinal<TIn>.Throw(Expression<Func<Exception>> func)
        {
            _executor.SetExceptionToThrow(func);

            return this;
        }
    }
}

