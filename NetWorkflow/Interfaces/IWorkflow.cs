
namespace NetWorkflow
{
    public interface IWorkflow { } 

    /// <summary>
    /// Defines the required members and methods of the Workflow
    /// </summary>
    /// <typeparam name="TContext">The context of the Workflow</typeparam>
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
        public void Run(CancellationToken token = default);
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
        public abstract void Build(IWorkflowBuilder<TContext> builder);

        /// <summary>
        /// Runs the Workflow and provides a WorkflowResult
        /// </summary>
        /// <returns>A WorkflowResult</returns>
        public TResult? Run(CancellationToken token = default);
    }
}
