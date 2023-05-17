using System;

namespace NetWorkflow
{
    public interface IWorkflowResult<TOut>
    {
        TimeSpan Duration { get; }
        Exception Exception { get; }
        bool IsCanceled { get; }
        bool IsCompleted { get; }
        bool IsFaulted { get; }
        TOut Output { get; }
    }
}