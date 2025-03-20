using System;
using System.Attributes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.WnExtensions.Abstractions.Attributes;
using Microsoft.AspNetCore.Mvc.WnExtensions.@internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Extensions;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Extension;
using IpHelper = WindNight.Extension.HttpContextExtension;

namespace Microsoft.AspNetCore.Mvc.WnExtensions.Controllers
{
    [Route("api/internal")]
    [HiddenApi(false, true)]
    [NonAuth]
    public class InternalController : ControllerBase // Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InternalController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpGet("config")]
        public object GetConfigs()
        {
            if (!ConfigItems.OpenInternalApi)
            {
                return false;
            }

            return new { Configuration = GetConfiguration() };
        }

        //[HttpGet("config2")]
        //public object GetConfigs2()
        //{
        //    if (!ConfigItems.OpenInternalApi)
        //    {
        //        return false;
        //    }

        //    return new { Configuration = ConfigItems.GetAllConfigs() };
        //}

        private object GetConfiguration(IEnumerable<IConfigurationSection> sections = null)
        {
            if (!ConfigItems.OpenInternalApi)
            {
                return false;
            }

            var _config = Ioc.GetService<IConfiguration>();
            return _config.GetConfiguration(sections);
        }

        [HttpGet("info")]
        public object GetProjectVersion()
        {
            if (!ConfigItems.OpenInternalApi)
            {
                return false;
            }

            return new
            {
                DateTime = HardInfo.Now,
                ConfigItems.SysAppId,
                ConfigItems.SysAppCode,
                ConfigItems.SysAppName,
                AssemblyVersions = GetAssemblyVersions(),
                ServerIp = _httpContextAccessor.HttpContext.GetServerIp(),
                IpHelper.LocalServerIp,
                IpHelper.LocalServerIps,
            };
        }

        [HttpGet("version")]
        public object GetVersion()
        {
            if (!ConfigItems.OpenInternalApi)
            {
                return false;
            }

            return new
            {
                DateTime = HardInfo.Now,
                AssemblyVersion = typeof(InternalController).Assembly?.GetName()?.Version?.ToString(),
            };
        }

        [HttpGet("versions")]
        public object GetVersions()
        {
            if (!ConfigItems.OpenInternalApi)
            {
                return false;
            }

            return new { DateTime = HardInfo.Now, AssemblyVersions = GetAssemblyVersions() };
        }

        private object GetAssemblyVersions()
        {
            try
            {
                var result = new Dictionary<string, string>();
                var path = AppDomain.CurrentDomain.BaseDirectory;
                foreach (var file in Directory.GetFiles(path).Where(m => ".dll".Equals(Path.GetExtension(m))))
                    try
                    {
                        var assembly = Assembly.LoadFile(file);
                        if (assembly != null)
                            result.Add(assembly.GetName()?.Name, assembly.GetName()?.Version?.ToString());
                    }
                    catch
                    {
                    }

                return result;
            }
            catch
            {
            }

            return null;
        }
    }
}
