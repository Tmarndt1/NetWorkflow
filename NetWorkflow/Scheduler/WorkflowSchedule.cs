using System;
using NetWorkflow.Exceptions;

namespace NetWorkflow.Scheduler
{
    /// <summary>
    /// WorkflowSchedule represents when a WorkflowScheduler should execute a Workflow.
    /// </summary>
    /// <remarks>
    /// Days span from 1 to 31.
    /// Hours are in military time so they span from 0 to 23.
    /// Minutes span from 0 to 59.
    /// </remarks>
    public abstract class WorkflowSchedule
    {
        /// <summary>
        /// Designates the WorkflowScheduler to execute the Workflow at the given frequency.
        /// </summary>
        /// <param name="frequency">The frequency to execute the Workflow.</param>
        /// <returns>An instance of FrequencySchedule.</returns>
        public static FrequencySchedule AtFrequency(TimeSpan frequency)
        {
            if (frequency <= TimeSpan.Zero)
            {
                throw new WorkflowInvalidValueException("Frequency must be greater than zero.");
            }

            return new FrequencySchedule(frequency);
        }

        /// <summary>
        /// Designates a WorkflowScheduler should execute a Workflow at the given day, hour, and minute.
        /// </summary>
        /// <param name="day">The day of the month the Workflow should be executed.</param>
        /// <param name="hour">The hour of the day the Workflow should be executed.</param>
        /// <param name="minute">The minute of the hour the Workflow should be executed.</param>
        /// <returns>A new instance of CalendarSchedule.</returns>
        public static CalendarSchedule AtDay(int day, int hour, int minute)
        {
            return new CalendarSchedule(day, hour, minute);
        }

        /// <summary>
        /// Designates a WorkflowScheduler should execute a Workflow at the given day, hour, and minute 0.
        /// </summary>
        /// <param name="day">The day of the month the Workflow should be executed.</param>
        /// <param name="hour">The hour of the day the Workflow should be executed.</param>
        /// <returns>A new instance of CalendarSchedule.</returns>
        public static CalendarSchedule AtDay(int day, int hour)
        {
            return new CalendarSchedule(day, hour, 0);
        }

        /// <summary>
        /// Designates a WorkflowScheduler should execute a Workflow on the given day, hour 0, and minute 0.
        /// </summary>
        /// <param name="day">The day of the month the Workflow should be executed.</param>
        /// <returns>A new instance of CalendarSchedule.</returns>
        public static CalendarSchedule AtDay(int day)
        {
            return new CalendarSchedule(day, 0, 0);
        }

        /// <summary>
        /// Designates a WorkflowScheduler should execute a Workflow at the given hour and minute.
        /// </summary>
        /// <param name="hour">The hour of the day the Workflow should be executed.</param>
        /// <param name="minute">The minute of the hour the Workflow should be executed.</param>
        /// <returns>A new instance of CalendarSchedule.</returns>
        public static CalendarSchedule AtHour(int hour, int minute)
        {
            return new CalendarSchedule(hour, minute);
        }

        /// <summary>
        /// Designates a WorkflowScheduler should execute a Workflow at the given hour and minute 0.
        /// </summary>
        /// <param name="hour">The hour of the day the Workflow should be executed.</param>
        /// <returns>A new instance of CalendarSchedule.</returns>
        public static CalendarSchedule AtHour(int hour)
        {
            return new CalendarSchedule(hour, 0);
        }

        /// <summary>
        /// Designates a WorkflowScheduler should execute a Workflow at the given minute.
        /// </summary>
        /// <param name="minute">The minute of the hour the Workflow should be executed.</param>
        /// <returns>A new instance of CalendarSchedule.</returns>
        public static CalendarSchedule AtMinute(int minute)
        {
            return new CalendarSchedule(minute);
        }

        /// <summary>
        /// Designates the Workflow to execute until the count is met.
        /// </summary>
        /// <param name="count">The max amount of times the Workflow should execute.</param>
        /// <returns>The same instance of the WorkflowSchedule.</returns>
        public WorkflowSchedule Until(int count)
        {
            if (count < 1)
            {
                throw new WorkflowInvalidValueException("Execution count must be greater than zero.");
            }

            ExecutionCount = count;
            return this;
        }

        /// <summary>
        /// Designates the Workflow to execute until the count is met.
        /// </summary>
        internal int ExecutionCount { get; private set; } = -1;
    }

    public class CalendarSchedule : WorkflowSchedule
    {
        /// <summary>
        /// The day of the month a Workflow should be executed.
        /// </summary>
        public int Day { get; private set; } = -1;

        /// <summary>
        /// The hour of the day a Workflow should be executed.
        /// </summary>
        public int Hour { get; private set; } = -1;

        /// <summary>
        /// The minute of the hour a Workflow should be executed.
        /// </summary>
        public int Minute { get; private set; }

        internal CalendarSchedule(int day, int hour, int minute)
        {
            Day = ValidateDay(day);
            Hour = ValidateHour(hour);
            Minute = ValidateMinute(minute);
        }

        internal CalendarSchedule(int hour, int minute)
        {
            Hour = ValidateHour(hour);
            Minute = ValidateMinute(minute);
        }

        internal CalendarSchedule(int minute)
        {
            Minute = ValidateMinute(minute);
        }

        private static int ValidateDay(int day)
        {
            if (day < 1 || day > 31)
            {
                throw new WorkflowInvalidValueException("Day must be between 1 and 31.");
            }

            return day;
        }

        private static int ValidateHour(int hour)
        {
            if (hour < 0 || hour > 23)
            {
                throw new WorkflowInvalidValueException("Hour must be between 0 and 23.");
            }

            return hour;
        }

        private static int ValidateMinute(int minute)
        {
            if (minute < 0 || minute > 59)
            {
                throw new WorkflowInvalidValueException("Minute must be between 0 and 59.");
            }

            return minute;
        }

        internal bool IsNow(DateTimeOffset now)
        {
            if (Day != -1)
            {
                return now.Day == Day && now.Hour == Hour && now.Minute == Minute;
            }

            if (Hour != -1)
            {
                return now.Hour == Hour && now.Minute == Minute;
            }

            return now.Minute == Minute;
        }
    }

    public class FrequencySchedule : WorkflowSchedule
    {
        /// <summary>
        /// Determines the frequency of how often the WorkflowScheduler executes a Workflow.
        /// </summary>
        public TimeSpan Frequency { get; }

        internal FrequencySchedule(TimeSpan frequency)
        {
            Frequency = frequency;
        }
    }
}
