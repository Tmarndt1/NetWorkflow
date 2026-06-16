using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace NetWorkflow
{
    internal class ConditionalExecutor<TIn> : IWorkflowExecutor
    {
        private readonly List<ConditionBranch> _branches = new List<ConditionBranch>();

        private ConditionBranch _active => _branches.Last();

        private bool _disposedValue;

        public ConditionalExecutor(Expression<Func<TIn, bool>> expression)
        {
            _branches.Add(new ConditionBranch(expression));
        }

        public void Append(Expression<Func<TIn, bool>> expression)
        {
            _branches.Add(new ConditionBranch(expression));
        }

        public void Append(IWorkflowExecutor executor)
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
            var branch = _active;

            branch.Retry = true;
            branch.Delay = delay;
            branch.RetryCount = maxRetries;
            branch.OnRetry = onRetry;
        }

        public async ValueTask<object> RunAsync(object args, CancellationToken token = default)
        {
            var typedArgs = (TIn)args;

            foreach (var branch in _branches)
            {
                if (branch.Condition.Invoke(typedArgs))
                {
                    token.ThrowIfCancellationRequested();

                    if (branch.ShouldStop)
                        throw new WorkflowStoppedException();

                    if (branch.OnException != null)
                        throw branch.OnException.Compile().Invoke();

                    if (branch.Retry)
                    {
                        if (branch.RetryCount == 0)
                            throw new WorkflowMaxRetryException();

                        branch.RetryCount--;

                        if (branch.OnRetry == null)
                            throw new InvalidOperationException("Internal error with null OnRetry callback");

                        await Task.Delay(branch.Delay, token).ConfigureAwait(false);

                        branch.OnRetry();
                    }

                    if (branch.Executor == null)
                        return null;

                    return await branch.Executor.RunAsync(args, token).ConfigureAwait(false);
                }
            }

            throw new WorkflowNoConditionMetException();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    foreach (var branch in _branches)
                        branch.Executor?.Dispose();

                    _branches.Clear();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }

        private sealed class ConditionBranch
        {
            public Func<TIn, bool> Condition { get; }
            public IWorkflowExecutor Executor { get; set; }
            public Expression<Func<Exception>> OnException { get; set; }
            public TimeSpan Delay { get; set; }
            public bool Retry { get; set; }
            public int RetryCount { get; set; }
            public Action OnRetry { get; set; }
            public bool ShouldStop { get; set; }

            public ConditionBranch(Expression<Func<TIn, bool>> expression)
            {
                if (expression == null) throw new ArgumentNullException(nameof(expression));

                Condition = expression.Compile();
            }
        }
    }
}
