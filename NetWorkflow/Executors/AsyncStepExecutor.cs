using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace NetWorkflow
{
    internal class AsyncStepExecutor<TIn, TOut> : IWorkflowExecutor
    {
        private readonly Func<IWorkflowStepAsync<TIn, TOut>> _factory;

        private bool _disposedValue;

        public AsyncStepExecutor(Expression<Func<IWorkflowStepAsync<TIn, TOut>>> expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            _factory = expression.Compile();
        }

        public async ValueTask<object> RunAsync(object args, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var step = _factory.Invoke()
                ?? throw new InvalidOperationException("IWorkflowStep cannot be null");

            return await step.RunAsync((TIn)args, token).ConfigureAwait(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }
    }
}
