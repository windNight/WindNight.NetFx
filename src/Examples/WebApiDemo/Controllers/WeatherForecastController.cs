using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.WnExtensions.Abstractions.Attributes;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Options;
using WindNight.Core.Abstractions;
using WindNight.Extension.Logger.DcLog.Abstractions;
using WindNight.Extension.Logger.DcLog.Extensions;
using WindNight.LogExtension;

namespace WebApiDemo.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
        };

        private readonly ILogger<WeatherForecastController> _logger;

        private readonly Dictionary<string, string> SignDict = new()
        {
            { "Authorization", "格式 Bearer xx" },
            { "AppId", "AppId" },
            { "AppCode", "AppCode" },
            { "AppToken", "Sign" },
            { "Ts", "当前时间戳" },
        };

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

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

        private Dictionary<string, string> GetAllHeaderData(HttpRequest httpRequest)
        {
            var dict = new Dictionary<string, string>();
            foreach (var item in httpRequest.Headers)
            {
                dict.Add(item.Key, item.Value);
            }

            return dict;
        }

        [HttpGet("tt")]
        [NonAuth]
        public object Get()
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


        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> GetWeatherForecast()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(HardInfo.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)],
                })
                .ToArray();
        }


        [HttpGet("log/config")]
        [NonAuth]
        public object TestLogConfig()
        {
            try
            {
                var dcLogOption = Ioc.GetService<IOptionsMonitor<DcLogOptions>>().CurrentValue;

                return new
                {
                    HardInfo.OperatorSys,
                    LogConfigPath = LogHelper.CurrentLog4NetConfigPath,
                    DcLogOptions = dcLogOption,
                };
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        [HttpGet("log/push")]
        [NonAuth]
        public object TestLogPush()
        {
            LogHelper.Info("test info log store LogHelper ");

            DcLogHelper.Info("test info log store DcLogHelper ");

            Ioc.GetService<ILogService>()?.Info("test info log store ILogService");

            LogHelper.Log4Info("test info log store Log4Info");

            return true;
        }
    }
}
