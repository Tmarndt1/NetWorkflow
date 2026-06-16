using System.Threading;
using System.Threading.Tasks;

namespace NetWorkflow.Extensions
{
    /// <summary>
    /// Runs a Workflow resolved from the dependency injection container.
    /// </summary>
    /// <typeparam name="TWorkflow">The type of Workflow to run.</typeparam>
    /// <typeparam name="TResult">The type of result produced by the Workflow.</typeparam>
    public interface IWorkflowRunner<TWorkflow, TResult>
        where TWorkflow : IWorkflow<TResult>
    {
        /// <summary>
        /// Resolves and runs the Workflow asynchronously.
        /// </summary>
        /// <param name="token">The CancellationToken to cancel the Workflow.</param>
        /// <returns>A WorkflowResult with TResult data.</returns>
        Task<WorkflowResult<TResult>> RunAsync(CancellationToken token = default);
    }
}
