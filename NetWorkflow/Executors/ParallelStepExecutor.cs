using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace NetWorkflow
{
    internal class ParallelStepExecutor<TIn, TOut> : IWorkflowExecutor
    {
        private readonly Func<IEnumerable<IWorkflowStepAsync<TIn, TOut>>> _factory;

        private bool _disposedValue;

        public ParallelStepExecutor(Expression<Func<IEnumerable<IWorkflowStepAsync<TIn, TOut>>>> expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            _factory = expression.Compile();
        }

        public async ValueTask<object> RunAsync(object args, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var steps = _factory.Invoke()
                ?? throw new InvalidOperationException("IWorkflowStep cannot be null");

            var typedArgs = (TIn)args;
            var tasks = steps.Select(step =>
            {
                if (step == null)
                    throw new InvalidOperationException("IWorkflowStep cannot be null");

                return step.RunAsync(typedArgs, token);
            }).ToArray();

            return await Task.WhenAll(tasks).ConfigureAwait(false);
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
