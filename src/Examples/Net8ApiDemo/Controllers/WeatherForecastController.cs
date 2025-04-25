using System.Security.Cryptography;
using System.Security.Cryptography.Extensions;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Newtonsoft.Json.Extension;
using WindNight.Core.Attributes.Abstractions;
using WindNight.Extension;
using WindNight.LogExtension;

namespace Net8ApiDemo.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }



        private readonly Dictionary<string, string> SignDict = new()
        {
            {"Authorization","格式 Bearer xx"},  { "AppId", "AppId" }, { "AppCode", "AppCode" }, { "AppToken", "Sign" }, { "Ts", "当前时间戳" },
        };



        private string GetHeaderData(HttpRequest httpRequest, string headerName)
        {
            if (httpRequest.Headers.TryGetValue(headerName, out var requestHeader))
            {
                var header = requestHeader.FirstOrDefault();
                if (!header.IsNullOrEmpty())
                {
                    return header.Trim();
                }
            }

            return string.Empty;
        }

        Dictionary<string, string> GetAllHeaderData(HttpRequest httpRequest)
        {
            var dict = new Dictionary<string, string>();
            foreach (var item in httpRequest.Headers)
            {
                dict.Add(item.Key, item.Value);
            }

            return dict;
        }

        [HttpGet("dingtalk")]
        [NonAuth, DebugApi]
        public async Task<object> TestDingTalkNoticeAsync([FromQuery] TIn req = null)
        {
            var content = GetNoticeContent("测试内容测试内容测试内容");

            var title = "调度任务通知";
            var obj = new
            {
                msgtype = "markdown",
                markdown = new
                {
                    title,
                    text = content,
                },
                at = new
                {

                },
            };
            var token = "";
            var signKey = "";
            var ts = HardInfo.NowUnixTime;
            var sign = EncryptHelper.CalcDingTalkSign(ts, signKey);
            //var sign1 = EncryptHelper.CalcDingTalkSign1(ts, signKey);
            var requestUri = $"https://oapi.dingtalk.com/robot/send?access_token={token}&timestamp={ts}&sign={sign}";
            //var rlt = await HttpPostAsync(requestUri, obj);

            var rlt = await HttpHelper.PostAsync<string>(requestUri, obj, convertFunc: _ => _.ToString());


            return rlt;

        }

        async Task<string> HttpPostAsync(string url, object bodyObj)
        {
            // var requestUri = $"https://oapi.dingtalk.com/robot/send?access_token={token}";
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(bodyObj.ToJsonStr(), Encoding.UTF8, "application/json"),
            };
            var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("content-type", "application/json;charset=UTF-8");

            var res = await httpClient.SendAsync(request);
            var rlt = await res.Content.ReadAsStringAsync();
            return rlt;
        }



        string GetNoticeContent(string message)
        {
            var env = Ioc.GetService<IHostEnvironment>();
            var environmentName = env?.EnvironmentName;
            var applicationName = env?.ApplicationName;
            var content = $"###  Test 任务通知\n" +
                          " #### 任务基础属性\n\n" +
                          "> #### JobCode:(任务代号)\n\n" +
                          $"> ##### TestJobCode\n\n" +
                          "> #### JobId:(本次运行的jobId)\n\n" +
                          $"> ##### TestJobId\n\n" +
                          "#### 执行情况:\n\n" +
                          $"> ##### {message}\n\n" +
                          // "> ![screenshot](https://gw.alipayobjects.com/zos/skylark-tools/public/files/84111bbeba74743d2771ed4f062d1f25.png)\n" +
                          $"  ####  {applicationName}_{environmentName} \n";
            return content;
        }

        [HttpGet("debugapi")]
        [NonAuth, DebugApi]
        public object DebugApi([FromQuery] TIn req = null)
        {
            LogHelper.Info($"DebugApi");
            return Get(req);
        }

        [HttpPost("debugapi/post")]
        [NonAuth, DebugApi]
        public object DebugApiPost([FromBody] TIn req = null)
        {
            LogHelper.Info($"DebugApiPost");
            return Get(req);
        }


        [HttpGet("sysapi/v0")]
        [NonAuth, SysApi]
        public object SysApi([FromQuery] TIn req = null)
        {
            return Get(req);
        }

        [HttpGet("sysapi/v10")]
        [NonAuth, SysApi(10)]
        public object SysApiV10([FromQuery] TIn req = null)
        {
            return Get(req);
        }

        [HttpGet("tt")]
        [NonAuth]
        public object Get([FromQuery] TIn req = null)
        {
            var signData = new Dictionary<string, string>();

            foreach (var item in SignDict)
            {
                var data = GetHeaderData(Request, item.Key);
                signData.Add(item.Key, data);
            }

            var rangeData = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
            })
                .ToArray();
            var allHeaderData = GetAllHeaderData(Request);
            return new { allHeaderData, signData, rangeData };
        }


        [HttpGet("t")]
        public object Get111()
        {
            var signData = new Dictionary<string, string>();

            foreach (var item in SignDict)
            {
                var data = GetHeaderData(Request, item.Key);
                signData.Add(item.Key, data);
            }

            var rangeData = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
            })
                .ToArray();
            var allHeaderData = GetAllHeaderData(Request);
            return new { allHeaderData, signData, rangeData };
        }




        [HttpGet]
        public IEnumerable<WeatherForecast> GetTTT()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }




    /// <summary> </summary>
    public class TIn
    {
        /// <summary>  KL1 </summary>
        public string KL1 { get; set; }

        /// <summary> kl1 </summary>
        public int kl1 { get; set; }

        /// <summary> Kl1 </summary>
        public decimal Kl1 { get; set; }

        /// <summary> kL1 </summary>
        public string kL1 { get; set; }



    }


}
