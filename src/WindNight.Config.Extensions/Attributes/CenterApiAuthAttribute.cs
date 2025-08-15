using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.AspNetCore.Mvc.Extensions;
using WindNight.Config.Abstractions;
using WindNight.Core;
using WindNight.Core.Extension;
using WindNight.Extension;

namespace WindNight.Config.Extensions.Attributes
{
    internal class CenterApiAuthAttribute : ActionFilterAttribute
    {

        public CenterApiAuthAttribute()
        {
            Order = 0;
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var CCAuth = Ioc.GetService<IConfigCenterAuth>();
            if (CCAuth != null)
            {
                var isValid = CCAuth.ConfigCenterApiAuth();
                if (!isValid)
                {

                    // context.HttpContext.Response.StatusCode = 404;
                    context.Result = new ObjectResult(ResponseResult.GenNotFoundRes(null));
                    return;

                }

            }

            var remoteIp = context.HttpContext.GetClientIp();
            if (!remoteIp.IsInternalIp())
            {
                var ak = GetAccessToken(context.HttpContext);
                var token = GetAppTokenValue(context.HttpContext);
                if (ak.IsNullOrEmpty() && token.IsNullOrEmpty())
                {
                    // context.HttpContext.Response.StatusCode = 404;
                    context.Result = new ObjectResult(ResponseResult.GenNotFoundRes(null));
                    return;

                }
            }

            base.OnActionExecuting(context);


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
