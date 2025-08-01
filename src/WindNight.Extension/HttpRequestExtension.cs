using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WindNight.Core;

namespace WindNight.AspNetCore.Mvc.Extensions
{
    public static class HttpRequestExtension
    {

        public static string GetUserAgentValue(this HttpRequest httpRequest) => httpRequest.QueryHeaderValue(ConstantKeys.USER_AGENT_KEY);

        public static string GetAppTokenValue(this HttpRequest httpRequest) => httpRequest.QueryHeaderValue(ConstantKeys.AppTokenKey);

        public static string GetAuthorizationValue(this HttpRequest httpRequest) => httpRequest.QueryHeaderValue(ConstantKeys.AuthorizationKey);

        public static string GetAppCodeValue(this HttpRequest httpRequest) => httpRequest.QueryHeaderValue(ConstantKeys.AppCodeKey);

        public static string GetAppNameValue(this HttpRequest httpRequest) => httpRequest.QueryHeaderValue(ConstantKeys.AppNameKey);

        public static string GetAppEnvNameValue(this HttpRequest httpRequest) => httpRequest.QueryHeaderValue(ConstantKeys.AppEnvNameKey);

        public static long GetTimestampValue(this HttpRequest httpRequest)
        {

            var tsString = httpRequest.QueryHeaderValue(ConstantKeys.TimestampKey);
            if (tsString.IsNullOrEmpty())
            {
                return 0L;
            }

            return tsString.ToLong(0L);

        }

        public static string QueryHeaderValue(this HttpRequest httpRequest, string headerName, string defaultValue = "")
        {
            if (httpRequest.Headers.TryGetValue(headerName, out var requestHeader))
            {
                var header = requestHeader.FirstOrDefault();
                if (!header.IsNullOrEmpty())
                {
                    return header.Trim();
                }
            }

            return defaultValue;
        }




    }
}
