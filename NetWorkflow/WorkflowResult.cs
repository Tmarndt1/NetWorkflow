using System;

namespace NetWorkflow
{
    /// <summary>
    /// Represents the result of a workflow, providing information about its execution.
    /// </summary>
    /// <typeparam name="TOut">The type of the Workflow's output.</typeparam>
    public sealed class WorkflowResult<TOut>
    {
        private WorkflowResult(WorkflowResultStatus status, TOut output, Exception exception, TimeSpan duration)
        {
            Status = status;
            Output = output;
            Exception = exception;
            Duration = duration;
        }

        /// <summary>
        /// The final status of the Workflow.
        /// </summary>
        public WorkflowResultStatus Status { get; }

        /// <summary>
        /// A user friendly string message capturing the end result of the Workflow.
        /// </summary>
        public string Message
        {
            get
            {
                switch (Status)
                {
                    case WorkflowResultStatus.Completed:
                        return "The Workflow has completed successfully.";
                    case WorkflowResultStatus.Canceled:
                        return "The Workflow was canceled.";
                    case WorkflowResultStatus.Faulted:
                        return "The Workflow was stopped because an exception was thrown.";
                    default:
                        throw new InvalidOperationException("Invalid Workflow result status.");
                }
            }
        }

        /// <summary>
        /// The caught exception if thrown within the steps.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// The duration from when the Workflow was initially ran until the WorkflowResult is returned.
        /// </summary>
        public TimeSpan Duration { get; }

        /// <summary>
        /// The Workflow's output.
        /// </summary>
        public TOut Output { get; }

        /// <summary>
        /// Attempts to get the Workflow's output.
        /// </summary>
        /// <param name="output">The Workflow output when the Workflow completed successfully.</param>
        /// <returns>True when the Workflow completed successfully; otherwise false.</returns>
        public bool TryGetOutput(out TOut output)
        {
            output = Output;

            return Status == WorkflowResultStatus.Completed;
        }

        /// <summary>
        /// Throws the caught exception when the Workflow faulted.
        /// </summary>
        public void ThrowIfFaulted()
        {
            if (Exception != null)
            {
                throw Exception;
            }
        }

        /// <summary>
        /// Gets the Workflow output or throws if the Workflow did not complete successfully.
        /// </summary>
        /// <returns>The Workflow output.</returns>
        public TOut GetOutputOrThrow()
        {
            ThrowIfFaulted();

            if (Status == WorkflowResultStatus.Canceled)
            {
                throw new OperationCanceledException(Message);
            }

            return Output;
        }

        internal static WorkflowResult<TOut> Success(TOut output, TimeSpan duration)
        {
            return new WorkflowResult<TOut>(WorkflowResultStatus.Completed, output, null, duration);
        }

        internal static WorkflowResult<TOut> Faulted(Exception ex, TimeSpan duration)
        {
            return new WorkflowResult<TOut>(WorkflowResultStatus.Faulted, default, ex, duration);
        }

        internal static WorkflowResult<TOut> Cancelled(TimeSpan duration)
        {
            return new WorkflowResult<TOut>(WorkflowResultStatus.Canceled, default, null, duration);
        }

        public static implicit operator TOut(WorkflowResult<TOut> result) => result.Output;
    }
}
