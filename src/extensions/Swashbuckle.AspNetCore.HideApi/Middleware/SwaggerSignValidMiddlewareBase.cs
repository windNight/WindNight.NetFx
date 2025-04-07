using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Extensions.@internal;
using Swashbuckle.AspNetCore.HideApi.@internal;

namespace Swashbuckle.AspNetCore.HideApi.Middleware
{
    //[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    //public class NonAuthAttribute : Attribute
    //{
    //    public NonAuthAttribute(bool noauth = true)
    //    {
    //        NoAuth = noauth;
    //    }

    //    public bool NoAuth { get; }
    //}


    internal class InternalSwaggerMiddlewareBase
    {
        private readonly RequestDelegate _next;
        public InternalSwaggerMiddlewareBase(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // if (context.Request.Path.StartsWithSegments("/swagger"))
                if (context.IsInSwaggerPage())
                {
                    // 获取客户端IP地址
                    // var remoteIp = context.Request.HttpContext.Connection.RemoteIpAddress;
                    var remoteIp = context.Request.HttpContext.QueryDefaultClient();
                    if (!remoteIp.IpValid())
                    {
                        context.Response.StatusCode = 404;
                        return;
                    }

                    if (ConfigItems.IsOnline && !ConfigItems.SwaggerOnlineDebug)
                    {
                        context.Response.StatusCode = 404;
                        return;
                    }

                    if (ConfigItems.HiddenSwagger)
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

    }
    public abstract partial class SwaggerSignValidMiddlewareBase
    {
        protected readonly RequestDelegate _next;

        public SwaggerSignValidMiddlewareBase(RequestDelegate next)
        {
            _next = next;
        }


        public SwaggerSignValidMiddlewareBase(RequestDelegate next, Dictionary<string, string> signKeyDict)
        {
            _next = next;
            _signKeyDict = signKeyDict ?? new Dictionary<string, string>();
        }

        protected Dictionary<string, string> _signKeyDict { get; }

        private List<SwaggerSignConfig> SignConfigs => ConfigItems.SwaggerSignConfigs;

        protected virtual bool SwaggerCanDebug => ConfigItems.SwaggerCanDebug;

        protected virtual Dictionary<string, string> DefaultSignDict { get; } = new();

        protected virtual Dictionary<string, string> CurrentSignKeyDict
        {
            get
            {
                try
                {
                    if (!DefaultSignDict.IsNullOrEmpty())
                    {
                        return DefaultSignDict;
                    }

                    if (!_signKeyDict.IsNullOrEmpty())
                    {
                        return _signKeyDict;
                    }

                    var signKeyDict = ConfigItems.SwaggerConfigs.GetSignDict();
                    return signKeyDict;
                }
                catch (Exception ex)
                {
                    return new Dictionary<string, string>();
                }
            }
        }

        protected virtual bool VerifyClientIp(HttpContext context)
        {
            return context.RemoteIpValid();
            //  var remoteIp = context.Request.HttpContext.QueryDefaultClient();
            //  return remoteIp.IpValid();
        }

        protected virtual List<string> AllowedPaths => new() { "/api" };

        protected abstract bool CheckValidData(HttpContext context, Dictionary<string, string> dict);


        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await DoInvokeAsync(context);
            }
            catch (Exception ex)
            {
                await _next(context);
            }

        }

        protected virtual async Task DoInvokeAsync(HttpContext context)
        {

            // 判断是否来自 Swagger 页面的请求
            var isSwaggerPage = IsInSwaggerPage(context);
            if (!isSwaggerPage)
            {
                await _next(context);
                return;
            }

            if (!IsPathAllowed(context.Request.Path))
            {
                await _next(context);
                return;
            }

            if (!SwaggerCanDebug)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid or missing signature");
                return;
            }




            // 执行验证逻辑
            var isAuth = await DoCheckAsync(context);
            if (isAuth)
            {
                await _next(context);
                return;
            }

