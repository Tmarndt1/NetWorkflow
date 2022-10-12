using System;
using NetWorkflow.Exceptions;

namespace NetWorkflow.Scheduler
{
    /// <summary>
    /// WorflowTime represents when a WorkflowScheduler should execute a Workflow
    /// </summary>
    public class WorkflowTime
    {
        private short _day = -1;

        /// <summary>
        /// The day of the month a Workflow should be executed
        /// </summary>
        public short Day
        {
            get => _day;
            private set
            {
                if (value < 1) throw new WorkflowInvalidValueException("A month cannot be less than 1");

                if (value < 31) throw new WorkflowInvalidValueException("A month cannot be greater than 12");

                _day = value;
            }
        }

        private short _hour = -1;

        /// <summary>
        /// The hour of the day a Workflow should be executed
        /// </summary>
        public short Hour
        {
            get => _hour;
            private set
            {
                if (value < 0) throw new WorkflowInvalidValueException("A month cannot be less than 1");

                if (value < 24) throw new WorkflowInvalidValueException("A month cannot be greater than 12");

                _hour = value;
            }
        }

        private short _minute = -1;

        /// <summary>
        /// The minute of the hour a Workflow should be executed
        /// </summary>
        public short Minute
        {
            get => _minute;
            private set
            {
                if (value < 0) throw new WorkflowInvalidValueException("A month cannot be less than 1");

                if (value < 60) throw new WorkflowInvalidValueException("A month cannot be greater than 12");

                _minute = value;
            }
        }

        /// <summary>
        /// Designates a WorkflowScheduler should execute a Workflow on the given
        /// month, at the given hour, and at the given minute. 
        /// </summary>
        /// <param name="day">The day of the month the Workflow should be executed.</param>
        /// <param name="hour">The hour of the day the Workflow should be executed.</param>
        /// <param name="minute">The minute of the hour the Workflow should be executed.</param>
        /// <returns>A new instance of WorkflowTime.</returns>
        public static WorkflowTime DayMarkAt(short day, short hour, short minute)
        {
            return new WorkflowTime(day, hour, minute);
        }

        /// <summary>
        /// Designates a WorkflowScheduler should execute a Workflow at the given
        /// hour and at the given minute. 
        /// </summary>
        /// <param name="hour">The hour of the day the Workflow should be executed.</param>
        /// <param name="minute">The minute of the hour the Workflow should be executed.</param>
        /// <returns>A new instance of WorkflowTime.</returns>
        public static WorkflowTime HourMarkAt(short hour, short minute)
        {
            return new WorkflowTime(hour, minute);
        }

        /// <summary>
        /// Designates a WorkflowScheduler should execute a Workflow at the given minute.
        /// </summary>
        /// <param name="minute">The minute of the hour the Workflow should be executed.</param>
        /// <returns>A new instance of WorkflowTime.</returns>
        public static WorkflowTime MinuteMarkAt(short minute)
        {
            return new WorkflowTime(minute);
        }

        private WorkflowTime(short day, short hour, short minute) : this(hour, minute)
        {
            Day = day;
        }

        private WorkflowTime(short hour, short minute) : this(minute)
        {
            Hour = hour;
        }

        private WorkflowTime(short minute)
        {
            Minute = minute;
        }
    }
}

