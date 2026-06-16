using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace NetWorkflow
{
    internal class SyncStepExecutor<TIn, TOut> : IWorkflowExecutor
    {
        private readonly Func<IWorkflowStep<TIn, TOut>> _factory;

        private bool _disposedValue;

        public SyncStepExecutor(Expression<Func<IWorkflowStep<TIn, TOut>>> expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            _factory = expression.Compile();
        }

        public ValueTask<object> RunAsync(object args, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var step = _factory.Invoke()
                ?? throw new InvalidOperationException("IWorkflowStep cannot be null");

            return new ValueTask<object>(step.Run((TIn)args, token));
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
