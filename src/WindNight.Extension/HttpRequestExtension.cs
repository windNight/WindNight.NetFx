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

        public static string GetUserAgentValue(this HttpRequest httpRequest) => httpRequest.QueryHeaderValue(ConstantKeys.UserAgentKey);

        public static string GetAppTokenValue(this HttpRequest httpRequest) => httpRequest.QueryHeaderValue(ConstantKeys.AppTokenKey);

        public static string GetAuthorizationValue(this HttpRequest httpRequest) => httpRequest.QueryHeaderValue(ConstantKeys.AuthorizationKey);

        public static string GetAppCodeValue(this HttpRequest httpRequest) => httpRequest.QueryHeaderValue(ConstantKeys.AppCodeKey);

        public static string GetAppNameValue(this HttpRequest httpRequest) => httpRequest.QueryHeaderValue(ConstantKeys.AppNameKey);

        public static string GetAppEnvNameValue(this HttpRequest httpRequest) => httpRequest.QueryHeaderValue(ConstantKeys.AppEnvNameKey);

        public static string GetReqTraceIdValue(this HttpRequest httpRequest) => httpRequest.QueryHeaderValue(ConstantKeys.ReqTraceIdKey);

        public static string GetAccessTokenValue(this HttpRequest httpRequest)
        {
            var authorizationValue = GetAuthorizationValue(httpRequest);

            if (authorizationValue.IsNullOrEmpty())
            {
                return string.Empty;
            }

            var akArray = authorizationValue.Split(' ');
            if (akArray.Length == 2)
            {
                var akType = akArray[0];
                var ak = akArray[1];
                return ak;
            }

            return string.Empty;

        }


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
                if (header.IsNotNullOrEmpty())
                {
                    return header.Trim();
                }
            }

            return defaultValue;
        }




    }
}
