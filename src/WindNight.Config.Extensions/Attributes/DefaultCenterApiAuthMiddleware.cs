using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.AspNetCore.Mvc.Extensions;
using WindNight.Config.Abstractions;
using WindNight.Core.Extension;
using WindNight.Extension;

namespace Net8ApiDemo
{
    public class DefaultCenterApiAuthMiddleware
    {
        private readonly RequestDelegate _next;
        public DefaultCenterApiAuthMiddleware(RequestDelegate next)
        {
            _next = next;

        }


        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var CCAuth = Ioc.GetService<IConfigCenterAuth>();
                if (CCAuth != null)
                {
                    var isValid = CCAuth.ConfigCenterApiAuth();
                    if (!isValid)
                    {

                        context.Response.StatusCode = 404;
                        return;

                    }

                }


                var remoteIp = context.Request.HttpContext.GetClientIp();
                if (!remoteIp.IsPrivateOrLoopback())
                {
                    var ak = GetAccessToken(context);
                    var token = GetAppTokenValue(context);
                    if (ak.IsNullOrEmpty() && token.IsNullOrEmpty())
                    {
                        context.Response.StatusCode = 404;
                        return;

                    }
                }

                await _next(context);

            }
            catch (Exception ex)
            {
                await _next(context);
            }
        }

        string GetAccessToken(HttpContext context)
        {
            var authorizationValue = context.Request.GetAuthorizationValue();

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

        string GetAppTokenValue(HttpContext context)
        {
            return context.Request.GetAppTokenValue();
            ;
        }

    }
}
