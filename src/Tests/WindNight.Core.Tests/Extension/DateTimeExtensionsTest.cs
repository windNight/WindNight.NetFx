using System;
using System.Linq;
using Newtonsoft.Json.Extension;
using Xunit;
using Xunit.Abstractions;

namespace WindNight.Core.Tests
{
    public class DateTimeExtensionsTest : TestBase
    {
        public DateTimeExtensionsTest(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Theory(DisplayName = "LastDayOfWeekTest")]
        [InlineData("2021-11-01", "2021-11-07")]
        [InlineData("2022-01-01", "2022-01-02")]
        [InlineData("2022-02-01", "2022-02-06")]
        public void LastDayOfWeekTest(DateTime testDate, DateTime expectLastDayOfWeek)
        {
            var lastDay = testDate.LastDayOfWeek();
            Assert.True(expectLastDayOfWeek == lastDay, $"LastDayOfWeek({testDate:yyyMMdd})  =>{lastDay:yyyyMMdd} !=expectLastDayOfWeek({expectLastDayOfWeek:yyyyMMdd})");
            Output($"LastDayOfWeek({testDate:yyyMMdd})={lastDay:yyyMMdd},expected is {expectLastDayOfWeek:yyyMMdd}");

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


        [Theory(DisplayName = "WeekOfYearTest")]
        [InlineData("2021-11-01", 45)]
        [InlineData("2021-11-07", 45)]
        [InlineData("2022-02-07", 7)]
        [InlineData("2022-02-13", 7)]
        [InlineData("2022-02-14", 8)]
        [InlineData("2022-02-20", 8)]
        [InlineData("2024-01-31", 5)]
        [InlineData("2024-02-01", 5)]
        public void WeekOfYearTest(DateTime toTestDate, int expectWeek)
        {
            var rlt = toTestDate.WeekOfYear();
            Assert.True(expectWeek == rlt, $"WeekOfYear({toTestDate:yyyyMMdd}) is {rlt} !=expectWeek({expectWeek})");
            Output($"WeekOfYear({toTestDate:yyyyMMdd})=>(week:{rlt})");

        }


        [Theory(DisplayName = "CalcDateRangeByWeekTest")]
        [InlineData(45, 2021, "2021-11-01", "2021-11-07")]
        [InlineData(7, 2022, "2022-02-07", "2022-02-13")]
        [InlineData(8, 2022, "2022-02-14", "2022-02-20")]
        public void CalcDateRangeByWeekTest(int week, int year, DateTime expectBeginDate, DateTime expectEndDate)
        {

            var range = HardInfo.Now.CalcDateRangeByWeek(week, year);
            Assert.True(expectBeginDate == range.beginDate, $"CalcDateRangeByWeek({week},{year}).beginDate({range.beginDate:yyyyMMdd}) !=expectBeginDate({expectBeginDate:yyyyMMdd})");
            Assert.True(expectBeginDate == range.beginDate, $"CalcDateRangeByWeek({week},{year}).beginDate({range.endDate:yyyyMMdd}) !=expectEndDate({expectEndDate:yyyyMMdd})");
            Output($"CalcDateRangeByWeek({week},{year})=>(beginDate:{range.beginDate:yyyyMMdd},endDate:{range.endDate:yyyyMMdd})");
        }

        [Theory(DisplayName = "GeneratorDateIntListTest")]
        [InlineData("2021-11-01")]
        [InlineData("2022-01-01")]
        [InlineData("2022-02-01")]
        public void GeneratorDateIntListTest(DateTime beginDate)
        {
            var expectBeginDateInt = beginDate.ToDateInt();
            var expectEndDateInt = HardInfo.Now.ToDateInt();
            var list = beginDate.GeneratorDateIntList(withLastDay: true);
            var rltB = list.FirstOrDefault();
            var rltE = list.LastOrDefault();

            Assert.True(expectBeginDateInt == rltB, $"GeneratorDateIntList({beginDate:yyyyMMdd}).FirstOrDefault =>{rltB} !=expectBeginDateInt({expectBeginDateInt})");
            Assert.True(expectEndDateInt == rltE, $"GeneratorDateIntList({beginDate:yyyyMMdd}).LastOrDefault =>{rltE} !=expectBeginDateInt({expectEndDateInt})");

        }

        [Theory(DisplayName = "GeneratorDateIntListV2Test")]
        [InlineData(20211101)]
        [InlineData(20220101)]
        [InlineData(20220201)]
        public void GeneratorDateIntListV2Test(int beginDateInt)
        {
            var expectEndDateInt = HardInfo.Now.ToDateInt();
            var list = beginDateInt.GeneratorDateIntList(withLastDay: true);
            var rltB = list.FirstOrDefault();
            var rltE = list.LastOrDefault();

            Assert.True(beginDateInt == rltB, $"GeneratorDateIntList({beginDateInt}).FirstOrDefault =>{rltB} !=expectBeginDateInt({beginDateInt})");
            Assert.True(expectEndDateInt == rltE, $"GeneratorDateIntList({beginDateInt}).LastOrDefault =>{rltE} !=expectBeginDateInt({expectEndDateInt})");
        }

        [Theory(DisplayName = "GeneratorDateMonthListTest")]
        [InlineData("2023-01-01", "2023-12-01", 12, 202301, 202312)]
        [InlineData("2023-01-01", "2024-12-01", 24, 202301, 202412)]
        public void GeneratorDateMonthListTest(DateTime beginDate, DateTime endDate, int expectCount, int expectBeginMonth, int expectEndMonth)
        {
            var execList = beginDate.GeneratorDateMonthList(endDate, withLastDay: true);
            var execCount = execList.Count();
            var rltB = execList.FirstOrDefault();
            var rltE = execList.LastOrDefault();
            Output($"GeneratorDateMonthList({beginDate:yyyyMMdd},{endDate:yyyyMMdd})=>(execList:{execList.ToJsonStr()})");

            Assert.True(execCount == expectCount, $"GeneratorDateMonthList({beginDate},{endDate}).Count =>{execCount} !=expectCount({expectCount})");
            Assert.True(expectBeginMonth == rltB, $"GeneratorDateMonthList({beginDate},{endDate}).FirstOrDefault =>{rltB} !=expectBeginMonth({expectBeginMonth})");
            Assert.True(expectEndMonth == rltE, $"GeneratorDateMonthList({beginDate},{endDate}).LastOrDefault =>{rltE} !=expectEndMonth({expectEndMonth})");



        }
        [Theory(DisplayName = "GeneratorDateWeekListTest")]
        [InlineData("2023-01-01", "2023-12-31", 53, 202301, 202353)]
        [InlineData("2023-01-01", "2024-01-31", 58, 202301, 202405)]
        [InlineData("2023-01-01", "2025-01-31", 111, 202301, 202505)]// 53+53+5
        public void GeneratorDateWeekListTest(DateTime beginDate, DateTime endDate, int expectCount, int expectBeginYearWeek, int expectEndYearWeek)
        {
            var execList = beginDate.GeneratorDateWeekList(endDate);
            var execCount = execList.Count();
            var rltB = execList.FirstOrDefault();
            var rltE = execList.LastOrDefault();
            Output($"GeneratorDateWeekList({beginDate:yyyyMMdd},{endDate:yyyyMMdd})=>(execList:{execList.ToJsonStr()})");

            Assert.True(execCount == expectCount, $"GeneratorDateWeekList({beginDate},{endDate}).Count =>{execCount} !=expectCount({expectCount})");
            Assert.True(expectBeginYearWeek == rltB, $"GeneratorDateWeekList({beginDate},{endDate}).FirstOrDefault =>{rltB} !=expectBeginYearWeek({expectBeginYearWeek})");
            Assert.True(expectEndYearWeek == rltE, $"GeneratorDateWeekList({beginDate},{endDate}).LastOrDefault =>{rltE} !=expectEndYearWeek({expectEndYearWeek})");



        }


    }
}
