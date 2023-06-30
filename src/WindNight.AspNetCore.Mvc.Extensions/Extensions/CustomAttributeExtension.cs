using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace WindNight.AspNetCore.Mvc.Extensions.Extensions
{
    public static class CustomAttributeExtension
    {
        public static IEnumerable<T> GetMethodAttributes<T>(this ActionDescriptor actionDescriptor) where T : Attribute
        {
            if (actionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                return controllerActionDescriptor.MethodInfo.GetCustomAttributes<T>();

            return Enumerable.Empty<T>();
        }

        public static IEnumerable<T> GetControllerAndActionAttributes<T>(this ActionDescriptor actionDescriptor) where T : Attribute
        {
            if (actionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                var methodInfo = controllerActionDescriptor.MethodInfo;
                var attrs = methodInfo.GetCustomAttributes<T>();
                if (methodInfo.DeclaringType != null)
                    attrs = attrs.Concat(methodInfo.DeclaringType.GetCustomAttributes<T>());
                return attrs;
            }
            return Enumerable.Empty<T>();
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="apiDesc"></param>
        /// <returns></returns>
        public static IEnumerable<TAttribute> GetControllerAndActionAttributes<TAttribute>(this ApiDescription apiDesc)
            where TAttribute : Attribute
        {
            if (!apiDesc.TryGetMethodInfo(out var methodInfo)) return new List<TAttribute>();
            if (methodInfo == null) return new List<TAttribute>();
            var attrs = methodInfo.GetCustomAttributes<TAttribute>();
            if (methodInfo.DeclaringType != null)
                attrs = attrs.Concat(methodInfo.DeclaringType.GetCustomAttributes<TAttribute>());
            return attrs;
        }



    }

}
