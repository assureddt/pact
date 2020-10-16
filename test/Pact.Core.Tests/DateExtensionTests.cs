using System;
using Pact.Core.Extensions;
using Shouldly;
using Xunit;

namespace Pact.Core.Tests
{
    public class DateExtensionTests
    {
        private readonly DateTime _fridayMorning = new DateTime(2020, 10, 16, 08, 15, 30);
        private readonly DateTime _lastWednesdayEvening = new DateTime(2020, 10, 14, 20, 15, 30);
        private readonly DateTime _nextWednesdayEvening = new DateTime(2020, 10, 21, 20, 15, 30);
        private readonly DateTime _pretendHoliday = new DateTime(2020, 10, 19);

        [Fact]
        public void BusinessDays_Basic_OK()
        {
            _fridayMorning.GetBusinessDays(_nextWednesdayEvening).ShouldBe(3.5);
        }

        [Fact]
        public void BusinessDays_WithHoliday_OK()
        {
            _fridayMorning.GetBusinessDays(_nextWednesdayEvening, _pretendHoliday).ShouldBe(2.5);
        }

        [Fact]
        public void BusinessDays_Reverse_OK()
        {
            _fridayMorning.GetBusinessDays(_lastWednesdayEvening).ShouldBe(-1.5);
        }

        [Fact]
        public void BusinessDays_Working_Hours_OK()
        {
            var workStart = new TimeSpan(8, 30, 0);
            var workEnd = new TimeSpan(17, 30, 0);
            var workingHours = workEnd.Subtract(workStart).TotalHours;

            // the range falls just before the start of friday and the end of wednesday, so is 4 full working days (measured in working hours, not full calendar days)
            _fridayMorning.GetBusinessDays(_nextWednesdayEvening, workStart, workEnd)
                .ShouldBe(TimeSpan.FromHours(workingHours * 4));
        }

        [Fact]
        public void Earliest_OK()
        {
            DateExtensions.Earliest(_fridayMorning, _nextWednesdayEvening).ShouldBe(_fridayMorning);
        }

        [Fact]
        public void Latest_OK()
        {
            DateExtensions.Latest(_lastWednesdayEvening, _nextWednesdayEvening).ShouldBe(_nextWednesdayEvening);
        }
    }
}
