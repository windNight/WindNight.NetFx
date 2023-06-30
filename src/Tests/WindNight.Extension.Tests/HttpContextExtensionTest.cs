using Xunit;
using Xunit.Abstractions;
using IpHelper = WindNight.Extension.HttpContextExtension;


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
            var ips = IpHelper.GetLocalServerIps();
            Output($"GetLocalServerIps()->{string.Join(",", ips)}");

        }

        [Fact]
        public void GetLocalServerIpsTest()
        {
            var ips = IpHelper.GetLocalServerIps();
            Output($"GetLocalServerIps()->{string.Join(",", ips)}");
            ips = IpHelper.GetLocalServerIps();
            Output($"GetLocalServerIps()->{string.Join(",", ips)}");

        }



    }
}
