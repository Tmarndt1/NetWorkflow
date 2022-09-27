namespace NetWorkflow
{
    public abstract class Workflow : IWorkflow { }

    /// <summary>
    /// Defines a Workflow and runs the various steps in sequence
    /// </summary>
    /// <typeparam name="TContext">The Workflow's context to be passed between the steps</typeparam>
    public abstract class Workflow<TContext, TData> : Workflow, IWorkflow<TContext, TData>
    {
        private readonly WorkflowBuilder<TContext> _next;

        private readonly WorkflowOptions? _options;

        /// <summary>
        /// Workflow constructor that requires a context to optionally be passed and used within the various steps
        /// </summary>
        /// <param name="context">The context to optionally be passed and used throughout the various steps</param>
        protected Workflow(TContext context)
        {
            _next = new WorkflowBuilder<TContext>(context);

            Build(_next);
        }

        /// <summary>
        /// Workflow constructor that requires a context to optionally be passed and used within the various steps
        /// </summary>
        /// <param name="context">The context to optionally be passed and used throughout the various steps</param>
        /// <param name="options">The WorkflowOptions to pass within a Workflow to provide tailored functionality</param>
        protected Workflow(TContext context, WorkflowOptions options)
        {
            _next = new WorkflowBuilder<TContext>(context);

            _options = options;

            Build(_next);
        }

        /// <summary>
        /// Abstract method that injects a IWorkflowBuilder to build the steps of the Workflow
        /// </summary>
        /// <param name="builder">The IWorkflowBuilder to build the Workflow's steps</param>
        public abstract IWorkflowBuilder<TContext, TData> Build(IWorkflowBuilder<TContext> builder);

        /// <summary>
        /// Runs the Workflow and returns a final result if each step has executed successfully
        /// </summary>
        /// <param name="token">The CancellationToken to cancel the workflow</param>
        /// <returns>A generic WorkflowResult</returns>
        public WorkflowResult<TData> Run(CancellationToken token = default)
        {
            DateTime timestamp = DateTime.Now;

            try
            {
                object? result = _next.Run(null, token);

                if (result is WorkflowStopped stopped)
                {
                    return WorkflowResult<TData>.Cancelled(DateTime.Now - timestamp);
                }

                return WorkflowResult<TData>.Success((TData?)result, DateTime.Now - timestamp);
            }
            catch (OperationCanceledException)
            {
                if (_options?.Rethrow == true) throw;

                return WorkflowResult<TData>.Cancelled(DateTime.Now - timestamp);
            }
            catch (Exception ex)
            {
                if (_options?.Rethrow == true) throw;

                return WorkflowResult<TData>.Faulted(ex.InnerException ?? ex, DateTime.Now - timestamp);
            }
        }
        /// <summary>
        /// Runs the Workflow asynchronously and returns a final result if each step has executed successfully
        /// </summary>
        /// <param name="token">The CancellationToken to cancel the workflow</param>
        /// <returns>A Task with a generic WorkflowResult</returns>
        public Task<WorkflowResult<TData>> RunAsync(CancellationToken token = default)
        {
            return Task.Run(() => Run(token), token);
        }
    }
}