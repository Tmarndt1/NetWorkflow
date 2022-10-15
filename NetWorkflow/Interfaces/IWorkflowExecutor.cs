using System.Threading;

namespace NetWorkflow
{
    public interface IWorkflowExecutor { }

    public interface IWorkflowExecutor<in T, out TResult> : IWorkflowExecutor
    {
        public TResult Run(T args, CancellationToken token = default);
    }
}
