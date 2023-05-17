using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace NetWorkflow
{
    internal class WorkflowExecutorConditional<TIn> : IWorkflowExecutor<TIn, object>
    {
        private List<ExecutorWrapper> _wrappers = new List<ExecutorWrapper>();

        private bool _disposedValue;

        public WorkflowExecutorConditional(Expression<Func<TIn, bool>> expression)
        {
            _wrappers.Add(new ExecutorWrapper(expression));
        }

        public void Append(Expression<Func<TIn, bool>> expression)
        {
            _wrappers.Add(new ExecutorWrapper(expression));
        }

        public void Append(IWorkflowExecutor<TIn, object> executor)
        {
            _wrappers.Last().Executor = executor;
        }

        public void SetStop()
        {
            _wrappers.Last().ShouldStop = true;
        }

        public void SetExceptionToThrow(Expression<Func<Exception>> func)
        {
            _wrappers.Last().ExceptionFunc = func;
        }

        public void SetRetry(TimeSpan delay, int maxRetries, Action onRetry)
        {
            ExecutorWrapper wrapper = _wrappers.Last();

            wrapper.Retry = true;

            wrapper.Delay = delay;

            wrapper.RetryCount = maxRetries;

            wrapper.OnRetry = onRetry;
        }

        public object Run(TIn args, CancellationToken token = default)
        {
            var enumerator = _wrappers.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (((Func<TIn, bool>)enumerator.Current.Expression.Compile()).Invoke(args))
                {
                    if (enumerator.Current.ShouldStop || token.IsCancellationRequested)
                    {
                        throw new WorkflowStoppedException();
                    }
                    else if (enumerator.Current.ExceptionFunc != null)
                    {
                        throw enumerator.Current.ExceptionFunc.Compile().Invoke();
                    }
                    else if (enumerator.Current.Retry)
                    {
                        if (enumerator.Current.RetryCount == 0) throw new WorkflowMaxRetryException();

                        --enumerator.Current.RetryCount;

                        if (enumerator.Current.OnRetry == null) throw new InvalidOperationException("Internal error with null OnRetry callback");

                        Thread.Sleep(enumerator.Current.Delay);

                        enumerator.Current.OnRetry();
                    }

                    return enumerator.Current.Executor?.Run(args, token);
                }
            }

            // If no condition was met then default to a WorkflowConditionResult
            return new WorkflowResult("No condition was met");
        }

        private sealed class ExecutorWrapper
        {
            public LambdaExpression Expression { get; set; }

            public IWorkflowExecutor<TIn, object> Executor { get; set; }

            public Expression<Func<Exception>> ExceptionFunc { get; set; }

            public TimeSpan Delay { get; set; }

            public bool Retry { get; set; }

            public int RetryCount { get; set; }

            public Action OnRetry { get; set; }

            public bool ShouldStop { get; set; } = false;

            public ExecutorWrapper(LambdaExpression expression)
            {
                Expression = expression;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _wrappers = null;
                }

                _disposedValue = true;
            }
        }

        ~WorkflowExecutorConditional()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }
    }
}
