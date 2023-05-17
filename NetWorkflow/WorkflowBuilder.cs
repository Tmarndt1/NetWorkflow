using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;

namespace NetWorkflow
{
    internal class WorkflowBuilder : IWorkflowBuilder, IDisposable
    {
        protected WorkflowBuilder _next;

        private bool _disposedValue;

        public object Result { get; protected set; }

        public IWorkflowBuilderNext<TOut> StartWith<TOut>(Expression<Func<IWorkflowStep<TOut>>> func)
        {
            _next = new WorkflowBuilder<object, TOut>(new WorkflowStepExecutor<object, TOut>(func));

            return (IWorkflowBuilderNext<TOut>)_next;
        }

        public virtual object Run(object args, CancellationToken token = default) => _next?.Run(args, token);

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _next?.Dispose();

                    Result = null;
                }

                _disposedValue = true;
            }
        }

        ~WorkflowBuilder()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public virtual void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }
    }

    internal class WorkflowBuilder<TIn, TOut> : WorkflowBuilder, IWorkflowBuilderNext<TIn, TOut>, IDisposable
    {
        internal readonly IWorkflowExecutor<TIn, TOut> _executor;

        private bool _disposedValue;

        internal WorkflowBuilder(IWorkflowExecutor<TIn, TOut> executor)
        {
            _executor = executor;
        }

        public IWorkflowBuilderNext<IEnumerable<TNext>> Parallel<TNext>(Expression<Func<IEnumerable<IWorkflowStepAsync<TOut, TNext>>>> func)
        {
            _next = new WorkflowBuilder<TOut, IEnumerable<TNext>>(new WorkflowParallelExecutor<TOut, TNext>(func));

            return (IWorkflowBuilderNext<IEnumerable<TNext>>)_next;
        }

        public IWorkflowBuilderNext<TOut, TNext> Then<TNext>(Expression<Func<IWorkflowStep<TOut, TNext>>> func)
        {
            _next = new WorkflowBuilder<TOut, TNext>(new WorkflowStepExecutor<TOut, TNext>(func));

            return (IWorkflowBuilderNext<TOut, TNext>)_next;
        }

        public IWorkflowBuilderNext<TOut, TNext> ThenAsync<TNext>(Expression<Func<IWorkflowStepAsync<TOut, TNext>>> func)
        {
            _next = new WorkflowBuilder<TOut, TNext>(new WorkflowStepAsyncExecutor<TOut, TNext>(func));

            return (IWorkflowBuilderNext<TOut, TNext>)_next;
        }

        public IWorkflowBuilderConditional<TOut> If(Expression<Func<TOut, bool>> func)
        {
            _next = new WorkflowBuilderConditional<TOut>(new WorkflowExecutorConditional<TOut>(func), this);

            return (IWorkflowBuilderConditional<TOut>)_next;
        }

        public override object Run(object args, CancellationToken token = default)
        {
            Result = _executor.Run((TIn)args, token);

            if (_next == null) return Result;

            return _next?.Run(Result, token);
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _executor?.Dispose();
                }

                _disposedValue = true;

                base.Dispose(disposing);
            }
        }

        ~WorkflowBuilder()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public override void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }
    }

    internal class WorkflowBuilderConditional<TIn> : WorkflowBuilder, 
        IWorkflowBuilderConditional<TIn>, 
        IWorkflowBuilderConditionalNext<TIn>, 
        IWorkflowBuilderConditionalFinal<TIn>, 
        IWorkflowBuilderConditionalFinalAggregate
    {
        private readonly WorkflowExecutorConditional<TIn> _executor;

        private readonly WorkflowBuilder _lastBuilder;

        private CancellationToken _token;

        private bool _disposedValue;

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
            _next = new WorkflowBuilder<object, object>(new WorkflowSkipExecutor<object>());

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

        public IWorkflowBuilderConditionalNext<TIn> Retry(TimeSpan delay, int maxRetries = 1)
        {
            _executor.SetRetry(delay, maxRetries, () => _lastBuilder.Run(_lastBuilder.Result, _token));

            return this;
        }

        public override object Run(object args, CancellationToken token = default)
        {
            _token = token;

            Result = _executor.Run((TIn)args, token);

            if (_next == null) return Result;

            return _next?.Run(Result, token);
        }

        IWorkflowBuilderConditionalFinalAggregate IWorkflowBuilderConditionalFinal<TIn>.Do<TNext>(Expression<Func<IWorkflowStep<TIn, TNext>>> func)
        {
            Do(func);

            return this;
        }

        IWorkflowBuilderConditionalFinalAggregate IWorkflowBuilderConditionalFinal<TIn>.Stop()
        {
            Stop();

            return this;
        }

        IWorkflowBuilderConditionalFinalAggregate IWorkflowBuilderConditionalFinal<TIn>.Throw(Expression<Func<Exception>> func)
        {
            Throw(func);

            return this;
        }

        IWorkflowBuilderConditionalFinalAggregate IWorkflowBuilderConditionalFinal<TIn>.Retry(TimeSpan delay, int maxRetries)
        {
            Retry(delay, maxRetries);

            return this;
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _executor?.Dispose();
                }

                _disposedValue = true;

                base.Dispose(disposing);
            }
        }

        ~WorkflowBuilderConditional()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public override void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }
    }
}

