using NetWorkflow.Exceptions;

namespace NetWorkflow.Scheduler
{
    /// <summary>
    /// WorflowTime represents when a WorkflowScheduler should execute a Workflow
    /// </summary>
    /// <remarks>
    /// Days span from 1 to 31.
    /// Hours are in military time so they span from 1 to 24.
    /// Minutes span from 0 to 59.
    /// </remarks>
    public class WorkflowTime
    {
        private int _day = -1;

        /// <summary>
        /// The day of the month a Workflow should be executed.
        /// </summary>
        public int Day
        {
            get => _day;
            private set
            {
                if (value < 1) throw new WorkflowInvalidValueException("A month cannot be less than 1");

                if (value > 31) throw new WorkflowInvalidValueException("A month cannot be greater than 12");

                _day = value;
            }
        }

        private int _hour = -1;

        /// <summary>
        /// The hour of the day a Workflow should be executed.
        /// </summary>
        public int Hour
        {
            get => _hour;
            private set
            {
                if (value < 1) throw new WorkflowInvalidValueException("A hour cannot be less than 1");

                if (value > 24) throw new WorkflowInvalidValueException("A hour cannot be greater than 24");

                _hour = value;
            }
        }

        private int _minute = -1;

        /// <summary>
        /// The minute of the hour a Workflow should be executed.
        /// </summary>
        public int Minute
        {
            get => _minute;
            private set
            {
                if (value < 0) throw new WorkflowInvalidValueException("A minute cannot be less than 0");

                if (value > 59) throw new WorkflowInvalidValueException("A minute cannot be greater than 59");

                _minute = value;
            }
        }

        /// <summary>
        /// Designates the WorkTime to repeat indefinitely.
        /// </summary>
        public bool Repeat { get; private set; }

        /// <summary>
        /// Designates a WorkflowScheduler should execute a Workflow on the given
        /// month, at the given hour, and at the given minute. 
        /// </summary>
        /// <param name="day">The day of the month the Workflow should be executed.</param>
        /// <param name="hour">The hour of the day the Workflow should be executed.</param>
        /// <param name="minute">The minute of the hour the Workflow should be executed.</param>
        /// <returns>A new instance of WorkflowTime.</returns>
        public static WorkflowTime AtDay(int day, int hour, int minute)
        {
            return new WorkflowTime(day, hour, minute);
        }

        /// <summary>
        /// Designates a WorkflowScheduler should execute a Workflow on the given
        /// month, at the given hour, and at the given minute indefinitely.
        /// </summary>
        /// <param name="day">The day of the month the Workflow should be executed.</param>
        /// <param name="hour">The hour of the day the Workflow should be executed.</param>
        /// <param name="minute">The minute of the hour the Workflow should be executed.</param>
        /// <returns>A new instance of WorkflowTime.</returns>
        /// <remarks>
        /// The WorkflowScheduler will kick off a new Workflow at the day/hour/minute mark of each month indefinitely.
        /// </remarks>
        public static WorkflowTime AtDayIndefinitely(int day, int hour, int minute)
        {
            return new WorkflowTime(day, hour, minute)
            {
                Repeat = true
            };
        }

        /// <summary>
        /// Designates a WorkflowScheduler should execute a Workflow at the given
        /// hour and at the given minute. 
        /// </summary>
        /// <param name="hour">The hour of the day the Workflow should be executed.</param>
        /// <param name="minute">The minute of the hour the Workflow should be executed.</param>
        /// <returns>A new instance of WorkflowTime.</returns>
        public static WorkflowTime AtHour(int hour, int minute)
        {
            return new WorkflowTime(hour, minute);
        }

        /// <summary>
        /// Designates a WorkflowScheduler should execute a Workflow at the given
        /// hour and at the given minute indefinitely. 
        /// </summary>
        /// <param name="hour">The hour of the day the Workflow should be executed.</param>
        /// <param name="minute">The minute of the hour the Workflow should be executed.</param>
        /// <returns>A new instance of WorkflowTime.</returns>
        /// <remarks>
        /// The WorkflowScheduler will kick off a new Workflow at the hour/minute mark of each day indefinitely.
        /// </remarks>
        public static WorkflowTime AtHourIndefinitely(int hour, int minute)
        {
            return new WorkflowTime(hour, minute)
            {
                Repeat = true
            };
        }

        /// <summary>
        /// Designates a WorkflowScheduler should execute a Workflow at the given minute.
        /// </summary>
        /// <param name="minute">The minute of the hour the Workflow should be executed.</param>
        /// <returns>A new instance of WorkflowTime.</returns>
        public static WorkflowTime AtMinute(int minute)
        {
            return new WorkflowTime(minute);
        }

        /// <summary>
        /// Designates a WorkflowScheduler should execute a Workflow at the given minute indefinitely.
        /// </summary>
        /// <param name="minute">The minute of the hour the Workflow should be executed.</param>
        /// <returns>A new instance of WorkflowTime.</returns>
        /// <remarks>
        /// The WorkflowScheduler will kick off a new Workflow at the minute mark of each hour indefinitely.
        /// </remarks>
        public static WorkflowTime AtMinuteIndefinitely(int minute)
        {
            return new WorkflowTime(minute)
            {
                Repeat = true
            };
        }

        private WorkflowTime(int day, int hour, int minute) : this(hour, minute)
        {
            Day = day;
        }

        private WorkflowTime(int hour, int minute) : this(minute)
        {
            Hour = hour;
        }

        private WorkflowTime(int minute)
        {
            Minute = minute;
        }
    }
}

