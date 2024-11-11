using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using WindNight.Core.Abstractions;
using WindNight.Linq.Extensions.Expressions;

#if NETFRAMEWORK
using System.Web;
using System.Runtime.Remoting.Messaging;
#else
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.WnExtension;
#endif
namespace WindNight.Extension
{
    public class IPHelper
    {
        private const string DefaultIp = "0.0.0.0";
        // private const string LocalServerIpKey = "WindNight:HttpContext:LocalServerIp";
        // private const string LocalServerIpsKey = "WindNight:HttpContext:LocalServerIps";

        public static List<string> LocalServerIps = HardInfo.GetLocalIps().ToList();
        public static string LocalServerIp = LocalServerIps.FirstOrDefault();
        public static string LocalServerIpsString = string.Join(",", LocalServerIps);

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


        /// <summary>
        /// </summary>
        /// <param name="ipStr"></param>
        /// <returns></returns>
        public static bool IsDefaultIp(string ipStr)
        {
            return ipStr == "127.0.0.1" || ipStr == "0.0.0.0" || ipStr == "::1";
        }

        /// <summary> 获取本地IP地址信息  </summary>
        public static string GetServerIp()
        {
            try
            {
                var context = GetHttpContext();
                return context.GetServerIp();
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
        public static string GetClientIp()
        {
            try
            {
                var context = GetHttpContext();
                return context.GetClientIp();
            }
            catch
            {
                return DefaultIp;
            }
        }





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
                headerDict.Add(item.Key, item.Value);
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
                "HTTP_X_REAL_IP",
                "X-Real-IP",
                "x-forwarded-for",
                "HTTP_X_FORWARDED_FOR",
                "REMOTE_ADDR"
            };
            foreach (var key in timKey)
                if (headerDict.TryGetValue(key, out ip) && !ip.IsNullOrEmpty())
                    break;
            return ip;
        }

        #endregion
    }
}
