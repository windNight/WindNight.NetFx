using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Extensions.@internal;
using WindNight.Core.Extension;

namespace Swashbuckle.AspNetCore.HideApi.@internal
{
    internal static class HttpContextExtension
    {
        public static bool IsInSwaggerPage(this HttpContext context)
        {
            var refer = context.Request.GetHeaderData("Referer");
            if (refer.IsNullOrEmpty())
            {
                return false;
            }

            if (!refer.Contains("swagger", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        public static string GetHeaderData(this HttpRequest httpRequest, string headerName, string defaultValue = "")
        {
            if (httpRequest.Headers.TryGetValue(headerName, out var requestHeader))
            {
                var header = requestHeader.FirstOrDefault();
                if (header.IsNotNullOrEmpty())
                {
                    return header.Trim();
                }
            }

            return defaultValue;
        }

        public static Dictionary<string, string> QueryClientIpDict(this HttpContext context)
        {
            var headerDict = new Dictionary<string, string>();


            var validIPKeys = new[] { "X-Real-IP", "HTTP_X_REAL_IP", "x-forwarded-for", "REMOTE_ADDR" };

            foreach (var item in context.Request.Headers.Where(m => validIPKeys.Contains(m.Key)))
            {
                headerDict.Add(item.Key, item.Value);
            }

            if (headerDict.IsNullOrEmpty())
            {
                headerDict.Add("RemoteIpAddress", context.Connection.RemoteIpAddress?.ToString() ?? "");
            }

            return headerDict;

        }

        public static string QueryDefaultClient(this HttpContext context, string defaultIp = "")
        {
            var dict = context.QueryClientIpDict();
            var ip = dict.FirstOrDefault().Value ?? "";
            if (ip.IsNullOrEmpty())
            {
                ip = defaultIp;
            }

            return ip;
        }


        public static bool IpValid(this string ip)
        {
            if (IPAddress.TryParse(ip, out var ipAddress))
            {
                return ipAddress.IpValid();
            }
            return false;
        }

        public static bool RemoteIpValid(this HttpContext context)
        {
            var remoteIp = context.Request.HttpContext.QueryDefaultClient();
            return remoteIp.IpValid();
        }

        public static bool IpValid(this IPAddress ipAddress)
        {
            if (ConfigItems.CheckClientIp)
            {
                try
                {
                    if (ipAddress == null)
                    {
                        return false;
                    }
                    if (!ipAddress.IsPrivateOrLoopback())
                    {
                        if (!ConfigItems.LimitIps.Contains(ipAddress.ToString()))
                        {
                            return false;
                        }
                    }

                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
