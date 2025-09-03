#if NETFRAMEWORK
using System.Web;
using System.Runtime.Remoting.Messaging;
#else
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.WnExtension;
#endif
using WindNight.Core.Attributes.Abstractions;
using WindNight.Core.Extension;
using WindNight.Linq.Extensions.Expressions;


namespace WindNight.Extension
{
    [Alias("IpHelper")]
    public static class HttpContextExtension
    {
        private const string DefaultIp = "0.0.0.0";
        // private const string LocalServerIpKey = "WindNight:HttpContext:LocalServerIp";
        // private const string LocalServerIpsKey = "WindNight:HttpContext:LocalServerIps";

        public static List<string> LocalServerIps = HardInfo.GetLocalIps().ToList();
        public static string LocalServerIp = HardInfo.GetLocalIp().IpV6ToIpV4() ?? "";
        public static string LocalServerIpsString = LocalServerIps.Join();

        public static string GetLocalServerIp()
        {
            try
            {
                return LocalServerIp;
            }
            catch (Exception ex)
            {
                return DefaultIp;
            }
        }

        public static IEnumerable<string> GetLocalServerIps()
        {
            try
            {
                return LocalServerIps;
            }
            catch (Exception ex)
            {
                return new[] { DefaultIp };
            }
        }


        /// <summary> 获取本地IP地址信息  </summary>
        public static string GetServerIp(bool onlyIpV4 = true)
        {
            try
            {
                var context = GetHttpContext();
                return context.GetServerIp(onlyIpV4);
            }
            catch
            {
                return DefaultIp;
            }
        }

        ///// <summary>
        ///// </summary>
        ///// <returns></returns>
        //private static string GetLocalIp()
        //{
        //    return GetLocalIps().FirstOrDefault();
        //}
        ///// <summary>
        ///// </summary>
        ///// <returns></returns>
        //private static IEnumerable<string> GetLocalIps()
        //{
        //    try
        //    {
        //        var validAddressFamilies = new List<AddressFamily> { AddressFamily.InterNetwork, AddressFamily.InterNetworkV6 };
        //        var unicastAddresses = NetworkInterface.GetAllNetworkInterfaces()?
        //            .Where(m => m.OperationalStatus == OperationalStatus.Up)?
        //            .Select(m => m.GetIPProperties().UnicastAddresses);

        //        var ipList = (from unicastAddress in unicastAddresses
        //                      from unicastIpAddress in unicastAddress
        //                      where unicastIpAddress != null
        //                      where unicastIpAddress.IsDnsEligible
        //                      where validAddressFamilies.Contains(unicastIpAddress.Address.AddressFamily)
        //                      select unicastIpAddress.Address.ToString()).ToList();

        //        if (!ipList.Any())
        //        {
        //            ipList.Add(DefaultIp);
        //        }
        //        return ipList;
        //    }
        //    catch
        //    {
        //        return new[] { DefaultIp };
        //    }
        //}

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetServerIp(this HttpContext context, bool onlyIpV4 = true)
        {
            try
            {
                if (context == null)
                {
                    //  var serverIps = LocalServerIps.Join(",");
                    return LocalServerIp;
                }

                var serverIp = string.Empty;
#if NETFRAMEWORK
                serverIp = context?.Request?.ServerVariables?.Get("Local_Addr") ?? "::1";
#else
                serverIp = context.Connection?.LocalIpAddress?.ToString() ?? string.Empty;
#endif

                if (serverIp.IsDefaultIp())
                {
                    // serverIp = LocalServerIps.Join(",");
                    serverIp = LocalServerIp;
                }

                if (onlyIpV4)
                {
                    serverIp = serverIp.IpV6ToIpV4();
                }

                return serverIp;
            }
            catch
            {
                return DefaultIp;
            }
        }

        public static string GetCurrentUrl(this HttpContext context)
        {
            try
            {
                return context?.Request?.Path ?? "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string GetCurrentUrl()
        {
            try
            {
                var context = GetHttpContext();
                return context.GetCurrentUrl();
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static string GetClientIp(bool onlyIpV4 = true)
        {
            try
            {
                var context = GetHttpContext();
                return context.GetClientIp(onlyIpV4);
            }
            catch
            {
                return DefaultIp;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetClientIp(this HttpContext context, bool onlyIpV4 = true)
        {
            try
            {
                if (context == null)
                {
                    return DefaultIp;
                }

                var headerDict = GetHeaderDict(context);
                var ip = GetIpFromDict(headerDict);
                if (ip.IsNullOrEmpty())
                {
#if NETFRAMEWORK
                    ip = context.Request.UserHostAddress;
#else
                    ip = context.Connection.RemoteIpAddress?.ToString();
#endif
                }

                if ("::1".Equals(ip))
                {
                    return DefaultIp;
                }

                if (ip.IsNullOrEmpty())
                {
                    return DefaultIp;
                }

                var clientIp = ip.Split(',')[0];
                if (onlyIpV4)
                {
                    clientIp = clientIp.IpV6ToIpV4();
                }

                return clientIp;
            }
            catch
            {
                return DefaultIp;
            }
        }


#if !NETFRAMEWORK

        public static Dictionary<string, string> GetClientIps(this HttpContext context)
        {
            var dict = new Dictionary<string, string>();
            try
            {
                if (context == null)
                {
                    return dict;
                }

                var headerDict = GetHeaderDict(context);
                if (headerDict.IsNullOrEmpty())
                {
                    headerDict.Add("RemoteIpAddress", context.Connection.RemoteIpAddress?.ToString() ?? "");
                }

                return headerDict;
            }
            catch
            {
                return dict;
            }
        }

#endif

        public static HttpContext? GetHttpContext()
        {
            HttpContext? context = null;

#if NETFRAMEWORK
            context = HttpContext.Current;
#else
            context = Ioc.GetService<IHttpContextAccessor>()?.HttpContext;
#endif
            return context;
        }


        #region =====Private =====

        private static Dictionary<string, string> GetHeaderDict(HttpContext context)
        {
            var headerDict = new Dictionary<string, string>();
#if !NETFRAMEWORK

            var validIPKeys = new[] { "X-Real-IP", "HTTP_X_REAL_IP", "x-forwarded-for", "REMOTE_ADDR" };
            foreach (var item in context.Request.Headers.Where(m => validIPKeys.Contains(m.Key)))
            {
                headerDict.Add(item.Key, item.Value);
            }
#else
            foreach (var item in context.Request.Headers.Keys)
            {
                headerDict.Add(item.ToString(), context.Request.Headers[item.ToString()]?.ToString());
            }
            if (!context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].IsNullOrEmpty())
            {
                headerDict.Add("HTTP_X_FORWARDED_FOR", context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]?.ToString());
            }
            if (!context.Request.ServerVariables["REMOTE_ADDR"].IsNullOrEmpty())
            {
                headerDict.Add("REMOTE_ADDR", context.Request.ServerVariables["REMOTE_ADDR"]?.ToString());
            }

#endif

            return headerDict;
        }

        private static string GetIpFromDict(Dictionary<string, string> headerDict)
        {
            var ip = string.Empty;
            var timKey = new[]
            {
                "HTTP_X_REAL_IP", "X-Real-IP", "x-forwarded-for", "HTTP_X_FORWARDED_FOR", "REMOTE_ADDR",
            };

            foreach (var key in timKey)
            {
                if (headerDict.TryGetValue(key, out ip) && ip.IsNotNullOrEmpty())
                {
                    break;
                }
            }

            return ip;
        }

        #endregion
    }
}
