using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetWorkflow
{
    internal class PassThroughExecutor : IWorkflowExecutor
    {
        private bool _disposedValue;

        public ValueTask<object> RunAsync(object args, CancellationToken token = default)
        {
            return new ValueTask<object>(args);
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
