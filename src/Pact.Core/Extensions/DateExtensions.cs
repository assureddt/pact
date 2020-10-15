using System;
using System.Collections.Generic;
using System.Linq;

namespace Pact.Core.Extensions
{
    public static class DateExtensions
    {
        public static double GetBusinessDays(
            this DateTime start, DateTime end,
            params DateTime [] holidays)
        {
            return GetBusinessDays(start, end, new TimeSpan(0, 0, 0), new TimeSpan(24, 0, 0), holidays).TotalDays;
        }

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
                output = output + new TimeSpan(0, 0, dailyWorkSecondsToAdd);
            }

            return flip ? -output : output;
        }

        public static DateTime Earliest(DateTime a, DateTime b) => a <= b ? a : b;
        public static DateTime Latest(DateTime a, DateTime b) => a >= b ? a : b;
    }
}
