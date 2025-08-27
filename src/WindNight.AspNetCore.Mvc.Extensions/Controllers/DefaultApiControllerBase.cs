using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.WnExtensions.@internal;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.AspNetCore.Mvc.Extensions;
using WindNight.Core;
using WindNight.Core.Abstractions;
using WindNight.Core.Extension;
using WindNight.Extension;

namespace Microsoft.AspNetCore.Mvc.WnExtensions.Controllers
{
    public class DefaultApiControllerBase : ControllerBase
    {
        protected static ISysApiAuthCheck SysApiAuthCheckImpl => Ioc.GetService<ISysApiAuthCheck>();

        protected virtual bool OpenDebug => ConfigItems.OpenDebug;

        protected virtual string SysAppId => HardInfo.AppId;

        protected virtual string SysAppCode => HardInfo.AppCode;

        protected virtual string SysAppName => HardInfo.AppName;

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

        protected virtual bool IsAuthType1(bool ignoreIp = true)
        {

            if (SysApiAuthCheckImpl != null)
            {
                if (SysApiAuthCheckImpl.OpenSysApiAuthCheck)
                {
                    var isValid = SysApiAuthCheckImpl.SysApiAuth();
                    if (!isValid)
                    {
                        Response.StatusCode = 404; //new ObjectResult(ResponseResult.GenNotFoundRes(null));
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

            }

            if (ignoreIp && HttpClientIpIsPrivate())
            {
                return true;
            }

            var ak = GetAccessToken();

            var appToken = GetAppTokenValue();

            if (ak.IsNullOrEmpty(true) && appToken.IsNullOrEmpty(true))
            {
                return false;
            }

            return true;
        }


        protected virtual string GetUserAgentValue() => Request.GetUserAgentValue();

        protected virtual string GetAppTokenValue() => Request.GetAppTokenValue();

        protected virtual string GetAuthorizationValue() => Request.GetAuthorizationValue();

        protected virtual string GetRequestAppCodeValue() => Request.GetAppCodeValue();

        protected virtual string GetRequestAppNameValue() => Request.GetAppNameValue();

        protected virtual string GetAppEnvNameValue() => Request.GetAppEnvNameValue();

        protected virtual long GetRequestTsValue() => Request.GetTimestampValue();


        protected virtual string QueryHeaderValue(string key, string defaultValue = "")
        {
            var value = Request.QueryHeaderValue(key, defaultValue);
            if (!value.IsNullOrEmpty())
            {
                value = value.UrlDecode();
            }
            return value;
        }

        protected virtual string QueryValueInHeader(string key, string defaultValue = "")
        {
            return QueryHeaderValue(key);
        }


        protected virtual bool QueryValueInHeader(string key, bool defaultValue = false)
        {
            var value = QueryHeaderValue(key, "");

            return value.ToBoolean(defaultValue);

        }


        protected virtual int QueryValueInHeader(string key, int defaultValue = 0)
        {
            var value = QueryHeaderValue(key);

            return value.ToInt(defaultValue);
        }

        protected virtual long QueryValueInHeader(string key, long defaultValue = 0L)
        {
            var value = QueryHeaderValue(key);

            return value.ToLong(defaultValue);
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

            if (clientIp.IsInternalIp())
            {
                return true;
            }

            return false;

        }

        protected virtual bool RequestWithInternalNet => HttpClientIpIsPrivate();


    }

}
