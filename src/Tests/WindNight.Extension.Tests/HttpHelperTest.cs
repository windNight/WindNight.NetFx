using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Extension;
using Xunit;
using Xunit.Abstractions;

namespace WindNight.Extension.Tests
{
    public class HttpHelperTest : TestBase
    {

        public HttpHelperTest(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public void HttpCheckFileTest()
        {
            //var url = "https://r.bing.com/rp/TrZ6uPwHDjuq464yPHzr6MbATo4.br.js";
            var url = "http://res.engrid.cn/en-firmware-update/dtu/ML302ANLM-downgrade-pac111-100.bin";

            var res = HttpHelper.HttpHead(url);
            var exist = HttpHelper.CheckRemoteFile(url);
            Output(res.ToJsonStr());
            Output($"exist is {exist}");
            if (exist)
            {
                var bytes = HttpHelper.Download(url, checkExist: false);
                Output($"bytes is {bytes.Length}");
            }

        }


    }
}
