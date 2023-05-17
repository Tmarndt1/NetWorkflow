
using System;

namespace NetWorkflow.Scheduler
{
    /// <summary>
    /// The options to use within a WorkflowScheduler.
    /// </summary>
    public class WorkflowSchedulerConfiguration
    {
        private const string _changeExceptionMessage = "Cannot change the WorkflowSchedulerOptions after they have been set.";

        internal bool _executeAtSet = false;

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
                if (_executeAtSet) throw new InvalidOperationException(_changeExceptionMessage);

                _executeAtSet = true;

                _executeAt = value;
            }
        }
    }
}
