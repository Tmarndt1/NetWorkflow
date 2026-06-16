namespace NetWorkflow.Scheduler
{
    /// <summary>
    /// Identifies the constructor overload that runs workflows through a custom runner delegate.
    /// </summary>
    public enum WorkflowSchedulerRunnerMode
    {
        /// <summary>
        /// Use the provided runner delegate for each scheduled execution.
        /// </summary>
        CustomRunner
    }
}
