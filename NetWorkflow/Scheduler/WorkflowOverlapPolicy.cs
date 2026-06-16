namespace NetWorkflow.Scheduler
{
    /// <summary>
    /// Determines how the scheduler behaves when a scheduled execution is due while a previous execution is still running.
    /// </summary>
    public enum WorkflowOverlapPolicy
    {
        /// <summary>
        /// Wait for each execution to complete before scheduling the next execution.
        /// </summary>
        Wait,

        /// <summary>
        /// Skip a due execution when a previous execution is still running.
        /// </summary>
        Skip,

        /// <summary>
        /// Allow multiple executions to run at the same time.
        /// </summary>
        AllowConcurrent
    }
}
