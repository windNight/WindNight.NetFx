using System;
using System.Attributes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.WnExtensions.Abstractions.Attributes;
using Microsoft.AspNetCore.Mvc.WnExtensions.Internals;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Extensions;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.ConfigCenter.Extension;
using WindNight.Extension;
using IpHelper = WindNight.Extension.HttpContextExtension;

namespace Microsoft.AspNetCore.Mvc.WnExtensions.Controllers
{
    [Route("api/internal")]
    [HiddenApi]
    public class InternalController : ControllerBase // Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InternalController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpGet("config")]
        [NonAuth]
        public object GetConfigs()
        {
            if (!ConfigItems.OpenInternalApi)
            {
                return false;
            }
            return new
            {
                Configuration = GetConfiguration()
            };
        }
        [HttpGet("config2")]
        [NonAuth]
        public object GetConfigs2()
        {
            if (!ConfigItems.OpenInternalApi)
            {
                return false;
            }
            return new
            {
                Configuration = ConfigItems.GetAllConfigs(),
            };
        }
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
        [NonAuth]
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
                LocalServerIp = IpHelper.LocalServerIp,
                LocalServerIps = IpHelper.LocalServerIps,
            };
        }

        [HttpGet("version")]
        [NonAuth]
        public object GetVersion()
        {
            if (!ConfigItems.OpenInternalApi)
            {
                return false;
            }
            return new
            {
                DateTime = HardInfo.Now,
                AssemblyVersion = typeof(InternalController).Assembly?.GetName()?.Version?.ToString()
            };
        }

        [HttpGet("versions")]
        [NonAuth]
        public object GetVersions()
        {
            if (!ConfigItems.OpenInternalApi)
            {
                return false;
            }
            return new
            {
                DateTime = HardInfo.Now,
                AssemblyVersions = GetAssemblyVersions()
            };
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