using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.WnExtensions.Abstractions.Attributes;
using System.Attributes;
using WindNight.Extension;

namespace Microsoft.AspNetCore.Mvc.WnExtensions.Controllers
{
    [Route("api/monitor")]
    [HiddenApi(testApi: true)]
    [NonAuth]
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
        [HttpGet("check/ips")]
        [ClearResult]
        public object HealthCheckIps()
        {
            return _httpContextAccessor.HttpContext.GetClientIps();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet("healthcheck")]
        [HttpGet("basicmonitor")]
        [ClearResult]
        public object HealthCheck()
        {
            return GetInfo();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet("zabbix")]
        [ClearResult]
        public object Zabbix()
        {
            return GetInfo();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet("slb")]
        [ClearResult]
        public object Slb()
        {
            return GetInfo();
        }

        private object GetInfo()
        {
            var serverIp = _httpContextAccessor.HttpContext.GetServerIp();
            var clientIp = _httpContextAccessor.HttpContext.GetClientIp();
            return $"200-ok-{serverIp}-{clientIp}";
        }
    }
}
