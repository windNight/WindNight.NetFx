﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.WnExtensions.Abstractions.Attributes;
using System.Attributes;
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
        [HttpGet("check/ips")]
        [NonAuth]
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
            var serverIp = _httpContextAccessor.HttpContext.GetServerIp();
            var clientIp = _httpContextAccessor.HttpContext.GetClientIp();
            return $"200-ok-{serverIp}-{clientIp}";
        }
    }
}