using System;
using System.Collections.Generic;
using WindNight.AspNetCore.Mvc.Extensions;
using WindNight.Core;
using WindNight.Core.Extension;
using WindNight.Extension;

namespace Microsoft.AspNetCore.Mvc.WnExtensions.Controllers
{
    public class DefaultApiControllerBase : ControllerBase
    {
        protected virtual string GetAccessToken()
        {
            var authorizationValue = GetAuthorizationValue();

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

        protected virtual string GetUserAgentValue() => Request.GetUserAgentValue();

        protected virtual string GetAppTokenValue() => Request.GetAppTokenValue();

        protected virtual string GetAuthorizationValue() => Request.GetAuthorizationValue();
        protected virtual string GetRequestAppCodeValue() => Request.GetAppCodeValue();
        protected virtual string GetRequestAppNameValue() => Request.GetAppNameValue();

        protected virtual long GetRequestTsValue() => Request.GetTimestampValue();


        protected virtual string QueryHeaderValue(string key)
        {
            var value = Request.QueryHeaderValue(key);
            return value;
        }


        protected virtual string GetHttpServerIp(bool onlyIpV4 = true)
        {
            var value = Request.HttpContext.GetServerIp(onlyIpV4);
            return value;
        }

        protected virtual string GetHttpClientIp(bool onlyIpV4 = true)
        {
            var value = Request.HttpContext.GetClientIp(onlyIpV4);
            return value;
        }

        protected virtual Dictionary<string, string> GetHttpClientIps()
        {
            var value = Request.HttpContext.GetClientIps();
            return value;
        }

        protected virtual bool HttpClientIpIsPrivate()
        {
            var clientIp = GetHttpClientIp();
            if (clientIp.IsPrivateOrLoopback())
            {
                return true;
            }
            return false;
        }
    }

}
