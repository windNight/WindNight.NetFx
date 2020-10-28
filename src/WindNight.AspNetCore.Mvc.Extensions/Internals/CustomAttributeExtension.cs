using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Microsoft.AspNetCore.Mvc.WnExtensions.Internals
{
    internal static class CustomAttributeExtension
    {
        public static IEnumerable<T> GetMethodAttributes<T>(this ActionDescriptor actionDescriptor) where T : Attribute
        {
            if (actionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                return controllerActionDescriptor.MethodInfo.GetCustomAttributes<T>();

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

    internal static class ApiDescriptionExtensions
    {
        public static bool TryGetMethodInfo(
            this ApiDescription apiDescription,
            out MethodInfo methodInfo)
        {
            if (apiDescription.ActionDescriptor is ControllerActionDescriptor actionDescriptor)
            {
                methodInfo = actionDescriptor.MethodInfo;
                return true;
            }

            methodInfo = null;
            return false;
        }

        public static IEnumerable<object> CustomAttributes(this ApiDescription apiDescription)
        {
            if (apiDescription.TryGetMethodInfo(out var methodInfo))
                return methodInfo.GetCustomAttributes(true).Union(methodInfo.DeclaringType.GetCustomAttributes(true));
            return Enumerable.Empty<object>();
        }

        [Obsolete("Use TryGetMethodInfo() and CustomAttributes() instead")]
        public static void GetAdditionalMetadata(
            this ApiDescription apiDescription,
            out MethodInfo methodInfo,
            out IEnumerable<object> customAttributes)
        {
            if (apiDescription.TryGetMethodInfo(out methodInfo))
                customAttributes = methodInfo.GetCustomAttributes(true)
                    .Union(methodInfo.DeclaringType.GetCustomAttributes(true));
            else
                customAttributes = Enumerable.Empty<object>();
        }

        internal static string RelativePathSansQueryString(this ApiDescription apiDescription)
        {
            var relativePath = apiDescription.RelativePath;
            if (relativePath == null)
                return null;
            return relativePath.Split('?').First();
        }
    }
}