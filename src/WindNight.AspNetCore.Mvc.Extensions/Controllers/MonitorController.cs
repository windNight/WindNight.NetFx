using System;
using System.Linq;
using WindNight.AspNetCore.Mvc.Extensions.FilterAttributes;
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
        [SysApiAuthActionFilter(false)]
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
        [SysApiAuthActionFilter(false)]
        public object HealthCheck()
        {
            return GetInfo();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet("zabbix")]
        [ClearResult]
        [SysApiAuthActionFilter(false)]
        public object Zabbix()
        {
            return GetInfo();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet("slb")]
        [ClearResult]
        [SysApiAuthActionFilter(false)]
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
        [NonAuth, SysApi(5)]
        [SysApiAuthActionFilter]
        public object SvrInfo()
        {
            var clientIp = GetHttpClientIp();

            //if (!clientIp.IsInternalIp() && GetAppTokenValue().IsNullOrEmpty())
            //{
            //    return false;
            //}

            if (!IsAuthType1())
            {
                return NotFound();
            }
            //  var svrInfo = DefaultSvrHostInfo.GenDefault;
            var svrInfo = HardInfo.GenSvrMonitorInfo(SvrMonitorTypeEnum.Query);
            svrInfo.QueryDateTime = HardInfo.NowFullString;
            var serIp = svrInfo.ServerIp;
            if (serIp.IsNullOrEmpty() || IPHelper.IsDefaultIp(serIp))
            {
                svrInfo.ServerIp = GetHttpServerIp();
            }

            svrInfo.ClientIp = clientIp;
            return svrInfo;
        }

        [HttpGet("buildinfo")]
        [NonAuth, SysApi(5)]
        [SysApiAuthActionFilter]
        public virtual object QueryBuildInfo()
        {
            if (!IsAuthType1())
            {
                return NotFound();
            }

            var buildInfo = HardInfo.QuerySvrBuildInfo();

            var obj = buildInfo.SvrBuildInfoDict.ToDictionary(k => k.Key, v => v.Value);

            obj["AppId"] = SysAppId;
            obj["AppCode"] = SysAppCode;
            obj["AppName"] = SysAppName;
            obj["ClientIp"] = GetHttpClientIp();
            obj["ServiceIp"] = GetHttpServerIp();

            // obj["BuildTime"] = BuildInfo.BuildTs.ConvertToTimeFormatUseUnix();
            return obj;
        }


    }
}
