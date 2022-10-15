
using System.Threading;

namespace NetWorkflow
{
    public class WorkflowPassiveExecutor<TArgs> : IWorkflowExecutor<TArgs, TArgs>
    {
        public TArgs Run(TArgs args, CancellationToken token = default) => args;
    }
}
