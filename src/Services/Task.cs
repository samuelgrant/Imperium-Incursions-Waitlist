using System;


namespace Imperium_Incursions_Waitlist
{
    public static class Task
    {
        /// <summary>
        /// Schedule a task that occurs every X seconds.
        /// </summary>
        /// <param name="hour">start hour</param>
        /// <param name="sec">start second</param>
        /// <param name="interval">occur every X seconds</param>
        public static void IntervalInSeconds(int hour, int sec, double interval, Action task)
        {
            interval = interval / 3600;
            SchedulerService.Instance.ScheduleTask(hour, sec, interval, task);
        }

        /// <summary>
        /// Schedule a task that occurs every X minutes.
        /// </summary>
        /// <param name="hour">start hour</param>
        /// <param name="min">start minute</param>
        /// <param name="interval">occur every X minutes</param>
        public static void IntervalInMinutes(int hour, int min, double interval, Action task)
        {
            interval = interval / 60;
            SchedulerService.Instance.ScheduleTask(hour, min, interval, task);
        }

        /// <summary>
        /// Schedule a task that occurs every X hours.
        /// </summary>
        /// <param name="hour">start hour</param>
        /// <param name="min">start minute</param>
        /// <param name="interval">occur every X hours</param>
        public static void IntervalInHours(int hour, int min, double interval, Action task)
        {
            SchedulerService.Instance.ScheduleTask(hour, min, interval, task);
        }

        /// <summary>
        /// Schedule a task that occurs every X days.
        /// </summary>
        /// <param name="hour">start hour</param>
        /// <param name="minute">start minute</param>
        /// <param name="interval">occur every X days</param>
        public static void IntervalInDays(int hour, int minute, double interval, Action task)
        {
            interval = interval * 24;
            SchedulerService.Instance.ScheduleTask(hour, minute, interval, task);
        }
    }
}
