
using System;
using System.Threading;

namespace NetWorkflow
{
    internal class WorkflowMoveNextExecutor<TArgs> : IWorkflowExecutor<TArgs, TArgs>
    {
        private bool _disposedValue;

        public TArgs Run(TArgs args, CancellationToken token = default) => args;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }
    }
}
