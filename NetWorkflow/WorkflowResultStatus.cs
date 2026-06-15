namespace NetWorkflow
{
    /// <summary>
    /// Represents the final status of a Workflow execution.
    /// </summary>
    public enum WorkflowResultStatus
    {
        /// <summary>
        /// The Workflow completed successfully and produced an output.
        /// </summary>
        Completed,

        /// <summary>
        /// The Workflow was canceled before successful completion.
        /// </summary>
        Canceled,

        /// <summary>
        /// The Workflow stopped because an exception was thrown.
        /// </summary>
        Faulted
    }
}
