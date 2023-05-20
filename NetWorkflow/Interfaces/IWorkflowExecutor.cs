using System;
using System.Threading;

namespace NetWorkflow
{
    internal interface IWorkflowExecutor : IDisposable { }

    internal interface IWorkflowExecutor<in TIn, out TOut> : IWorkflowExecutor
    {
        TOut Run(TIn args, CancellationToken token = default);
    }
}
