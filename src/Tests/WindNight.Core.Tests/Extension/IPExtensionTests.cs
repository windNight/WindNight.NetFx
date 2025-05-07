using WindNight.Core.Extension;
using Xunit;
using Xunit.Abstractions;

namespace WindNight.Core.Tests.Extension
{
    public class IPExtensionTests : TestBase
    {
        public IPExtensionTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }



        [Theory(DisplayName = "IsPrivateOrLoopbackTest")]
        [InlineData("0.0.0.0", true)]
        [InlineData("10.0.0.1", true)]
        [InlineData("172.20.0.1", true)]
        [InlineData("192.168.1.1", true)]
        [InlineData("120.168.1.1", false)]
        public void IsPrivateOrLoopbackTest(string ip, bool expectedF)
        {
            var rlt = ip.IsPrivateOrLoopback();
            Assert.True(rlt == expectedF, $"input:{ip} IsPrivateOrLoopback expectedF ({expectedF})");
            Output($"input:{ip} IsPrivateOrLoopback expectedF ({expectedF}) rlt({rlt})");
        }





    }
}
