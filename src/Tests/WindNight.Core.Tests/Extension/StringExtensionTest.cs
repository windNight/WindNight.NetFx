using System;
using System.Linq;
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

    }
}
