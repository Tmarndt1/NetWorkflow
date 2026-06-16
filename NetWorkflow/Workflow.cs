using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace NetWorkflow
{
    /// <summary>
    /// Defines a Workflow and runs the various WorkflowSteps in sequence that are established within the Build method.
    /// </summary>
    public abstract class Workflow<TOut> : IWorkflow<TOut>, IDisposable
    {
        private WorkflowOptions _options;

        private bool _disposedValue;

        /// <summary>
        /// Workflow constructor.
        /// </summary>
        protected Workflow()
        {
        }

        /// <summary>
        /// Overloaded Workflow constructor that requires WorkflowOptions for enhanced usablility.
        /// </summary>
        /// <param name="options">The WorkflowOptions to pass within a Workflow to provide tailored functionality.</param>
        protected Workflow(WorkflowOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Abstract method that injects a IWorkflowBuilder to build the steps of the Workflow.
        /// The Workflow is lazily built when the Run method is invoked.
        /// </summary>
        /// <param name="builder">The IWorkflowBuilder to build the Workflow's steps.</param>
        public abstract IWorkflowBuilder<TOut> Build(IWorkflowBuilder builder);

        /// <summary>
        /// Builds and runs the Workflow and returns a final result if each step has executed successfully.
        /// </summary>
        /// <param name="token">The CancellationToken to cancel the workflow.</param>
        /// <returns>A generic WorkflowResult.</returns>
        public WorkflowResult<TOut> Run(CancellationToken token = default)
        {
            if (_disposedValue)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            var stopwatch = Stopwatch.StartNew();

            try
            {
                using var builder = new WorkflowBuilder();

                Build(builder);

                TOut result = (TOut)builder.Run(default, token);

                return WorkflowResult<TOut>.Success(result, stopwatch.Elapsed);
            }
            catch (OperationCanceledException)
            {
                if (_options?.RethrowExceptions == true) throw;

                return WorkflowResult<TOut>.Cancelled(stopwatch.Elapsed);
            }
            catch (WorkflowStoppedException)
            {
                if (_options?.RethrowExceptions == true) throw;

                return WorkflowResult<TOut>.Cancelled(stopwatch.Elapsed);
            }
            catch (Exception ex)
            {
                if (_options?.RethrowExceptions == true) throw;

                return WorkflowResult<TOut>.Faulted(ex.InnerException ?? ex, stopwatch.Elapsed);
            }
        }

        /// <summary>
        /// Builds and runs the Workflow asynchronously and returns a final result if each step has executed successfully
        /// </summary>
        /// <param name="token">The CancellationToken to cancel the workflow.</param>
        /// <returns>A Task with a generic WorkflowResult.</returns>
        public async Task<WorkflowResult<TOut>> RunAsync(CancellationToken token = default)
        {
            if (_disposedValue)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            var stopwatch = Stopwatch.StartNew();

            try
            {
                using var builder = new WorkflowBuilder();

                Build(builder);

                TOut result = (TOut)await builder.RunAsync(default, token).ConfigureAwait(false);

                return WorkflowResult<TOut>.Success(result, stopwatch.Elapsed);
            }
            catch (OperationCanceledException)
            {
                if (_options?.RethrowExceptions == true) throw;

                return WorkflowResult<TOut>.Cancelled(stopwatch.Elapsed);
            }
            catch (WorkflowStoppedException)
            {
                if (_options?.RethrowExceptions == true) throw;

                return WorkflowResult<TOut>.Cancelled(stopwatch.Elapsed);
            }
            catch (Exception ex)
            {
                if (_options?.RethrowExceptions == true) throw;

                return WorkflowResult<TOut>.Faulted(ex.InnerException ?? ex, stopwatch.Elapsed);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _options = null;
                }

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
