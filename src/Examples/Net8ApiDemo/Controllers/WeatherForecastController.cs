using Microsoft.AspNetCore.Mvc;
using WindNight.Core.Attributes.Abstractions;
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
            return new { signData, rangeData };
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
