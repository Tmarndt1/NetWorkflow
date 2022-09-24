namespace NetWorkflow
{
    public abstract class Workflow : IWorkflow { }

    /// <summary>
    /// Defines a Workflow and runs the various steps in sequence
    /// </summary>
    /// <typeparam name="TContext">The Workflow's context to be passed between the steps</typeparam>
    public abstract class Workflow<TContext, TResult> : Workflow, IWorkflow<TContext, TResult>
    {
        private readonly WorkflowBuilder<TContext> _next;

        private readonly WorkflowOptions? _options;

        protected Workflow(TContext context)
        {
            _next = new WorkflowBuilder<TContext>(context);

            Build(_next);
        }

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
        public abstract IWorkflowBuilder<TContext, TResult> Build(IWorkflowBuilder<TContext> builder);

        /// <summary>
        /// Runs the Workflow and returns a final result if executed step has executed successfully
        /// </summary>
        /// <param name="token">The CancellationToken to cancel the workflow</param>
        /// <returns>A generic WorkflowResult</returns>
        public WorkflowResult<TResult> Run(CancellationToken token = default)
        {
            DateTime timestamp = DateTime.Now;

            try
            {
                object? result = _next.Run(null, token);

                if (result is WorkflowStoppedResult stopped)
                {
                    return WorkflowResult<TResult>.Cancelled(DateTime.Now - timestamp);
                }

                return WorkflowResult<TResult>.Success((TResult?)result, DateTime.Now - timestamp);
            }
            catch (Exception ex)
            {
                if (_options?.RethrowExceptions == true) throw;

                return WorkflowResult<TResult>.Faulted(ex, DateTime.Now - timestamp);
            }
        }
    }
}