using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;
using WindNight.Core.Attributes.Abstractions;

namespace Swashbuckle.AspNetCore.Extensions.@internal
{
    internal static class CustomAttributeExtension
    {
        // private static readonly ConcurrentDictionary<string, object> _asmCache = new();

        /// <summary>
        /// </summary>
        /// <typeparam name="TAttr"></typeparam>
        /// <param name="apiDesc"></param>
        /// <returns></returns>
        public static IEnumerable<TAttr> GetControllerAndActionAttributes<TAttr>(this ApiDescription apiDesc)
            where TAttr : Attribute, IAttribute
        {
            try
            {
                if (apiDesc == null)
                {
                    return Empty<TAttr>();
                }

                var key = $"{apiDesc.GetType().FullName}_{typeof(TAttr).FullName}";
                // var attrs = (IEnumerable<TAttr>)_asmCache.GetOrAdd(key, k =>
                //   {
                if (!apiDesc.TryGetMethodInfo(out var methodInfo))
                {
                    return Empty<TAttr>();
                }

                if (methodInfo == null)
                {
                    return Empty<TAttr>();
                }
                var attrs = methodInfo.GetCustomAttributes<TAttr>();
                if (methodInfo.DeclaringType != null)
                {
                    attrs = attrs.Concat(methodInfo.DeclaringType.GetCustomAttributes<TAttr>());
                }

                return attrs;
                //  });

                // return attrs;
            }
            catch (Exception ex)
            {
                return Empty<TAttr>();

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttr"></typeparam>
        /// <param name="apiDesc"></param>
        /// <returns></returns>
        public static IEnumerable<TAttr> GetAttributesOnControllerAndAction<TAttr>(this ApiDescription apiDesc)
            where TAttr : Attribute, IAttribute
        {
            try
            {
                var attributes = apiDesc.GetControllerAndActionAttributes<TAttr>()
                    .OfType<TAttr>().ToList();
                return attributes;
            }
            catch (Exception ex)
            {
                return Empty<TAttr>();
            }
        }
        static IEnumerable<TAttr> Empty<TAttr>()
            where TAttr : Attribute, IAttribute
        {
            return Enumerable.Empty<TAttr>();
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TAttr"></typeparam>
        /// <param name="apiDesc"></param>
        /// <returns></returns>
        public static TAttr? GetAttributeOnControllerAndAction<TAttr>(this ApiDescription apiDesc)
            where TAttr : Attribute, IAttribute
        {
            return apiDesc.GetAttributesOnControllerAndAction<TAttr>()?.LastOrDefault() ?? null;
        }




    }
}
