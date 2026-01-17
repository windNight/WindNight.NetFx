using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Extension;
using Xunit;
using Xunit.Abstractions;

namespace WindNight.Core.Tests.Extension
{
    public class StringExtensionTest : TestBase
    {
        public StringExtensionTest(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public void ConcatTest()
        {
            var result = "begin".Concat("1", "2", "3", "4");

            Assert.Equal("begin1234", result);
            Output($"After Concat Is {result}");
        }

        [Theory(DisplayName = "DoubleToInt")]
        [InlineData(1.55, 1)]
        [InlineData(-1.45, -1)]
        [InlineData(10.20, 10)]
        public void DoubleToIntTest(double data, int expectData)
        {
            var rlt = data.ToInt();
            Assert.True(expectData == rlt, $"DoubleToInt({data})  =>{rlt} !=expectData({expectData})");
            Output($"double.ToInt({data})  =>{rlt},expected is {expectData}");
        }



        [Theory(DisplayName = "DecimalToInt")]
        [InlineData(1.55, 1)]
        [InlineData(-1.45, -1)]
        [InlineData(10.20, 10)]
        public void DecimalToIntTest(decimal data, int expectData)
        {
            var rlt = data.ToInt();
            Assert.True(expectData == rlt, $"DecimalToInt({data})  =>{rlt} !=expectData({expectData})");
            Output($"decimal.ToInt({data})  =>{rlt},expected is {expectData}");
        }

        [Theory(DisplayName = "FloatToInt")]
        [InlineData(1.55, 1)]
        [InlineData(-1.45, -1)]
        [InlineData(10.20, 10)]
        public void FloatToIntTest(float data, int expectData)
        {
            var rlt = data.ToInt();
            Assert.True(expectData == rlt, $"FloatToInt({data})  =>{rlt} !=expectData({expectData})");
            Output($"float.ToInt({data})  =>{rlt},expected is {expectData}");
        }



    }
}
