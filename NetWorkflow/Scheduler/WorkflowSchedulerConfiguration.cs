using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetWorkflow.Scheduler
{
    /// <summary>
    /// The options to use within a WorkflowScheduler.
    /// </summary>
    public class WorkflowSchedulerConfiguration<TResult>
    {
        private readonly string _changeExceptionMessage = $"Cannot change the {nameof(WorkflowSchedulerConfiguration<TResult>)} after initial definition.";

        private bool _scheduleSet = false;
        private WorkflowSchedule _schedule;

        /// <summary>
        /// The schedule that determines when a Workflow should be executed.
        /// </summary>
        public WorkflowSchedule Schedule
        {
            get => _schedule;
            set
            {
                if (_scheduleSet) throw new InvalidOperationException(_changeExceptionMessage);

                _scheduleSet = true;
                _schedule = value;
            }
        }

        private bool _runImmediatelySet = false;
        private bool _runImmediately = false;

        /// <summary>
        /// Determines whether frequency schedules should execute once immediately before waiting for the first interval.
        /// </summary>
        public bool RunImmediately
        {
            get => _runImmediately;
            set
            {
                if (_runImmediatelySet) throw new InvalidOperationException(_changeExceptionMessage);

                _runImmediatelySet = true;
                _runImmediately = value;
            }
        }

        private bool _overlapPolicySet = false;
        private WorkflowOverlapPolicy _overlapPolicy = WorkflowOverlapPolicy.Wait;

        /// <summary>
        /// Determines how the scheduler handles overlapping executions.
        /// </summary>
        public WorkflowOverlapPolicy OverlapPolicy
        {
            get => _overlapPolicy;
            set
            {
                if (_overlapPolicySet) throw new InvalidOperationException(_changeExceptionMessage);

                _overlapPolicySet = true;
                _overlapPolicy = value;
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

                _onExecutedSet = true;
                _onExecuted = value;
            }
        }

        private bool _onExecutedAsyncSet = false;
        private Func<WorkflowResult<TResult>, CancellationToken, Task> _onExecutedAsync;

        /// <summary>
        /// Provides an async hook into retrieving the result of an executed Workflow.
        /// <remarks>Will be called once a Workflow has been completed, canceled or faulted.</remarks>
        /// </summary>
        public Func<WorkflowResult<TResult>, CancellationToken, Task> OnExecutedAsync
        {
            get => _onExecutedAsync;
            set
            {
                if (_onExecutedAsyncSet) throw new InvalidOperationException(_changeExceptionMessage);

                _onExecutedAsyncSet = true;
                _onExecutedAsync = value;
            }
        }

        private bool _onErrorAsyncSet = false;
        private Func<Exception, CancellationToken, Task> _onErrorAsync;

        /// <summary>
        /// Provides an async hook for scheduler-level errors.
        /// </summary>
        public Func<Exception, CancellationToken, Task> OnErrorAsync
        {
            get => _onErrorAsync;
            set
            {
                if (_onErrorAsyncSet) throw new InvalidOperationException(_changeExceptionMessage);

                _onErrorAsyncSet = true;
                _onErrorAsync = value;
            }
        }

        internal WorkflowSchedulerConfiguration() { }
    }
}
