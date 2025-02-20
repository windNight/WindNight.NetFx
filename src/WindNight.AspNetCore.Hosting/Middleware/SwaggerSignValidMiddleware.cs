using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.WnExtensions.Abstractions.Attributes;
using Swashbuckle.AspNetCore.HideApi.Middleware;
using WindNight.AspNetCore.Mvc.Extensions.Extensions;
using WindNight.Linq.Extensions.Expressions;

namespace WindNight.AspNetCore.Hosting.Middleware
{
    public abstract class WindSwaggerSignValidMiddleware : SwaggerSignValidMiddleware
    {

        protected WindSwaggerSignValidMiddleware(RequestDelegate next, Dictionary<string, string> signKeyDict) : base(next, signKeyDict)
        {

        }


        /// <summary>
        ///  需要额外自行实现
        ///     检查当前请求是否标记了 <see cref="NonAuthAttribute" />  属性。
        ///     如果标记了 <see cref="NonAuthAttribute" />，则跳过验证。
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool HasNonAuth(HttpContext context)
        {
            // 获取当前请求的控制器和方法
            var endpoint = context.GetEndpoint();
            if (endpoint == null)
            {
                return true;
            }

            // 检查控制器或方法是否标记了 [NonAuth] 属性
            var controllerActionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();

            if (controllerActionDescriptor != null)
            {
                var nonAuthAttrs = controllerActionDescriptor.GetControllerAndActionAttributes<NonAuthAttribute>().OfType<NonAuthAttribute>().ToList();
                if (!nonAuthAttrs.IsNullOrEmpty())
                {
                    return true;
                }

                return false;

            }

            return false;

        }


    }

}
