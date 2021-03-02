using System;
using Xunit;
using Xunit.Abstractions;

namespace WindNight.Core.Tests
{
    public class DateTimeExtensionsTest : TestBase
    {
        public DateTimeExtensionsTest(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public void LastDayOfWeekTest()
        {
            var nowDate = new DateTime(2020, 11, 17);
            var expectedDate = new DateTime(2020, 11, 22);
            var calcDate = nowDate.LastDayOfWeek();
            Assert.Equal(expectedDate.Date, calcDate.Date);
            Output($"LastDayOfWeek({nowDate})={calcDate},expected is {expectedDate}");
        }

        [Fact]
        public void FirstDayOfWeekTest()
        {
            var nowDate = new DateTime(2020, 11, 17);
            var expectedDate = new DateTime(2020, 11, 16);
            var calcDate = nowDate.FirstDayOfWeek();
            Assert.Equal(expectedDate.Date, calcDate.Date);
            Output($"FirstDayOfWeek({nowDate})={calcDate},expected is {expectedDate}");

        }


        [Fact]
        public void LastDayOfMonthTest()
        {
            var nowDate = new DateTime(2020, 11, 17);
            var expectedDate = new DateTime(2020, 11, 30);
            var calcDate = nowDate.LastDayOfMonth();
            Assert.Equal(expectedDate.Date, calcDate.Date);
            Output($"LastDayOfMonth({nowDate})={calcDate},expected is {expectedDate}");
        }

        [Theory(DisplayName = "ConvertToUnixTimeTest")]
        [InlineData("2021-01-28 14:55:34", 1611816934000L, true)]
        [InlineData("2021-01-28 14:55:34", 1611816934L, false)]
        [InlineData("2021-01-28 15:08:42", 1611817722000L, true)]
        [InlineData("2021-01-28 15:08:42", 1611817722L, false)]
        public void ConvertToUnixTimeTest(string dateTimeStr, long expectedTs, bool milliseconds)
        {
            var dateTime = dateTimeStr.TryToDateTimeSafe();
            Assert.True(dateTime.HasValue, $"input:{dateTimeStr} can't Parse To DateTime");
            var ts = dateTime.Value.ConvertToUnixTime(milliseconds);
            Assert.True(ts == expectedTs, $"input:[{dateTimeStr}:{milliseconds}]->{ts} !=expectedTs({expectedTs})");
            Output($"ConvertToUnixTime(dateTime:{dateTimeStr},milliseconds:{milliseconds})={ts},expected is {expectedTs}");
        }

        [Theory(DisplayName = "ConvertToTimeUseUnixTest")]
        [InlineData(1611816934000L, "2021-01-28 14:55:34", true)]
        [InlineData(1611816934L, "2021-01-28 14:55:34", false)]
        [InlineData(1611817722000L, "2021-01-28 15:08:42", true)]
        [InlineData(1611817722L, "2021-01-28 15:08:42", false)]
        public void ConvertToTimeUseUnixTest(long paramTs, string expectedDateTime, bool milliseconds)
        {
            var dateTime = paramTs.ConvertToTimeUseUnix(milliseconds);
            Assert.True(dateTime.FormatDateTime("yyyy-MM-dd HH:mm:ss") == expectedDateTime, $"input:[{paramTs}:{milliseconds}]->{dateTime:yyyy-MM-dd HH:mm:ss} !=expectedTs({expectedDateTime})");
            Output($"ConvertToTimeUseUnix(unixTime:{paramTs},milliseconds:{milliseconds})={dateTime:yyyy-MM-dd HH:mm:ss},expected is {expectedDateTime}");
        }


    }
}
