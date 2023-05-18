using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace NetWorkflow
{
    internal class WorkflowExecutorConditional<TIn> : IWorkflowExecutor<TIn, object>
    {
        private readonly List<ExecutorWrapper> _wrappers = new List<ExecutorWrapper>();

        private ExecutorWrapper _active => _wrappers.Last();

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
            _active.Executor = executor;
        }

        public void Stop()
        {
            _active.ShouldStop = true;
        }

        public void OnExceptionDo(Expression<Func<Exception>> func)
        {
            _active.OnException = func;
        }

        public void SetRetry(TimeSpan delay, int maxRetries, Action onRetry)
        {
            var wrapper = _active;

            wrapper.Retry = true;
            wrapper.Delay = delay;
            wrapper.RetryCount = maxRetries;
            wrapper.OnRetry = onRetry;
        }

        public object Run(TIn args, CancellationToken token = default)
        {
            foreach (var wrapper in _wrappers)
            {
                var condition = wrapper.Expression.Compile() as Func<TIn, bool>;

                if (condition.Invoke(args))
                {
                    if (wrapper.ShouldStop || token.IsCancellationRequested)
                        throw new WorkflowStoppedException();

                    if (wrapper.OnException != null)
                        throw wrapper.OnException.Compile()();

                    if (wrapper.Retry)
                    {
                        if (wrapper.RetryCount == 0)
                            throw new WorkflowMaxRetryException();

                        wrapper.RetryCount--;

                        if (wrapper.OnRetry == null)
                            throw new InvalidOperationException("Internal error with null OnRetry callback");

                        Thread.Sleep(wrapper.Delay);

                        wrapper.OnRetry();
                    }

                    return wrapper.Executor?.Run(args, token);
                }
            }

            // If no condition was met then default to a WorkflowConditionResult
            return new WorkflowResult("No condition was met");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    foreach (var wrapper in _wrappers)
                        (wrapper.Executor as IDisposable)?.Dispose();

                    _wrappers.Clear();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }

        private sealed class ExecutorWrapper
        {
            public LambdaExpression Expression { get; }
            public IWorkflowExecutor<TIn, object> Executor { get; set; }
            public Expression<Func<Exception>> OnException { get; set; }
            public TimeSpan Delay { get; set; }
            public bool Retry { get; set; }
            public int RetryCount { get; set; }
            public Action OnRetry { get; set; }
            public bool ShouldStop { get; set; }

            public ExecutorWrapper(Expression<Func<TIn, bool>> expression)
            {
                Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            }
        }
    }
}
