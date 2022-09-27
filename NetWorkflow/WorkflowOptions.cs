namespace NetWorkflow
{
    /// <summary>
    /// WorkflowOptions to pass within a Workflow to provide tailored functionality
    /// </summary>
    public class WorkflowOptions
    {
        /// <summary>
        /// Enables re-throwing exceptions within the Workflow when they are caught
        /// </summary>
        public bool Rethrow { get; set; } = false;
    }
}
