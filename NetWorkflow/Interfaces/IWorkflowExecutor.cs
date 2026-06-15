using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetWorkflow
{
    internal interface IWorkflowExecutor : IDisposable
    {
        ValueTask<object> RunAsync(object args, CancellationToken token = default);
    }
}
