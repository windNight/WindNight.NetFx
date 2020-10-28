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
using WindNight.Extension;

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
            return new
            {
                Configuration = GetConfiguration()
            };
        }

        private object GetConfiguration(IEnumerable<IConfigurationSection> sections = null)
        {
            var _config = Ioc.GetService<IConfiguration>();
            return _config.GetConfiguration(sections);
        }

        [HttpGet("info")]
        [NonAuth]
        public object GetProjectVersion()
        {
            return new
            {
                DateTime = DateTime.Now,
                ConfigItems.SysAppId,
                ConfigItems.SysAppCode,
                ConfigItems.SysAppName,
                AssemblyVersions = GetAssemblyVersions(),
                ServerIp = _httpContextAccessor.HttpContext.GetServerIP()
            };
        }

        [HttpGet("version")]
        [NonAuth]
        public object GetVersion()
        {
            return new
            {
                DateTime = DateTime.Now,
                AssemblyVersion = typeof(InternalController).Assembly?.GetName()?.Version?.ToString()
            };
        }

        [HttpGet("versions")]
        [NonAuth]
        public object GetVersions()
        {
            return new
            {
                DateTime = DateTime.Now,
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