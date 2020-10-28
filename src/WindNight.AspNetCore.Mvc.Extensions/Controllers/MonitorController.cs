using System.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.WnExtensions.Abstractions.Attributes;
using WindNight.Core.Extension;
using WindNight.Extension;

namespace Microsoft.AspNetCore.Mvc.WnExtensions.Controllers
{
    [Route("api/monitor")]
    [HiddenApi]
    public class MonitorController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MonitorController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet("healthcheck")]
        [HttpGet("basicmonitor")]
        [NonAuth]
        [ClearResult]
        public object HealthCheck()
        {
            return GetInfo();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet("zabbix")]
        [NonAuth]
        [ClearResult]
        public object Zabbix()
        {
            return GetInfo();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet("slb")]
        [NonAuth]
        [ClearResult]
        public object Slb()
        {
            return GetInfo();
        }

        private object GetInfo()
        {
            var serverIp = _httpContextAccessor.HttpContext.GetServerIP();
            var clientIp = _httpContextAccessor.HttpContext.GetClientIP();
            return $"200-ok-{serverIp}-{clientIp}";
        }
    }
}