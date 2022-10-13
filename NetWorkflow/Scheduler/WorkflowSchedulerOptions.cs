
namespace NetWorkflow.Scheduler
{
    /// <summary>
    /// The options to use within a WorkflowScheduler.
    /// </summary>
    public class WorkflowSchedulerOptions
    {
        private const string _changeExceptionMessage = "Cannot change the WorkflowSchedulerOptions after they have been set.";

        internal bool _atTimeSet = false;

        private WorkflowTime _executeAt;

        /// <summary>
        /// The specific time when a Workflow should be executed.
        /// </summary>
        /// <remarks>
        /// A specific day/hour/minute, hour/minute, or minute mark can be determined to run the Workflow.
        /// </remarks>
        public WorkflowTime ExecuteAt
        {
            get => _executeAt;
            set
            {
                if (_atTimeSet) throw new InvalidOperationException(_changeExceptionMessage);

                _atTimeSet = true;

                _executeAt = value;
            }
        }


        internal bool _executeAsyncSet = false;

        private bool _executeAsync = false;

        /// <summary>
        /// Determines if the WorkflowScheduler should execute each Workflow on it's own thread via the RunAsync method.
        /// </summary>
        public bool ExecuteAsync
        {
            get => _executeAsync;
            set
            {
                if (_executeAsyncSet) throw new InvalidOperationException(_changeExceptionMessage);

                _executeAsyncSet = true;

                _executeAsync = value;
            }
        }
    }
}
