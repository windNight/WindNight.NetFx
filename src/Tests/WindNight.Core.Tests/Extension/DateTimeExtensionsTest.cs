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



    }
}
