using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Microsoft.AspNetCore.Mvc.WnExtensions.@internal
{
   
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