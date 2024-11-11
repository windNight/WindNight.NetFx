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

    }
}
