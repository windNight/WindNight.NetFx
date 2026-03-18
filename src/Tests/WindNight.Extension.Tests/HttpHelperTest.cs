using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Extension;
using WindNight.Core;
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
            var url = "https://r.bing.com/rp/TrZ6uPwHDjuq464yPHzr6MbATo4.br.js";
            var head = new Dictionary<string, string>
            {

                {"User-Agent","Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36"}
            };
            var res = HttpHelper.HttpHead(url, head);
            var exist = HttpHelper.CheckRemoteFile(url);
            Output(res.ToJsonStr());
            Output($"exist is {exist}");
            if (exist)
            {
                var bytes = HttpHelper.Download(url, checkExist: false);
                Output($"bytes is {bytes.Length}");
            }

        }


        [Fact]
        public void HttpHelperTest1()
        {
            Output($"Version is {HttpHelper.CurrentVersion}");
            Output($"CompileTime is {HttpHelper.CurrentCompileTime}");

        }

        [Fact]
        public void HttpHelperPostUrl1()
        {

            var domain = "https://www.grphtk.cc:8179/ygCtrl";
            var api = "/api/enopt/rtdata/batchsync";
            var res = HttpHelper.Post<int>(domain, api, null, timeOut: 200);

            Output($"res is {res}");

        }

        [Fact]
        public void HttpHelperGetTest()
        {

            var domain = "";
            var api = "/api/monitor/slb";
            var res = HttpHelper.Get<string>(domain, api, null, timeOut: 200);

            Output($"res is {res}");

        }

        [Fact]
        public void HttpHelperGetTest2()
        {

            var domain = "";
            var api = "/api/monitor/svrinfo";
            var headerData = new Dictionary<string, string>
            {

            };
            var res = HttpHelper.Get<object>(domain, api, null, headerData, timeOut: 200);

            Output($"res is {res}");

        }

    }
}
