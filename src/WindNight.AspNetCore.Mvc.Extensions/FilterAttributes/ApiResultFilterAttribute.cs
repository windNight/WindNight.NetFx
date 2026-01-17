using System.Collections.Concurrent;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.WnExtensions;
using Microsoft.AspNetCore.Mvc.WnExtensions.@internal;
using Newtonsoft.Json.Extension;
using WindNight.AspNetCore.Mvc.Extensions;
using WindNight.Core;
using WindNight.Core.Attributes.Abstractions;
using WindNight.Extension;

namespace Microsoft.AspNetCore.Mvc.Filters.Extensions
{
    /// <summary>
    ///     Provides Api result transform feature base http request header.
    /// </summary>
    public class ApiResultFilterAttribute : ResultFilterAttribute
    {
        private const int DefaultOrder = 10;
        private const int SuccessStatusCode = 200;
        private const string SystemErrorMessage = "系统错误00";

        public ApiResultFilterAttribute()
        {
            Order = 10;
        }

        public virtual int ExecOrder { get; set; } = 0;

        protected virtual void FixResultBeforeResultExecuting(ResultExecutingContext context)
        {
            try
            {
                // 检查是否跳过包装
                if (ShouldSkipWrapping(context))
                {
                    return;
                }

                // 过滤 FileResult，不进行包装
                if (context.Result is FileResult)
                {
                    return;
                }

                // 只处理成功响应
                if (context.HttpContext.Response.StatusCode != SuccessStatusCode)
                {
                    return;
                }

                // 检查并包装结果
                if (!TryWrapResult(context, out var wrappedResult))
                {
                    if (wrappedResult != null)
                    {
                        context.Result = new ObjectResult(wrappedResult);
                        return;
                    }

                    LogHelper.Warn(
                        $"api[{context?.HttpContext?.Request?.Path ?? ""}] Result Check Handler Error context.Result is {context.Result.ToJsonStr()} ");

                    // context.Result = new ObjectResult(new ResponseResult<object>().SystemError(SystemErrorMessage));
                    return;
                }

                context.Result = new ObjectResult(wrappedResult);
            }
            catch (Exception ex)
            {

            }
        }


