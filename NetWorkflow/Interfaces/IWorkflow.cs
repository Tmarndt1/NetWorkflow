
using System.Threading;
using System.Threading.Tasks;

namespace NetWorkflow
{
    /// <summary>
    /// Marker interace for all Workflows to implement.
    /// </summary>
    public interface IWorkflow 
    {
        
    }

    /// <summary>
    /// Defines the required members and methods of the Workflow.
    /// </summary>
    /// <typeparam name="TOut">The result of the Workflow.</typeparam>
    public interface IWorkflow<TOut> : IWorkflow
    {
        /// <summary>
        /// Abstract method that injects a IWorkflowBuilder to build the steps of the Workflow.
        /// </summary>
        /// <param name="builder">The IWorkflowBuilder to build the Workflow's steps.</param>
        public abstract IWorkflowBuilder<TOut> Build(IWorkflowBuilder builder);

        /// <summary>
        /// Builds and runs the Workflow and provides a WorkflowResult.
        /// </summary>
        /// <param name="token">The CancellationToken to cancel the Workflow.</param>
        /// <returns>A WorkflowResult with TOut data.</returns>
        public WorkflowResult<TOut> Run(CancellationToken token = default);

        /// <summary>
        /// Builds and runs the Workflow asynchronously and provides a WorkflowResult.
        /// </summary>
        /// <param name="token">The CancellationToken to cancel the Workflow.</param>
        /// <returns>A a task with a WorkflowResult with TOut data.</returns>
        public Task<WorkflowResult<TOut>> RunAsync(CancellationToken token = default);
    }
}
