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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttr"></typeparam>
        /// <param name="actionDescriptor"></param>
        /// <returns></returns>
        public static TAttr GetAttributeOnAction<TAttr>(this ActionDescriptor actionDescriptor)
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
        public static TAttr GetAttributeOnControllerAndAction<TAttr>(this ActionDescriptor actionDescriptor)
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

                if (actionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                {
                    return controllerActionDescriptor.MethodInfo.GetCustomAttributes<TAttr>();
                }

                return Empty<TAttr>();

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
            return HardInfo.EmptyList<TAttr>();
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
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TAttr"></typeparam>
        /// <param name="apiDesc"></param>
        /// <returns></returns>
        public static TAttr GetAttributeOnControllerAndAction<TAttr>(this ApiDescription apiDesc)
            where TAttr : Attribute, IAttribute
        {
            return apiDesc.GetAttributesOnControllerAndAction<TAttr>()?.FirstOrDefault() ?? null;
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


            }
            catch (Exception ex)
            {
                return Empty<TAttr>();

            }


        }

    }

}
