
using System;

namespace NetWorkflow.Scheduler
{
    /// <summary>
    /// The options to use within a WorkflowScheduler.
    /// </summary>
    public class WorkflowSchedulerConfiguration<TResult>
    {
        private readonly string _changeExceptionMessage = $"Cannot change the {nameof(WorkflowSchedulerConfiguration<TResult>)} after initial definition.";

        private bool _executeAtSet = false;

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

        private bool _onExecutedSet = false;

        private Action<WorkflowResult<TResult>> _onExecuted;

        /// <summary>
        /// Provides a hook into retrieving the result of an executed Workflow.
        /// <remarks>Will be called once a Workflow has been completed, canceled or faulted.</remarks>
        /// </summary>
        public Action<WorkflowResult<TResult>> OnExecuted
        {
            get => _onExecuted;
            set
            {
                if (_onExecutedSet) throw new InvalidOperationException(_changeExceptionMessage);

                _onExecutedSet= true;

                _onExecuted = value;
            }
        }

        internal WorkflowSchedulerConfiguration() { }
    }
}
