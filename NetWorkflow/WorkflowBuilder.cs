using System.Linq.Expressions;

namespace NetWorkflow
{
    public class WorkflowBuilder : IWorkflowBuilder
    {
        protected WorkflowBuilder? _nextBuilder;

        public object? Result { get; protected set; }

        public IWorkflowBuilderNext<TOut> StartWith<TOut>(Expression<Func<IWorkflowStep<TOut>>> func)
        {
            _nextBuilder = new WorkflowBuilder<object, TOut>(new WorkflowStepExecutor<object, TOut>(func));

            return (IWorkflowBuilderNext<TOut>)_nextBuilder;
        }

        public virtual object? Run(object? args, CancellationToken token = default) => _nextBuilder?.Run(args, token);
    }

    public class WorkflowBuilder<TIn, TOut> : WorkflowBuilder, IWorkflowBuilderNext<TOut>, IWorkflowBuilderNext<TIn, TOut>
    {
        internal readonly IWorkflowExecutor<TIn, TOut> _executor;
        
        public WorkflowBuilder(IWorkflowExecutor<TIn, TOut> executor)
        {
            _executor = executor;
        }

        public IWorkflowBuilderNext<TNext[]> Parallel<TNext>(Expression<Func<IEnumerable<IWorkflowStepAsync<TOut, TNext>>>> func)
        {
            _nextBuilder = new WorkflowBuilder<TOut, TNext[]>(new WorkflowParallelExecutor<TOut, TNext>(func));

            return (IWorkflowBuilderNext<TNext[]>)_nextBuilder;
        }

        public IWorkflowBuilderNext<TOut, TNext> Then<TNext>(Expression<Func<IWorkflowStep<TOut, TNext>>> func)
        {
            _nextBuilder = new WorkflowBuilder<TOut, TNext>(new WorkflowStepExecutor<TOut, TNext>(func));

            return (IWorkflowBuilderNext<TOut, TNext>)_nextBuilder;
        }

        public override object? Run(object? args, CancellationToken token = default)
        {
            Result = _executor.Run((TIn)args, token);

            if (_nextBuilder == null) return Result;

            return _nextBuilder?.Run(Result, token);
        }

        public IWorkflowBuilderConditional<TOut> If(Expression<Func<TOut, bool>> func)
        {
            _nextBuilder = new WorkflowBuilderConditional<TOut>(new WorkflowExecutorConditional<TOut>(func), this);

            return (IWorkflowBuilderConditional<TOut>)_nextBuilder;
        }
    }

    public class WorkflowBuilderConditional<TIn> : WorkflowBuilder, 
        IWorkflowBuilderConditional<TIn>, 
        IWorkflowBuilderConditionalNext<TIn>, 
        IWorkflowBuilderConditionalFinal<TIn>, 
        IWorkflowBuilderConditionalFinalAggregate
    {
        private readonly WorkflowExecutorConditional<TIn> _executor;

        private readonly WorkflowBuilder _lastBuilder;

        private CancellationToken _token;

        public WorkflowBuilderConditional(WorkflowExecutorConditional<TIn> executor, WorkflowBuilder lastBuilder)
        {
            _executor = executor;

            _lastBuilder = lastBuilder;
        }

        public IWorkflowBuilderConditionalNext<TIn> Do<TNext>(Expression<Func<IWorkflowStep<TIn, TNext>>> func)
        {
            _executor.Append(new WorkflowStepExecutor<TIn, object>(func));

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
            _nextBuilder = new WorkflowBuilder<object, object>(new WorkflowPassiveExecutor<object>());

            return (IWorkflowBuilderNext<object>)_nextBuilder;
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

        public IWorkflowBuilderConditionalNext<TIn> Retry(int maxRetries)
        {
            _executor.SetRetry(maxRetries, () => _lastBuilder.Run(_lastBuilder.Result, _token));

            return this;
        }

        public override object? Run(object? args, CancellationToken token = default)
        {
            _token = token;

            Result = _executor.Run((TIn)args, token);

            if (_nextBuilder == null) return Result;

            return _nextBuilder?.Run(Result, token);
        }

        IWorkflowBuilderConditionalFinalAggregate IWorkflowBuilderConditionalFinal<TIn>.Do<TNext>(Expression<Func<IWorkflowStep<TIn, TNext>>> func)
        {
            _executor.Append(new WorkflowStepExecutor<TIn, object>(func));

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

        IWorkflowBuilderConditionalFinalAggregate IWorkflowBuilderConditionalFinal<TIn>.Retry(int maxRetries)
        {
            _executor.SetRetry(maxRetries, () => _lastBuilder.Run(_lastBuilder.Result, _token));

            return this;
        }
    }
}

