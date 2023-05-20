using System;

namespace NetWorkflow
{
    /// <summary>
    /// Represents the result of a workflow, providing information about its execution.
    /// </summary>
    /// <typeparam name="TOut">The type of the Workflow's output.</typeparam>
    public sealed class WorkflowResult<TOut>
    {
        /// <summary>
        /// A user friendly string message capturing the end result of the Workflow.
        /// </summary>
        public string Message { get; private set; } = "The Workflow completed successfully.";

        /// <summary>
        /// Boolean value on whether the Workflow has successfully completed all its steps.
        /// </summary>
        public bool IsCompleted { get; private set; } = false;

        /// <summary>
        /// Boolean value on whether the Workflow has catched an exception within the steps.
        /// </summary>
        public bool IsFaulted => Exception != null;

        /// <summary>
        /// Boolean value on whether the Worfklow has been canceled.
        /// </summary>
        public bool IsCanceled { get; private set; } = false;

        /// <summary>
        /// The catched exception if thrown within the steps
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// The duration from when the Workflow was initially ran until the WorkflowResult is returned.
        /// </summary>
        public TimeSpan Duration { get; private set; }

        /// <summary>
        /// The Workflow's output.
        /// </summary>
        public TOut Output { get; private set; }

        internal static WorkflowResult<TOut> Success(TOut output, TimeSpan duration)
        {
            return new WorkflowResult<TOut>()
            {
                Message = "The Workflow has completed successfully.",
                Output = output,
                Duration = duration,
                IsCompleted = true
            };
        }

        internal static WorkflowResult<TOut> Faulted(Exception ex, TimeSpan duration)
        {
            return new WorkflowResult<TOut>()
            {
                Message = "The Workflow was stopped because an exception was thrown.",
                Exception = ex,
                Duration = duration
            };
        }

        internal static WorkflowResult<TOut> Cancelled(TimeSpan duration)
        {
            return new WorkflowResult<TOut>()
            {
                Message = "The Workflow was canceled.",
                IsCanceled = true,
                Duration = duration
            };
        }

        public static implicit operator TOut(WorkflowResult<TOut> result) => result.Output;
    }
}
