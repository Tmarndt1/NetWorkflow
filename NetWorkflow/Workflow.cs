namespace NetWorkflow
{
    /// <summary>
    /// Defines a Workflow and runs the various steps in sequence.
    /// </summary>
    public abstract class Workflow<TOut> : IWorkflow<TOut>
    {
        private readonly WorkflowBuilder _next;

        private readonly WorkflowOptions? _options;

        /// <summary>
        /// Workflow constructor.
        /// </summary>
        protected Workflow()
        {
            _next = new WorkflowBuilder();
        }

        /// <summary>
        /// Workflow constructor that requires a context to optionally be passed and used within the various steps.
        /// </summary>
        /// <param name="options">The WorkflowOptions to pass within a Workflow to provide tailored functionality.</param>
        protected Workflow(WorkflowOptions options) : this()
        {
            _options = options;
        }

        /// <summary>
        /// Abstract method that injects a IWorkflowBuilder to build the steps of the Workflow.
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
            // Builds the Workflow
            Build(_next);

            DateTime timestamp = DateTime.Now;

            try
            {
                object? result = _next.Run(default, token);

                if (result is WorkflowStopped stopped)
                {
                    return WorkflowResult<TOut>.Cancelled(DateTime.Now - timestamp);
                }

                return WorkflowResult<TOut>.Success((TOut?)result, DateTime.Now - timestamp);
            }
            catch (OperationCanceledException)
            {
                if (_options?.Rethrow == true) throw;

                return WorkflowResult<TOut>.Cancelled(DateTime.Now - timestamp);
            }
            catch (WorkflowStoppedException)
            {
                if (_options?.Rethrow == true) throw;

                return WorkflowResult<TOut>.Cancelled(DateTime.Now - timestamp);
            }
            catch (Exception ex)
            {
                if (_options?.Rethrow == true) throw;

                return WorkflowResult<TOut>.Faulted(ex.InnerException ?? ex, DateTime.Now - timestamp);
            }
        }
        /// <summary>
        /// Builds and runs the Workflow asynchronously and returns a final result if each step has executed successfully
        /// </summary>
        /// <param name="token">The CancellationToken to cancel the workflow.</param>
        /// <returns>A Task with a generic WorkflowResult.</returns>
        public Task<WorkflowResult<TOut>> RunAsync(CancellationToken token = default)
        {
            return Task.Run(() => Run(token), token);
        }
    }
}