using System;
using System.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.WnExtensions.@internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Extensions;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.AspNetCore.Mvc.Extensions;
using WindNight.AspNetCore.Mvc.Extensions.FilterAttributes;
using WindNight.Core;
using WindNight.Core.Attributes.Abstractions;
using WindNight.Extension;
using WindNight.Linq.Extensions.Expressions;
using IpHelper = WindNight.Extension.HttpContextExtension;

namespace Microsoft.AspNetCore.Mvc.WnExtensions.Controllers
{

    [Route("api/internal")]
    [SysApi(50)]
    [NonAuth]
    [SysApiAuthActionFilter]
    public class InternalController : DefaultApiControllerBase // Controller
    {
        // private readonly IHttpContextAccessor _httpContextAccessor;

        public InternalController()
        {

        }

        //protected override bool IsAuthType1(bool ignoreIp = true)
        //{

        //    if (ignoreIp && HttpClientIpIsPrivate())
        //    {
        //        return true;
        //    }

        //    var ak = GetAccessToken();

        //    var appToken = GetAppTokenValue();
        //    if (ak.IsNullOrEmpty(true) && appToken.IsNullOrEmpty(true))
        //    {
        //        return false;
        //    }

        //    if (!ConfigItems.OpenInternalApi)
        //    {
        //        return false;
        //    }

        //    return true;

        //}

        [HttpGet("config")]
        public object GetConfigs()
        {
            if (!IsAuthType1())
            {
                return NotFound();
            }

            return new
            {
                Configuration = GetConfiguration(),
            };
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
            if (!IsAuthType1())
            {
                return NotFound();
            }

            var _config = Ioc.GetService<IConfiguration>();
            return _config.GetConfiguration(sections);
        }

        [HttpGet("info")]
        public object GetProjectVersion()
        {
            if (!IsAuthType1())
            {
                return NotFound();
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
            if (!IsAuthType1())
            {
                return NotFound();
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
            if (!IsAuthType1())
            {
                return NotFound();
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
            if (!IsAuthType1())
            {
                return NotFound();
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
                                if (key.IsNotNullOrEmpty() && !result.ContainsKey(key))
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



        [HttpGet("svr/pid")]
        [SysApi(100)]
        public object GetSvrPId()
        {
            if (!IsAuthType1())
            {
                return NotFound();
            }

            return new
            {
                DateTime = HardInfo.NowFullString,
                PId = HardInfo.QueryRuntimeProcessId(),
            };

        }


        [HttpGet("svr/kill")]
        [SysApi(100)]
        public object KillSvr()
        {
            if (!IsAuthType1())
            {
                return NotFound();
            }

            Environment.Exit(-1);

            return -1;

        }


        [HttpGet("svr/kill/pid")]
        [SysApi(100)]
        public object KillSvr(int pId)
        {
            if (!IsAuthType1())
            {
                return NotFound();
            }

            KillProcess(pId);
            return -1;
        }

        [HttpGet("svr/kill/pname")]
        [SysApi(100)]
        public object KillSvr(string pName)
        {
            if (!IsAuthType1())
            {
                return NotFound();
            }

            KillProcess(pName);
            return -1;
        }


        private void KillProcess(int pId)
        {

            try
            {
                if (pId == Environment.ProcessId)
                {
                    // 如果是当前进程，则不允许杀掉。使用另一个自杀的方法
                    throw new InvalidOperationException("Cannot terminate the calling process or its process tree.");

                    return;
                }

                var pInfo = Process.GetProcessById(pId);
                if (pInfo == null)
                {
                    return;

                }

                var startInfo = new ProcessStartInfo
                {
                    FileName = "taskkill",
                    Arguments = $"/PID {pId} /T /F", // /T terminates the tree, /F forces termination
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                };

                using (var process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                    {
                        string error = process.StandardError.ReadToEnd();
                        var msg = $"Failed to terminate process tree for PID {pId}: {error}";
                        LogHelper.Warn(msg);
                    }
                }
            }
            catch (Win32Exception ex)
            {

            }
            catch (InvalidOperationException ex)
            {

            }



        }


        private void KillProcess(string pName)
        {
            try
            {
                var processList = Process.GetProcessesByName(pName);
                if (processList.IsNullOrEmpty())
                {
                    return;
                }

                if (processList.Any(m => m.Id == Environment.ProcessId))
                {
                    // 如果是当前进程，则不允许杀掉。使用另一个自杀的方法
                    return;
                }


                foreach (var process in processList)
                {
                    try
                    {
                        if (process.Id == Environment.ProcessId)
                        {
                            // 如果是当前进程，则不允许杀掉。使用另一个自杀的方法
                            continue;
                        }


                        // 杀掉这个进程。
                        process.Kill(true);

                        process.WaitForExit();
                    }
                    catch (Win32Exception ex)
                    {

                    }
                    catch (InvalidOperationException)
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

    }
}
