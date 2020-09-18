using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;

#if NET45
using System.Web;
#else
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.WnExtension;
#endif


namespace WindNight.Extension
{
    public static class HttpContextExtension
    {
        private const string DefaultIP = "0.0.0.0";

        /// <summary>
        /// </summary>
        /// <param name="ipStr"></param>
        /// <returns></returns>
        public static bool IsDefaultIp(string ipStr)
        {
            return ipStr == "127.0.0.1" || ipStr == "0.0.0.0" || ipStr == "::1";
        }

        /// <summary> 获取本地IP地址信息  </summary>
        public static string GetServerIP()
        {
            try
            {
                var context = GetHttpContext();
                return context.GetServerIP();
            }
            catch
            {
                return DefaultIP;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIP()
        {
            return GetLocalIPs().OrderBy(m => m).FirstOrDefault();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetLocalIPs()
        {
            try
            {
                var validAddressFamilies = new[] { AddressFamily.InterNetwork, AddressFamily.InterNetworkV6 };
                var ips = NetworkInterface.GetAllNetworkInterfaces()?
                    .Where(m => m.OperationalStatus == OperationalStatus.Up)?
                    .Select(m => m.GetIPProperties().UnicastAddresses)?
                    .FirstOrDefault()?
                    .Where(m => validAddressFamilies.Contains(m.Address.AddressFamily))?
                    .Select(m => m.Address.ToString()).OrderBy(m => m).ToArray();
                return ips ?? new[] { DefaultIP };
            }
            catch
            {
                return new[] { DefaultIP };
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetServerIP(this HttpContext context)
        {
            try
            {
                if (context == null)
                {
                    var ips = GetLocalIPs();
                    var serverIps = string.Join(",", ips);

                    return serverIps;
                }

                var serverIp = string.Empty;
#if NET45
                serverIp = context?.Request?.ServerVariables?.Get("Local_Addr") ?? "::1";
#else
                serverIp = context.Connection?.LocalIpAddress?.ToString() ?? string.Empty;
#endif

                if ("::1".Equals(serverIp))
                    serverIp = string.Join(",", GetLocalIPs());
                return serverIp;
            }
            catch
            {
                return DefaultIP;
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
        public static string GetClientIP()
        {
            try
            {
                var context = GetHttpContext();
                return context.GetClientIP();
            }
            catch
            {
                return DefaultIP;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetClientIP(this HttpContext context)
        {
            try
            {
                if (context == null) return DefaultIP;
                var headerDict = GetHeaderDict(context);
                var ip = GetIpFromDict(headerDict);
                if (string.IsNullOrEmpty(ip))
                {
#if NET45
                    ip = context.Request.UserHostAddress;
#else
                    ip = context.Connection.RemoteIpAddress?.ToString();
#endif 
                }

                if ("::1".Equals(ip)) return DefaultIP;

                if (string.IsNullOrEmpty(ip)) return DefaultIP;

                return ip.Split(',')[0];
            }
            catch
            {
                return DefaultIP;
            }
        }
        public static HttpContext GetHttpContext()
        {
            HttpContext context = null;

#if NET45
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
#if !NET45

            var validIPKeys = new[] { "X-Real-IP", "HTTP_X_REAL_IP", "x-forwarded-for" };
            foreach (var item in context.Request.Headers.Where(m => validIPKeys.Contains(m.Key)))
                headerDict.Add(item.Key, item.Value);
#else
            foreach (var item in context.Request.Headers.Keys)
            {
                headerDict.Add(item.ToString(), context.Request.Headers[item.ToString()].ToString());
            }
            if (!string.IsNullOrEmpty(context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]?.ToString()))
            {
                headerDict.Add("HTTP_X_FORWARDED_FOR", context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString());
            }
            if (!string.IsNullOrEmpty(context.Request.ServerVariables["REMOTE_ADDR"]?.ToString()))
            {
                headerDict.Add("REMOTE_ADDR", context.Request.ServerVariables["REMOTE_ADDR"].ToString());
            }

#endif

            return headerDict;
        }

        private static string GetIpFromDict(Dictionary<string, string> headerDict)
        {
            var ip = string.Empty;
            var timKey = new[]
            {
                "HTTP_X_REAL_IP",
                "X-Real-IP",
                "x-forwarded-for",
                "HTTP_X_FORWARDED_FOR",
                "REMOTE_ADDR"
            };
            foreach (var key in timKey)
                if (headerDict.TryGetValue(key, out ip) && !string.IsNullOrEmpty(ip))
                    break;
            return ip;
        }

        #endregion
    }
}
