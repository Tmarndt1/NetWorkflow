using NetWorkflow.Exceptions;
using System;

namespace NetWorkflow.Scheduler
{
    /// <summary>
    /// WorflowTime represents when a WorkflowScheduler should execute a Workflow
    /// </summary>
    /// <remarks>
    /// Days span from 1 to 31.
    /// Hours are in military time so they span from 0 to 23.
    /// Minutes span from 0 to 59.
    /// </remarks>
    public abstract class WorkflowTime
    {
        /// <summary>
        /// Designates the WorkflowScheduler to execute the Workflow at the given frequency
        /// </summary>
        /// <param name="frequency">The frequency to execute the Workflow</param>
        /// <returns>An instance of WorkflowFrequency</returns>
        public static WorkflowFrequency AtFrequency(TimeSpan frequency)
        {
            return new WorkflowFrequency(frequency);
        }

        /// <summary>
        /// Designates a WorkflowScheduler should execute a Workflow on the given day, hour, and minute.
        /// </summary>
        /// <param name="day">The day of the month the Workflow should be executed.</param>
        /// <param name="hour">The hour of the day the Workflow should be executed.</param>
        /// <param name="minute">The minute of the hour the Workflow should be executed.</param>
        /// <returns>A new instance of WorkflowTime.</returns>
        public static WorkflowDateTime AtDay(int day, int hour, int minute)
        {
            return new WorkflowDateTime(day, hour, minute);
        }

        /// <summary>
        /// Designates a WorkflowScheduler should execute a Workflow on the given day, hour, and minute 0.
        /// </summary>
        /// <param name="day">The day of the month the Workflow should be executed.</param>
        /// <param name="hour">The hour of the day the Workflow should be executed.</param>
        /// <returns>A new instance of WorkflowTime.</returns>
        /// <remarks>
        /// Will execute at the beginning of the provided day and hour.
        /// </remarks>
        public static WorkflowDateTime AtDay(int day, int hour)
        {
            return new WorkflowDateTime(day, hour, 0);
        }

        /// <summary>
        /// Designates a WorkflowScheduler should execute a Workflow on the given day, hour 0, and minute 0.
        /// </summary>
        /// <param name="day">The day of the month the Workflow should be executed.</param>
        /// <returns>A new instance of WorkflowTime.</returns>
        /// <remarks>
        /// Will execute at midnight on the given day.
        /// </remarks>
        public static WorkflowDateTime AtDay(int day)
        {
            return new WorkflowDateTime(day, 0, 0);
        }

        /// Designates a WorkflowScheduler should execute a Workflow at the given
        /// hour and at the given minute. 
        /// </summary>
        /// <param name="hour">The hour of the day the Workflow should be executed.</param>
        /// <param name="minute">The minute of the hour the Workflow should be executed.</param>
        /// <returns>A new instance of WorkflowTime.</returns>
        public static WorkflowDateTime AtHour(int hour, int minute)
        {
            return new WorkflowDateTime(hour, minute);
        }

        /// Designates a WorkflowScheduler should execute a Workflow at the given hour and minute 0.
        /// </summary>
        /// <param name="hour">The hour of the day the Workflow should be executed.</param>
        /// <returns>A new instance of WorkflowTime.</returns>
        public static WorkflowDateTime AtHour(int hour)
        {
            return new WorkflowDateTime(hour, 0);
        }

        /// <summary>
        /// Designates a WorkflowScheduler should execute a Workflow at the given minute.
        /// </summary>
        /// <param name="minute">The minute of the hour the Workflow should be executed.</param>
        /// <returns>A new instance of WorkflowTime.</returns>
        public static WorkflowDateTime AtMinute(int minute)
        {
            return new WorkflowDateTime(minute);
        }

        /// <summary>
        /// Designates the Workflow to execute until the count is met.
        /// </summary>
        internal int ExecutionCount { get; private set; } = -1;

        /// <summary>
        /// Designates the Workflow to execute until the count is met.
        /// </summary>
        /// <param name="count">The max ammount of times the Workflow should execute.</param>
        /// <returns>The same instance of the WorkflowTime.</returns>
        public WorkflowTime Until(int count)
        {
            ExecutionCount = count;

            return this;
        }

    }

    public class WorkflowDateTime : WorkflowTime
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
                if (value < 0) throw new WorkflowInvalidValueException("A hour cannot be less than 0");

                if (value > 23) throw new WorkflowInvalidValueException("A hour cannot be greater than 23");

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

        internal WorkflowDateTime(int day, int hour, int minute) : this(hour, minute)
        {
            Day = day;
        }

        internal WorkflowDateTime(int hour, int minute) : this(minute)
        {
            Hour = hour;
        }

        internal WorkflowDateTime(int minute)
        {
            Minute = minute;
        }
    }

    public class WorkflowFrequency : WorkflowTime
    {
        /// <summary>
        /// Determines the frequency of how often the WorkflowScheduler executes a Workflow
        /// </summary>
        public readonly TimeSpan Frequency;

        internal WorkflowFrequency(TimeSpan frequency)
        {
            Frequency = frequency;
        }
    }
}

