
namespace NetWorkflow
{
    public interface IWorkflow { }

    /// <summary>
    /// Defines the required members and methods of the Workflow
    /// </summary>
    /// <typeparam name="TContext">The Workflow's context type</typeparam>
    public interface IWorkflow<TContext> : IWorkflow
    {
        /// <summary>
        /// Abstract method that injects a IWorkflowBuilder to build the steps of the Workflow
        /// </summary>
        /// <param name="builder">The IWorkflowBuilder to build the Workflow's steps</param>
        public abstract void Build(IWorkflowBuilder<TContext> builder);

        /// <summary>
        /// Runs the Workflow and provides a WorkflowResult
        /// </summary>
        /// <param name="token">The CancellationToken to cancel the Workflow</param>
        /// <returns>A task with a WorkflowResult with no data</returns>
        public WorkflowResult Run(CancellationToken token = default);

        /// <summary>
        /// Runs the Workflow asynchronously and provides a WorkflowResult
        /// </summary>
        /// <param name="token">The CancellationToken to cancel the Workflow</param>
        /// <returns>A task with a WorkflowResult with no data</returns>
        public Task<WorkflowResult> RunAsync(CancellationToken token = default);
    }

    /// <summary>
    /// Defines the required members and methods of the Workflow
    /// </summary>
    /// <typeparam name="TContext">The context of the Workflow</typeparam>
    /// <typeparam name="TResult">The result of the Workflow</typeparam>
    public interface IWorkflow<TContext, TResult> : IWorkflow
    {
        /// <summary>
        /// Abstract method that injects a IWorkflowBuilder to build the steps of the Workflow
        /// </summary>
        /// <param name="builder">The IWorkflowBuilder to build the Workflow's steps</param>
        public abstract IWorkflowBuilder<TContext, TResult> Build(IWorkflowBuilder<TContext> builder);

        /// <summary>
        /// Runs the Workflow and provides a WorkflowResult
        /// </summary>
        /// <param name="token">The CancellationToken to cancel the Workflow</param>
        /// <returns>A WorkflowResult with TResult data</returns>
        public WorkflowResult<TResult> Run(CancellationToken token = default);

        /// <summary>
        /// Runs the Workflow asynchronously and provides a WorkflowResult
        /// </summary>
        /// <param name="token">The CancellationToken to cancel the Workflow</param>
        /// <returns>A a task with a WorkflowResult with TResult data</returns>
        public Task<WorkflowResult<TResult>> RunAsync(CancellationToken token = default);
    }
}