        protected virtual void FixResultBeforeResultExecuting11(ResultExecutingContext context)
        {
            //var car = context.ActionDescriptor.GetMethodAttributes<ClearResultAttribute>().FirstOrDefault();
            try
            {
                var clsAttr = context.ActionDescriptor.GetAttributeOnAction<ClearResultAttribute>();

                var noClear = clsAttr?.IsClear ?? false;
                if (noClear)
                {
                    return;
                }

                if (context.HttpContext.Response.StatusCode != 200)
                {
                    return;
                }

                if (!(context.Result is ObjectResult objectResult) || (objectResult != null &&
                                                                       objectResult.Value != null &&
                                                                       objectResult.Value is ResponseResult))
                {
                    return;
                }


                if (objectResult == null || objectResult.Value == null || objectResult.DeclaredType == null)
                {
                    context.Result = new ObjectResult(new ResponseResult<object>().NotFound());
                }
                else
                {
                    try
                    {
                        //ResponseWrapper.WrapResponse(context, apiResult =>
                        //{

                        //    var ttl = 0L;
                        //    try
                        //    {
                        //        var beginTime = CurrentItem.GetItem<DateTime>(WebConst.BEGINTIME);
                        //        ttl = (long)(HardInfo.Now - beginTime).TotalMilliseconds;
                        //    }
                        //    catch (Exception ex)
                        //    {

                        //    }
                        //    ResponseWrapper.SetProperty(apiResult, "TTL", ttl);


                        //});

                        dynamic apiResult = Activator.CreateInstance(
                            typeof(ResponseResult<>).MakeGenericType(objectResult.DeclaredType), objectResult.Value);
                        var ttl = 0L;
                        try
                        {
                            var beginTime = CurrentItem.GetItem<DateTime>(WebConst.BEGINTIME);
                            ttl = (long)(HardInfo.Now - beginTime).TotalMilliseconds;
                        }
                        catch (Exception ex)
                        {
                        }

                        apiResult.TTL = ttl;
                        apiResult.ReqClientIp = context.HttpContext.GetClientIp();
                        context.Result = new ObjectResult(apiResult);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(
                            $"api[{context?.HttpContext?.Request?.Path ?? ""}] Result Check Handler Error {ex.Message}",
                            ex);
                        context.Result = new ObjectResult(new ResponseResult<object>().SystemError("系统错误1"));
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(
                    $"api[{context?.HttpContext?.Request?.Path ?? ""}] FixResultBeforeResultExecuting Handler Error {ex.Message}",
                    ex);
                context.Result = new ObjectResult(new ResponseResult<object>().SystemError("系统错误2"));
            }
        }


        private bool TryWrapResult(ResultExecutingContext context, out dynamic wrappedResult)
        {
            wrappedResult = null;


            if (!(context.Result is ObjectResult objectResult)) // || objectResult.Value is ResponseResult)
            {
                wrappedResult = context.Result;
                return false; // 已包装或非 ObjectResult
            }


            if (objectResult.Value == null)
            {
                wrappedResult = new ResponseResult<object>().NotFound();
            }
            else if (objectResult.Value is ResponseResult)
            {
                wrappedResult = objectResult.Value;
            }

            try
            {
                if (wrappedResult == null)
                {
                    // 使用泛型工厂创建包装（避免 Activator 的反射开销）
                    wrappedResult = CreateResponseWrapper(objectResult.Value, objectResult.DeclaredType);
                }

                // 设置额外属性
                SetAdditionalProperties(wrappedResult, context);
                return true;
            }
            catch (Exception ex)
            {
                LogError(context, $"Result 包装失败: {ex.Message}", ex);
                return false;
            }
        }

        private void LogError(ResultExecutingContext context, string message, Exception ex)
        {
            var path = context?.HttpContext?.Request?.Path ?? "Unknown";
            LogHelper.Error($"API [{path}] {message}", ex);
        }

        private void SetAdditionalProperties(dynamic apiResult, ResultExecutingContext context)
        {
            // 设置 TTL
            apiResult.TTL = CalculateTtl();
            apiResult.TraceId = FetchTraceId();

            // 设置客户端 IP
            apiResult.ReqClientIp = context.HttpContext.GetClientIp() ?? "Unknown";
        }

        private dynamic CreateResponseWrapper(object value, Type declaredType)
        {
            // 优化：使用预编译的泛型方法（通过 MethodInfo），避免每次 Activator
            var wrapperType = typeof(ResponseResult<>).MakeGenericType(declaredType ?? typeof(object));
            var constructor = wrapperType.GetConstructor(new[] { declaredType });
            if (constructor == null)
            {
                throw new InvalidOperationException($"ResponseResult<{declaredType?.Name ?? "object"}> 缺少构造函数");
            }

            // 使用 constructor.Invoke 替代 Activator（略微高效）
            return constructor.Invoke(new[] { value });
        }

        private long CalculateTtl()
        {
            try
            {
                var beginTime = CurrentItem.GetItem<DateTime>(WebConst.BEGINTIME);
                return (long)(HardInfo.Now - beginTime).TotalMilliseconds;
            }
            catch (Exception ex)
            {
                LogHelper.Warn($"TTL 计算失败: {ex.Message}"); // 使用 Warn 而非 Error，避免日志噪音
                return 0L;
            }
        }

        private string FetchTraceId()
        {
            try
            {
                return CurrentItem.GetSerialNumber;
            }
            catch (Exception ex)
            {
                return "";
            }
        }


        private bool ShouldSkipWrapping(ResultExecutingContext context)
        {
            var clearAttr = context.ActionDescriptor.GetAttribute<ClearResultAttribute>();
            return clearAttr?.IsClear == true;
        }


        #region override ResultFilterAttribute

        /// <summary>
        ///     Called when [result executing].
        /// </summary>
        /// <param name="context">The context.</param>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            try
            {
                FixResultBeforeResultExecuting(context);
            }
            catch
            {
            }

            base.OnResultExecuting(context);
        }

        #endregion
    }


