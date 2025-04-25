using WindNight.Core.Abstractions;
using WindNight.Core.Attributes.Abstractions;
using WindNight.Core.Extension;
using WindNight.Extension;
using WindNight.Linq.Extensions.Expressions;

namespace Microsoft.AspNetCore.Mvc.WnExtensions.Controllers
{
    [Route("api/monitor")]
    [SysApi(1)]
    [NonAuth]
    public class MonitorController : DefaultApiControllerBase
    {
        // private readonly IHttpContextAccessor _httpContextAccessor;

        public MonitorController()//IHttpContextAccessor httpContextAccessor)
        {
            //_httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet("check/ips")]
        [ClearResult]
        public object HealthCheckIps()
        {
            return GetHttpClientIps();
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
            var serverIp = GetHttpServerIp();
            var clientIp = GetHttpClientIp();
            return $"200-ok-{serverIp}-{clientIp}";
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet("svrinfo")]
        public object SvrInfo()
        {
            var clientIp = GetHttpClientIp();

            if (!clientIp.IsPrivateOrLoopback() && GetAppTokenValue().IsNullOrEmpty())
            {
                return false;
            }

            var svrInfo = DefaultSvrHostInfo.GenDefault;
            var serIp = svrInfo.ServerIp;
            if (serIp.IsNullOrEmpty() || IPHelper.IsDefaultIp(serIp))
            {
                svrInfo.ServerIp = GetHttpServerIp();
            }

            svrInfo.ClientIp = clientIp;
            return svrInfo;
        }


    }
}