            // 如果验证失败，确保不会重复发送响应
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid or missing signature");
            }
        }

        protected virtual bool IsPathAllowed(PathString path)
        {
            return AllowedPaths.Any(p => path.StartsWithSegments(p));
        }

        protected virtual bool IsInSwaggerPage(HttpContext context)
        {
            return context.IsInSwaggerPage();
            //var refer = GetHeaderData(context.Request, "Referer");
            //if (refer.IsNullOrEmpty())
            //{
            //    return false;
            //}

            //if (!refer.Contains("swagger", StringComparison.OrdinalIgnoreCase))
            //{
            //    return false;
            //}

            //return true;
        }

        /// <summary>
        ///     需要额外自行实现
        ///     检查当前请求是否标记了 <see cref="NonAuthAttribute" />  属性。
        ///     如果标记了 <see cref="NonAuthAttribute" />，则跳过验证。
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual bool HasNonAuth(HttpContext context)
        {
            //// 获取当前请求的控制器和方法
            //var endpoint = context.GetEndpoint();
            //if (endpoint == null)
            //{
            //    return true;
            //}

            //// 检查控制器或方法是否标记了 [NonAuth] 属性
            //var controllerActionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
            //if (controllerActionDescriptor != null)
            //{
            //    var nonAuthAttrs = controllerActionDescriptor.GetControllerAndActionAttributes<NonAuthAttribute>().OfType<NonAuthAttribute>().ToList();
            //    if (!nonAuthAttrs.IsNullOrEmpty())
            //    {
            //        return true;
            //    }

            //    return false;

            //}

            return false;
        }

        protected virtual void InternalTodo(HttpContext context)
        {
            if (context.RemoteIpValid())
            {
                var defaultToken = GetHeaderData(context.Request, "AppToken");
                if (defaultToken.IsNullOrEmpty())
                {
                    context.Request.Headers["AppToken"] = $"Token_swagger_{HardInfo.NowUnixTime}";
                }
            }
        }

        /// <summary>
        ///     获取签名相关的数据。
        ///     如果某些头（如 Ts 或 Timestamp）未提供，则自动填充当前时间戳。
        /// </summary>
        protected virtual Dictionary<string, string> TryGetValidData(HttpContext context)
        {
            var signData = new Dictionary<string, string>();
            var signKeyDict = CurrentSignKeyDict;

            if (!signKeyDict.IsNullOrEmpty())
            {
                foreach (var item in signKeyDict)
                {
                    var key = item.Key;
                    var data = GetHeaderData(context.Request, key);
                    if (data.IsNullOrEmpty())
                    {
                        if (key == "Ts")
                        {
                            data = ConvertToUnixTime(DateTime.Now).ToString();
                            context.Request.Headers["Ts"] = data;
                        }

                        if (key == "Timestamp")
                        {
                            data = ConvertToUnixTime(DateTime.Now).ToString();
                            context.Request.Headers["Timestamp"] = data;
                        }
                    }

                    signData.Add(key, data);
                }
            }

            return signData;
        }

        protected virtual async Task<bool> DoCheckAsync(HttpContext context)
        {
            try
            {
                if (IsPathAllowed(context.Request.Path))
                {
                    if (!VerifyClientIp(context))
                    {
                        return await Task.FromResult(false);
                    }
                    else
                    {
                        InternalTodo(context);
                    }

                    // 获取当前请求的控制器和方法
                    var endpoint = context.GetEndpoint();
                    if (endpoint == null)
                    {
                        return await Task.FromResult(true);
                    }

                    var hasNonAuth = HasNonAuth(context);
                    if (hasNonAuth)
                    {
                        return await Task.FromResult(true);
                    }


                    if (!CurrentSignKeyDict.IsNullOrEmpty())
                    {
                        var signData = TryGetValidData(context);

                        var flag = CheckValidData(context, signData);
                        return await Task.FromResult(flag);
                    }



                }

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        protected virtual string GetHeaderData(HttpRequest httpRequest, string headerName, string defaultValue = "")
        {
            if (httpRequest.Headers.TryGetValue(headerName, out var requestHeader))
            {
                var header = requestHeader.FirstOrDefault();
                if (!header.IsNullOrEmpty())
                {
                    return header.Trim();
                }
            }

            return defaultValue;
        }

        protected virtual long ConvertToUnixTime(DateTime dateTime, bool milliseconds = true)
        {
            var dateTimeOffset = new DateTimeOffset(dateTime);
            if (milliseconds)
            {
                return dateTimeOffset.ToUnixTimeMilliseconds();
            }

            return dateTimeOffset.ToUnixTimeSeconds();
        }
    }
}