    internal static class ResponseWrapper
    {
        // 缓存： 类型不够准确 泛型类型和创建委托的字典（返回 object，即 ResponseResult<T> 的 boxed 版本）
        private static readonly ConcurrentDictionary<Type, Func<object, object>> _creators = new();

        // 反射缓存：常见属性 setter（提升修改性能）
        private static readonly ConcurrentDictionary<string, Action<object, object>> _propertySetters = new();

        public static void WrapResponse(ResultExecutingContext context,
            Action<object>? propertyModifier = null, // 可选：反射式修改（简单场景）
            Func<object, object>? postProcessor = null) // 可选：委托式修改（需处理类型）
        {
            if (context.Result is not ObjectResult objectResult || objectResult.Value == null)
                return;

            var declaredType = objectResult.DeclaredType ?? typeof(object);

            // 性能优化：使用缓存的委托创建 ResponseResult<T> 实例（返回 object）
            var creator = _creators.GetOrAdd(declaredType, CreateInstanceDelegate);
            var apiResult = creator(objectResult.Value); // 类型：object (实际 ResponseResult<T>)

            // 步骤1: 反射式属性修改（示例：设置 Success/Message，根据条件）
            if (propertyModifier != null)
            {
                propertyModifier(apiResult);
            }

            // 默认修改示例（可自定义）：如基于异常设置 Message
            // SetProperty(apiResult, "Message", "默认成功消息");
            // SetProperty(apiResult, "Success", true);
            // 步骤2: 委托式修改（高级：调用者可 cast 到 ResponseResult<T>）
            if (postProcessor != null)
            {
                apiResult = postProcessor(apiResult);
            }

            context.Result = new ObjectResult(apiResult)
            {
                StatusCode = objectResult.StatusCode // 保留原始状态码
            };
        }

        // 辅助：反射设置属性（缓存 setter，避免每次 GetProperty）
        public static void SetProperty(object instance, string propertyName, object value)
        {
            var setter = _propertySetters.GetOrAdd(propertyName, name =>
            {
                var prop = instance.GetType().GetProperty(name)
                           ?? throw new InvalidOperationException($"ResponseResult 缺少 {name} 属性");
                var param = Expression.Parameter(typeof(object), "instance");
                var castParam = Expression.Convert(param, instance.GetType());
                var propAccess = Expression.Property(castParam, prop);
                var valueParam = Expression.Parameter(typeof(object), "value");
                var castValue = Expression.Convert(valueParam, prop.PropertyType);
                var assign = Expression.Assign(propAccess, castValue);
                var body = Expression.Block(assign, propAccess); // 返回以支持链式，但此处仅设
                return Expression.Lambda<Action<object, object>>(body, param, valueParam).Compile();
            });
            setter(instance, value);
        }

        private static Func<object, object> CreateInstanceDelegate(Type t)
        {
            // 使用表达式树编译委托：创建 ResponseResult<T>，返回 object
            var genericType = typeof(ResponseResult<>).MakeGenericType(t);
            var ctor = genericType.GetConstructor(new[] { t })
                       ?? throw new InvalidOperationException($"ResponseResult<{t.Name}> 缺少接受 {t.Name} 参数的构造函数");

            var param = Expression.Parameter(typeof(object), "value");
            var castParam = Expression.Convert(param, t);
            var body = Expression.New(ctor, castParam);
            var lambda = Expression.Lambda<Func<object, object>>(Expression.Convert(body, typeof(object)), param);

            return lambda.Compile();
        }
    }
}
