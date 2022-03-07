using WindNight.Core.Extension;
using Xunit;
using Xunit.Abstractions;

namespace WindNight.Core.Tests.Extension
{
    public class ValueTypeExtensionTest : TestBase
    {
        public ValueTypeExtensionTest(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Theory(DisplayName = "DecimalCeilingTest")]
        [InlineData(1.55, 2)]
        [InlineData(-1.45, -1)]
        [InlineData(10.2, 11)]
        public void DecimalCeilingTest(decimal data, int expectData)
        {
            var rlt = data.Ceiling();
            Assert.True(expectData == rlt, $"Ceiling({data})  =>{rlt} !=expectData({expectData})");
            Output($"decimal.Ceiling({data})  =>{rlt},expected is {expectData}");
        }

        [Theory(DisplayName = "DecimalRoundTest")]
        [InlineData(1.55, 2)]
        [InlineData(-1.45, -1)]
        [InlineData(10.2, 10)]
        public void DecimalRoundTest(decimal data, int expectData)
        {
            var rlt = data.Round(0);
            Assert.True(expectData == rlt, $"Round({data})  =>{rlt} !=expectData({expectData})");
            Output($"decimal.Round({data})  =>{rlt},expected is {expectData}");

        }

        [Theory(DisplayName = "DecimalFloorTest")]
        [InlineData(1.55, 1)]
        [InlineData(-1.45, -2)]
        [InlineData(10.2, 10)]
        public void DecimalFloorTest(decimal data, int expectData)
        {
            var rlt = data.Floor();
            Assert.True(expectData == rlt, $"Floor({data})  =>{rlt} !=expectData({expectData})");
            Output($"decimal.Floor({data})  =>{rlt},expected is {expectData}");

        }


        [Theory(DisplayName = "DecimalTruncateTest")]
        [InlineData(1.55, 1)]
        [InlineData(-1.45, -1)]
        [InlineData(10.2, 10)]
        public void DecimalTruncateTest(decimal data, int expectData)
        {
            var rlt = data.Truncate();
            Assert.True(expectData == rlt, $"Truncate({data})  =>{rlt} !=expectData({expectData})");
            Output($"decimal.Truncate({data})  =>{rlt},expected is {expectData}");

        }



        [Theory(DisplayName = "DoubleCeilingTest")]
        [InlineData(1.55, 2)]
        [InlineData(-1.45, -1)]
        [InlineData(10.2, 11)]
        public void DoubleCeilingTest(double data, int expectData)
        {
            var rlt = data.Ceiling();
            Assert.True(expectData == rlt, $"Ceiling({data})  =>{rlt} !=expectData({expectData})");
            Output($"double.Ceiling({data})  =>{rlt},expected is {expectData}");

        }

        [Theory(DisplayName = "DoubleRoundTest")]
        [InlineData(1.55, 2)]
        [InlineData(-1.45, -1)]
        [InlineData(10.2, 10)]
        public void DoubleRoundTest(double data, int expectData)
        {
            var rlt = data.Round(0);
            Assert.True(expectData == rlt, $"Round({data})  =>{rlt} !=expectData({expectData})");
            Output($"double.Round({data})  =>{rlt},expected is {expectData}");

        }

        [Theory(DisplayName = "DoubleFloorTest")]
        [InlineData(1.55, 1)]
        [InlineData(-1.45, -2)]
        [InlineData(10.2, 10)]
        public void DoubleFloorTest(double data, int expectData)
        {
            var rlt = data.Floor();
            Assert.True(expectData == rlt, $"Floor({data})  =>{rlt} !=expectData({expectData})");
            Output($"double.Floor({data})  =>{rlt},expected is {expectData}");
        }

        [Theory(DisplayName = "DoubleTruncateTest")]
        [InlineData(1.55, 1)]
        [InlineData(-1.45, -1)]
        [InlineData(10.2, 10)]
        public void DoubleTruncateTest(double data, int expectData)
        {
            var rlt = data.Truncate();
            Assert.True(expectData == rlt, $"Truncate({data})  =>{rlt} !=expectData({expectData})");
            Output($"double.Truncate({data})  =>{rlt},expected is {expectData}");
        }



    }
}
