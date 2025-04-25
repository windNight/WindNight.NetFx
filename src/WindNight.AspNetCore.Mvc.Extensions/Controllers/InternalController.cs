using System;
using System.Attributes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.WnExtensions.@internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Extensions;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.AspNetCore.Mvc.Extensions;
using WindNight.Core;
using WindNight.Core.Attributes.Abstractions;
using WindNight.Extension;
using IpHelper = WindNight.Extension.HttpContextExtension;

namespace Microsoft.AspNetCore.Mvc.WnExtensions.Controllers
{

    [Route("api/internal")]
    [SysApi(10)]
    [NonAuth]
    public class InternalController : DefaultApiControllerBase // Controller
    {
        // private readonly IHttpContextAccessor _httpContextAccessor;

        public InternalController()
        {

        }


        [HttpGet("config")]
        public object GetConfigs()
        {
            if (!HttpClientIpIsPrivate())
            {
                if (!ConfigItems.OpenInternalApi)
                {
                    return false;
                }
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
            if (!HttpClientIpIsPrivate())
            {
                if (!ConfigItems.OpenInternalApi)
                {
                    return false;
                }
            }

            var _config = Ioc.GetService<IConfiguration>();
            return _config.GetConfiguration(sections);
        }

        [HttpGet("info")]
        public object GetProjectVersion()
        {
            if (!HttpClientIpIsPrivate())
            {
                if (!ConfigItems.OpenInternalApi)
                {
                    return false;
                }
            }

            return new
            {
                DateTime = HardInfo.NowFullString,
                ConfigItems.SystemAppId,
                ConfigItems.SystemAppCode,
                ConfigItems.SystemAppName,
                AssemblyVersions = GetAssemblyVersions(),
                ServerIp = GetHttpServerIp(),
                IpHelper.LocalServerIp,
                IpHelper.LocalServerIps,
            };
        }

        [HttpGet("version")]
        public object GetVersion()
        {
            if (!HttpClientIpIsPrivate())
            {
                if (!ConfigItems.OpenInternalApi)
                {
                    return false;
                }
            }

            return new
            {
                DateTime = HardInfo.NowFullString,
                AssemblyVersion = typeof(InternalController).Assembly?.GetName()?.Version?.ToString(),
            };
        }

        [HttpGet("versions")]
        public object GetVersions()
        {
            if (!HttpClientIpIsPrivate())
            {
                if (!ConfigItems.OpenInternalApi)
                {
                    return false;
                }
            }

            return new
            {
                DateTime = HardInfo.NowFullString,
                AssemblyVersions = GetAssemblyVersions(),
            };
        }

        [HttpGet("versions/internal")]
        public object GetInternalVersions()
        {
            if (!HttpClientIpIsPrivate())
            {
                if (!ConfigItems.OpenInternalApi)
                {
                    return false;
                }
            }

            return new
            {
                DateTime = HardInfo.NowFullString,
                AssemblyVersions = GetAssemblyVersions(true),
            };
        }

        private object GetAssemblyVersions(bool ignoreMicrosoft = false)
        {
            try
            {
                var result = new SortedDictionary<string, string>();
                var path = AppDomain.CurrentDomain.BaseDirectory;
                var todoFiles = Directory.GetFiles(path).Where(m =>
                {

                    var flag = ".dll".Equals(Path.GetExtension(m));
                    if (!flag)
                    {
                        return false;
                    }

                    if (ignoreMicrosoft)
                    {
                        var fileName = Path.GetFileName(m);
                        if (fileName.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase) || fileName.StartsWith("System", StringComparison.OrdinalIgnoreCase))
                        {
                            return false;
                        }
                    }
                    return flag;
                });
                foreach (var file in todoFiles)
                {
                    try
                    {
                        var assembly = Assembly.LoadFile(file);
                        if (assembly != null)
                        {
                            var assemblyName = assembly.GetName();
                            if (assemblyName != null)
                            {
                                var key = assemblyName.Name;
                                if (ignoreMicrosoft && (key.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase) || key.StartsWith("System", StringComparison.OrdinalIgnoreCase)))
                                {
                                    continue;
                                }
                                if (!key.IsNullOrEmpty() && !result.ContainsKey(key))
                                {
                                    var value = assemblyName.Version.ToString();
                                    result.Add(key, value);

                                }
                            }
                        }

                    }
                    catch
                    {
                    }
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
