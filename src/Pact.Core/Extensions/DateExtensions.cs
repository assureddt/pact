using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace Pact.Core.Extensions
{
    public static class DateExtensions
    {
        /// <summary>
        /// Returns the precise number of working days between 2 date-times (assuming Monday-Friday working week)
        /// </summary>
        /// <param name="start">Start date</param>
        /// <param name="end">End date</param>
        /// <param name="holidays">Optional collection of holiday dates to add to the exclusion</param>
        /// <returns>The decimal working days difference between the date-times</returns>
        public static double GetBusinessDays(
            this DateTime start, DateTime end,
            params DateTime [] holidays)
        {
            return GetBusinessDays(start, end, new TimeSpan(0, 0, 0), new TimeSpan(24, 0, 0), holidays).TotalDays;
        }

        /// <summary>
        /// Returns the precise number of working days between 2 date-times (assuming Monday-Friday working week), factoring in actual working hours
        /// </summary>
        /// <param name="start">Start date</param>
        /// <param name="end">End date</param>
        /// <param name="workdayStartTime">The time the working day starts</param>
        /// <param name="workdayEndTime">The time the working day ends</param>
        /// <param name="holidays">Optional collection of holiday dates to add to the exclusion</param>
        /// <returns>The timespan difference between the date-times</returns>
        public static TimeSpan GetBusinessDays(
            this DateTime start, DateTime end,
            TimeSpan workdayStartTime, TimeSpan workdayEndTime,
            params DateTime [] holidays)
        {
            var flip = false;
            if (end < start)
            {
                flip = true;
                var temp = start;
                start = end;
                end = temp;
            }

            // Just create an empty list for easier coding.
            holidays ??= Array.Empty<DateTime>();

            if (holidays.Any(x => x.TimeOfDay.Ticks > 0))
                throw new ArgumentException("holidays can not have a TimeOfDay, only the Date.");

            var nonWorkDays = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday };

            var startTime = start.TimeOfDay;

            // If the start time is before the starting hours, set it to the starting hour.
            if (startTime < workdayStartTime) startTime = workdayStartTime;

            var timeBeforeEndOfWorkDay = workdayEndTime - startTime;

            // If it's after the end of the day, then this time lapse doesn't count.
            if (timeBeforeEndOfWorkDay.TotalSeconds < 0) timeBeforeEndOfWorkDay = new TimeSpan();
            // If start is during a non work day, it doesn't count.
            if (nonWorkDays.Contains(start.DayOfWeek)) timeBeforeEndOfWorkDay = new TimeSpan();
            else if (holidays.Contains(start.Date)) timeBeforeEndOfWorkDay = new TimeSpan();

            var endTime = end.TimeOfDay;

            // If the end time is after the ending hours, set it to the ending hour.
            if (endTime > workdayEndTime) endTime = workdayEndTime;

            var timeAfterStartOfWorkDay = endTime - workdayStartTime;

            // If it's before the start of the day, then this time lapse doesn't count.
            if (timeAfterStartOfWorkDay.TotalSeconds < 0) timeAfterStartOfWorkDay = new TimeSpan();
            // If end is during a non work day, it doesn't count.
            if (nonWorkDays.Contains(end.DayOfWeek)) timeAfterStartOfWorkDay = new TimeSpan();
            else if (holidays.Contains(end.Date)) timeAfterStartOfWorkDay = new TimeSpan();

            TimeSpan output;

            // Easy scenario if the times are during the day day.
            if (start.Date.CompareTo(end.Date) == 0)
            {
                if (nonWorkDays.Contains(start.DayOfWeek)) return new TimeSpan();
                if (holidays.Contains(start.Date)) return new TimeSpan();
                
                output = endTime - startTime;
            }
            else
            {
                var timeBetween = end - start;
                var daysBetween = (int)Math.Floor(timeBetween.TotalDays);
                var dailyWorkSeconds = (int)Math.Floor((workdayEndTime - workdayStartTime).TotalSeconds);

                var businessDaysBetween = 0;

                // Now the fun begins with calculating the actual Business days.
                if (daysBetween > 0)
                {
                    var nextStartDay = start.AddDays(1).Date;
                    var dayBeforeEnd = end.AddDays(-1).Date;
                    for (var d = nextStartDay; d <= dayBeforeEnd; d = d.AddDays(1))
                    {
                        if (nonWorkDays.Contains(d.DayOfWeek)) continue;
                        if (holidays.Contains(d.Date)) continue;
                        businessDaysBetween++;
                    }
                }

                var dailyWorkSecondsToAdd = dailyWorkSeconds * businessDaysBetween;

                output = timeBeforeEndOfWorkDay + timeAfterStartOfWorkDay;
                output += new TimeSpan(0, 0, dailyWorkSecondsToAdd);
            }

            return flip ? -output : output;
        }

        /// <summary>
        /// Returns the earliest of 2 dates
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>Whichever is the earliest</returns>
        public static DateTime Earliest(params DateTime[] dt) => dt.Min();

        /// <summary>
        /// Returns the latest of 2 dates
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>Whichever is the latest</returns>
        public static DateTime Latest(params DateTime[] dt) => dt.Max();

        /// <summary>
        /// If the value is within the valid SQL Server range for dates, it is returned unchanged, otherwise returns null
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime? NullifySqlDateTimeIfInvalid(this DateTime dateTime) => IsValidSqlDateTime(dateTime) ? dateTime : (DateTime?)null;

        /// <summary>
        /// Checks the provided value for SQL Server date range validity
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool IsValidSqlDateTime(this DateTime dateTime) => dateTime >= (DateTime)SqlDateTime.MinValue && dateTime <= (DateTime)SqlDateTime.MaxValue;
    }
}
