using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.WnExtensions.@internal;
using WindNight.Core.Attributes.Abstractions;

namespace WindNight.AspNetCore.Mvc.Extensions
{
    public static partial class CustomAttributeExtension
    {
        //  private static readonly ConcurrentDictionary<string, object> _asmCache = new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttr"></typeparam>
        /// <param name="actionDescriptor"></param>
        /// <returns></returns>
        public static TAttr? GetAttributeOnAction<TAttr>(this ActionDescriptor actionDescriptor)
            where TAttr : Attribute, IAttribute
        {

            return actionDescriptor.GetAttributesOnAction<TAttr>()?.LastOrDefault() ?? null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttr"></typeparam>
        /// <param name="actionDescriptor"></param>
        /// <returns></returns>
        public static IEnumerable<TAttr> GetMethodAttributes<TAttr>(this ActionDescriptor actionDescriptor)
            where TAttr : Attribute, IAttribute
        {
            return actionDescriptor.GetAttributesOnAction<TAttr>();
            //if (actionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            //{
            //    return controllerActionDescriptor.MethodInfo.GetCustomAttributes<TAttr>();
            //}

            //return Empty<TAttr>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttr"></typeparam>
        /// <param name="actionDescriptor"></param>
        /// <returns></returns>
        public static IEnumerable<TAttr> GetActionAttributes<TAttr>(this ActionDescriptor actionDescriptor)
            where TAttr : Attribute, IAttribute
        {

            return actionDescriptor.GetAttributesOnAction<TAttr>();
            //if (actionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            //{
            //    return controllerActionDescriptor.MethodInfo.GetCustomAttributes<TAttr>();
            //}

            //return Empty<TAttr>();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttr"></typeparam>
        /// <param name="actionDescriptor"></param>
        /// <returns></returns>
        public static IEnumerable<TAttr> GetControllerAndActionAttributes<TAttr>(this ActionDescriptor actionDescriptor)
            where TAttr : Attribute, IAttribute
        {

            return actionDescriptor.GetAttributesOnControllerAndAction<TAttr>();
            //if (actionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            //{
            //    var methodInfo = controllerActionDescriptor.MethodInfo;
            //    var attrs = methodInfo.GetCustomAttributes<TAttr>();
            //    if (methodInfo.DeclaringType != null)
            //    {
            //        attrs = attrs.Concat(methodInfo.DeclaringType.GetCustomAttributes<TAttr>());
            //    }
            //    return attrs;
            //}
            //return Empty<TAttr>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttr"></typeparam>
        /// <param name="actionDescriptor"></param>
        /// <returns></returns>
        public static IEnumerable<TAttr> GetAttributesOnControllerAndAction<TAttr>(this ActionDescriptor actionDescriptor)
            where TAttr : Attribute, IAttribute
        {
            try
            {
                if (actionDescriptor == null)
                {
                    return Empty<TAttr>();
                }

                // var key = $"{actionDescriptor.GetType().FullName}_{typeof(TAttr).FullName}";
                //   var attrs = (IEnumerable<TAttr>)_asmCache.GetOrAdd(key, k =>
                //{
                if (actionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                {
                    var methodInfo = controllerActionDescriptor.MethodInfo;
                    var attrs = methodInfo.GetCustomAttributes<TAttr>();
                    if (methodInfo.DeclaringType != null)
                    {
                        attrs = attrs.Concat(methodInfo.DeclaringType.GetCustomAttributes<TAttr>());
                    }
                    return attrs;
                }
                return Empty<TAttr>();
                // });

                //return attrs;
            }
            catch (Exception ex)
            {
                return Empty<TAttr>();

            }


        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TAttr"></typeparam>
        /// <param name="apiDesc"></param>
        /// <returns></returns>
        public static TAttr? GetAttributeOnControllerAndAction<TAttr>(this ActionDescriptor actionDescriptor)
            where TAttr : Attribute, IAttribute
        {
            return actionDescriptor.GetControllerAndActionAttributes<TAttr>()?.LastOrDefault() ?? null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttr"></typeparam>
        /// <param name="actionDescriptor"></param>
        /// <returns></returns>
        public static IEnumerable<TAttr> GetAttributesOnAction<TAttr>(this ActionDescriptor actionDescriptor)
            where TAttr : Attribute, IAttribute
        {
            try
            {
                //var key = $"{actionDescriptor.GetType().FullName}_{typeof(TAttr).FullName}";

                // var attrs = (IEnumerable<TAttr>)_asmCache.GetOrAdd(key, k =>
                // {
                if (actionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                {
                    return controllerActionDescriptor.MethodInfo.GetCustomAttributes<TAttr>();
                }

                return Empty<TAttr>();
                //  });

                // return attrs;

            }
            catch (Exception ex)
            {
                return Empty<TAttr>();

            }

        }

    }


    public static partial class CustomAttributeExtension
    {
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
        public static IEnumerable<TAttr> GetControllerAndActionAttributes<TAttr>(this ApiDescription apiDesc)
            where TAttr : Attribute, IAttribute
        {
            return apiDesc.GetAttributesOnControllerAndAction<TAttr>();

            //if (!apiDesc.TryGetMethodInfo(out var methodInfo) || methodInfo == null)
            //{
            //    return Empty<TAttr>();
            //}

            ////if (methodInfo == null)
            ////{
            ////    return Enumerable.Empty<TAttr>();
            ////}

            //var attrs = methodInfo.GetCustomAttributes<TAttr>();

            //if (methodInfo.DeclaringType != null)
            //{
            //    attrs = attrs.Concat(methodInfo.DeclaringType.GetCustomAttributes<TAttr>());
            //}

            //return attrs;
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

        /// <summary>
        /// </summary>
        /// <typeparam name="TAttr"></typeparam>
        /// <param name="apiDesc"></param>
        /// <returns></returns>
        public static IEnumerable<TAttr> GetAttributesOnControllerAndAction<TAttr>(this ApiDescription apiDesc)
            where TAttr : Attribute, IAttribute
        {
            try
            {
                //  var key = $"{apiDesc.GetType().FullName}_{typeof(TAttr).FullName}";

                // var attrs = (IEnumerable<TAttr>)_asmCache.GetOrAdd(key, k =>
                // {
                if (!apiDesc.TryGetMethodInfo(out var methodInfo) || methodInfo == null)
                {
                    return Empty<TAttr>();
                }


                var attrs = methodInfo.GetCustomAttributes<TAttr>();

                if (methodInfo.DeclaringType != null)
                {
                    attrs = attrs.Concat(methodInfo.DeclaringType.GetCustomAttributes<TAttr>());
                }

                return attrs;
                // });

                // return attrs;

            }
            catch (Exception ex)
            {
                return Empty<TAttr>();

            }


        }

    }

}
