namespace NetWorkflow
{
    /// <summary>
    /// Defines a Workflow and runs the various WorkflowSteps in sequence that are established within the Build method.
    /// </summary>
    public abstract class Workflow<TOut> : IWorkflow<TOut>
    {
        private readonly WorkflowBuilder _next;

        private readonly WorkflowOptions _options;

        /// <summary>
        /// Workflow constructor.
        /// </summary>
        protected Workflow()
        {
            _next = new WorkflowBuilder();
        }

        /// <summary>
        /// Overloaded Workflow constructor that requires WorkflowOptions for enhanced usablility.
        /// </summary>
        /// <param name="options">The WorkflowOptions to pass within a Workflow to provide tailored functionality.</param>
        protected Workflow(WorkflowOptions options) : this()
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
            // Builds the Workflow
            Build(_next);

            DateTime timestamp = DateTime.Now;

            try
            {
                TOut result = (TOut)_next.Run(default, token);

                return WorkflowResult<TOut>.Success(result, DateTime.Now - timestamp);
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