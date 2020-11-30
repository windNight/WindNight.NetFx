using System;
using Xunit;
using Xunit.Abstractions;

namespace WindNight.Extension.Tests
{
    public class HttpContextExtensionTest : TestBase
    {
        public HttpContextExtensionTest(ITestOutputHelper outputHelper) : base(outputHelper)
        {

        }

        [Fact]
        public void GetLocalIPsTest()
        {
            var ips = HttpContextExtension.GetLocalServerIps();
            Output($"GetLocalServerIps()->{string.Join(",", ips)}");

        }

        [Fact]
        public void GetLocalServerIpsTest()
        {
            var ips = HttpContextExtension.GetLocalServerIps();
            Output($"GetLocalServerIps()->{string.Join(",", ips)}");
            ips = HttpContextExtension.GetLocalServerIps();
            Output($"GetLocalServerIps()->{string.Join(",", ips)}");

        }



    }
}
