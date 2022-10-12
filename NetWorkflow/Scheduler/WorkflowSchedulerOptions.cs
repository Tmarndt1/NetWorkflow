
namespace NetWorkflow.Scheduler
{
    /// <summary>
    /// The options to use within a WorkflowScheduler.
    /// </summary>
    public class WorkflowSchedulerOptions
    {
        private const string _changeExceptionMessage = "Cannot change the WorkflowSchedulerOptions after they have been set.";

        internal bool _frequencySet = false;

        private TimeSpan _frequency = TimeSpan.Zero;

        /// <summary>
        /// The frequency on how often the Workflow will be invoked.
        /// </summary>
        /// <remarks>Default value is TimeSpan.Zero.</remarks>
        public TimeSpan Frequency
        {
            get => _frequency;
            set
            {
                // A user should configure a WorkflowScheduler to either execute at a specific time or frequency.
                if (_atTimeSet) throw new InvalidOperationException($"Cannot set both {nameof(AtTime)} and {nameof(Frequency)}.");

                if (_frequencySet) throw new InvalidOperationException(_changeExceptionMessage);

                _frequencySet = true;

                _frequency = value;
            }
        }

        internal bool _atTimeSet = false;

        private WorkflowTime _atTime;

        /// <summary>
        /// The specific time when a Workflow should be invoked.
        /// </summary>
        /// <remarks>
        /// A specific day/hour/minute, hour/minute, or minute mark can be determined to run the Workflow.
        /// </remarks>
        public WorkflowTime AtTime
        {
            get => _atTime;
            set
            {
                // A user should configure a WorkflowScheduler to either execute at a specific time or frequency.
                if (_frequencySet) throw new InvalidOperationException($"Cannot set both {nameof(Frequency)} and {nameof(AtTime)}.");

                if (_atTimeSet) throw new InvalidOperationException(_changeExceptionMessage);

                _atTimeSet = true;

                _atTime = value;
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
