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
    [Route("[controller]")]
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

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(HardInfo.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
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
                    LogConfigPath =  LogHelper.CurrentLog4NetConfigPath,
                    DcLogOptions = dcLogOption

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
            LogHelper.Info($"test info log store LogHelper ");

            DcLogHelper.Info($"test info log store DcLogHelper ");

            Ioc.GetService<ILogService>()?.Info("test info log store ILogService");

            LogHelper.Log4Info("test info log store Log4Info");

            return true;
        }


    }
}
