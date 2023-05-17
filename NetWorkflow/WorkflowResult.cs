using System;

namespace NetWorkflow
{
    /// <summary>
    /// Represents the result of a workflow, providing information about its execution.
    /// </summary>
    public class WorkflowResult
    {
        /// <summary>
        /// A user friendly string message capturing the end result of the Workflow
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// Boolean value on whether the Workflow has successfully completed all its steps
        /// </summary>
        public bool IsCompleted { get; protected set; } = false;

        /// <summary>
        /// Boolean value on whether the Workflow has catched an exception within the steps
        /// </summary>
        public bool IsFaulted => Exception != null;

        /// <summary>
        /// Boolean value on whether the Worfklow has been cancelled
        /// </summary>
        public bool IsCanceled { get; protected set; } = false;

        /// <summary>
        /// The catched exception if thrown within the steps
        /// </summary>
        public Exception Exception { get; protected set; }

        /// <summary>
        /// The duration from when the Workflow was initially ran until the WorkflowResult is retuend
        /// </summary>
        public TimeSpan Duration { get; protected set; }

        /// <summary>
        /// Constructor that requires a message
        /// </summary>
        /// <param name="message">The required user friendly message.</param>
        internal WorkflowResult(string message)
        {
            Message = message;
        }
    }

    /// <summary>
    /// Represents the result of a workflow, providing information about its execution.
    /// </summary>
    /// <typeparam name="TOut">The type of the Workflow's output.</typeparam>
    public sealed class WorkflowResult<TOut> : WorkflowResult
    {
        /// <summary>
        /// The final resulting data of the Workflow
        /// </summary>
        public TOut Output { get; private set; }

        private WorkflowResult(string message) : base(message) { }

        internal static WorkflowResult<TOut> Success(TOut output, TimeSpan duration)
        {
            return new WorkflowResult<TOut>("The Workflow has completed successfully.")
            {
                Output = output,
                Duration = duration,
                IsCompleted = true
            };
        }
        internal static WorkflowResult<TOut> Faulted(Exception ex, TimeSpan duration)
        {
            return new WorkflowResult<TOut>("The Workflow was stopped because an exception was thrown.")
            {
                Exception = ex,
                Duration = duration
            };
        }

        internal static WorkflowResult<TOut> Cancelled(TimeSpan duration)
        {
            return new WorkflowResult<TOut>("The Workflow was cancelled.")
            {
                IsCanceled = true,
                Duration = duration
            };
        }

        public static implicit operator TOut(WorkflowResult<TOut> result) => result.Output;
    }
}
