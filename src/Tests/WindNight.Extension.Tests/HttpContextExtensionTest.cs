using Newtonsoft.Json;
using Newtonsoft.Json.Extension;
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

        [Fact]
        public void GetPagedListTest()
        {
            var res = HttpHelper.GetPagedList<DtuWorkInfoDto>("http://szdtuv2.engrid.org/api/dtu/packets/read?dtuId=21529025");

            Output($"GetPagedList()->{res.ToJsonStr()}");
            Assert.True(res.List.Count > 0, $"GetPagedList List is Null");

        }



    }


    public class DtuWorkInfoDto
    {
        [JsonProperty("id")]
        public string DtuId { get; set; } = "";
        /// <summary> FrameName |ATCmd  </summary>
        [JsonProperty("name")]
        public string ActionName { get; set; } = "";
        /// <summary> HexString | ATCmdString</summary>
        [JsonProperty("sendHex")]
        public string SendDataHex { get; set; } = "";
        [JsonProperty("sendStr")]
        public string SendDataString { get; set; } = "";
        /// <summary> HexString | ATCmdString</summary>
        [JsonProperty("recHex")]
        public string ReceiveHexSting { get; set; } = "";

        [JsonProperty("recStr")]
        public string ReceiveString { get; set; } = "";

        /// <summary>  通讯耗时 </summary>
        [JsonProperty("ttl")]
        public long Ttl { get; set; }

        /// <summary> 13位时间戳 以发的时间为准  </summary>
        [JsonProperty("ts")]
        public long ActionTs { get; set; }

        /// <summary> <see cref="EnDtuActionStatusEnum"/> </summary>
        [JsonProperty("status")]
        public int ActionStatus { get; set; }
        [JsonProperty("msg")]
        public string ErrorMessage { get; set; } = "";


        public override string ToString()
        {
            return this.ToJsonStr();
        }
    }

}
