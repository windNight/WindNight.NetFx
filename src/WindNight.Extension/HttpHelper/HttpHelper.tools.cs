using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Newtonsoft.Json.Extension;
using RestSharp;
using WindNight.Core;
using WindNight.Extension.@internal;
using WindNight.LogExtension;

namespace WindNight.Extension
{
    public static partial class HttpHelper
    {
        public static Dictionary<string, object> GenQueryDict(this object queries)
        {
            var queryDict = new Dictionary<string, object>();
            try
            {
                if (queries == null)
                    return queryDict;

                var properties = queries.GetType().GetProperties(
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);

                foreach (var property in properties)
                {
                    try
                    {
                        var key = property.Name;
                        var value = property.GetValue(queries);
                        if (value != null && !queryDict.ContainsKey(key))
                        {
                            queryDict.Add(key, value);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(
                            $"GenQueryDict({property.Name},{property.ToJsonStr()}) Handler Error {ex.Message}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error($"GenQueryDict({queries.ToJsonStr()}) Handler Error {ex.Message}", ex);
            }

            return queryDict;
        }


        private static Dictionary<string, string> GeneratorHeaderDict(Dictionary<string, string> headerDict = null)
        {
            if (headerDict == null)
            {
                if (CurrentItem.Items != null)
                {
                    headerDict = new Dictionary<string, string>
                    {
                        { Consts.SERIZLNUMBER, CurrentItem.GetSerialNumber },

                    };
                }
            }
            else if (!headerDict.ContainsKey(Consts.SERIZLNUMBER))
            {
                if (CurrentItem.Items != null)
                {
                    headerDict.Add(Consts.SERIZLNUMBER, CurrentItem.GetSerialNumber);
                }
            }

            return headerDict ?? new Dictionary<string, string>();
        }

        private static Func<string, T> DefaultConvertFunc<T>() => _ => _.To<T>();

        private static Func<string, T> DefaultResConvertFunc<T>()
        {
            return s =>
            {
                var res = s.To<ResponseResult<T>>();
                if (res == null)
                {
                    return default;
                }

                if (res.Code == 0)
                {
                    return res.Data;
                }

                LogHelper.Warn($"Code({res.Code}) res is {res.ToJsonStr()}");
                return default;
            };
        }

        private static Func<string, IEnumerable<T>> DefaultEnumerableResConvertFunc<T>() // => _ => _.To<ResponseResult<IEnumerable<T>>>()?.Data ?? Array.Empty<T>();
        {
            return s =>
            {
                var res = s.To<ResponseResult<IEnumerable<T>>>(); //?.Data ?? Array.Empty<T>();
                if (res == null)
                {
                    return EmptyArray<T>();
                }

                if (res.Code == 0)
                {
                    return res.Data;
                }

                LogHelper.Warn($"Code({res.Code}) res is {res.ToJsonStr()}");

                return EmptyArray<T>();
            };
        }


        private static Func<string, IPagedList<T>> DefaultPagedConvertFunc<T>() //=> _ => _.To<ResponseResult<PagedList<T>>>()?.Data ?? PagedList.Empty<T>();
        {
            return s =>
            {
                var res = s.To<ResponseResult<PagedList<T>>>(); //?.Data ?? Array.Empty<T>();
                if (res == null)
                {
                    return PagedList.Empty<T>();
                }

                if (res.Code == 0)
                {
                    return res.Data;
                }

                LogHelper.Warn($"Code({res.Code}) res is {res.ToJsonStr()}");
                return PagedList.Empty<T>();
            };
        }

        //{
        //    return _ => _.To<T>();
        //}

        static string GenReqUrl(this IRestResponse response, string domain)
        {
            try
            {
                domain = domain.TrimEnd('/');
                return $"[{response.Request.Method}]({domain}/{response?.Request.Resource})";
            }
            catch (Exception ex)
            {
                return $"domain({domain})";
            }
        }

        public static T DeserializeResponse<T>(this IRestResponse response, Func<string, T> convertFunc, string domain, T defaultValue = default, Func<IRestResponse, bool> errStatusFunc = null)
        {
            var reqUrl = response.GenReqUrl(domain);
            try
            {

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //if (!response?.ResponseUri?.ToString()?.IsNullOrEmpty() ?? false)
                    //{
                    //    domain = response.ResponseUri.ToString();
                    //}

                    if (ConfigItems.DebugIsOpen)
                    {
                        LogHelper.Debug($"{reqUrl}-> response.Content is {response.Content} ", appendMessage: false);
                    }

                    convertFunc ??= DefaultConvertFunc<T>();
                    var r = convertFunc.Invoke(response.Content);
                    return r;
                }

                if (errStatusFunc != null)
                {
                    errStatusFunc.Invoke(response);
                    return defaultValue;
                }

                Trace.WriteLine($"{reqUrl}");

                LogHelper.Warn($"{reqUrl}->  ResponseStatus is {response.ResponseStatus} {response.ErrorMessage} {response.StatusDescription}",
                    appendMessage: false);

                return defaultValue;

            }
            catch (Exception ex)
            {
                LogHelper.Error($"{reqUrl}->  DeserializeResponse Handler Error {ex.Message}", ex);
                return defaultValue;
            }
        }


        public static T DeserializeResponse<T>(this IRestResponse response, string domain, Func<IRestResponse, bool> errStatusFunc = null)
        {
            return response.DeserializeResponse<T>(DefaultConvertFunc<T>(), domain, errStatusFunc: errStatusFunc);
        }


        public static IPagedList<T> DeserializeResponse2PageList<T>(this IRestResponse response, string domain, Func<IRestResponse, bool> errStatusFunc = null)
        {
            var defaultValue = PagedList.Empty<T>();
            var reqUrl = response.GenReqUrl(domain);
            try
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //if (!response?.ResponseUri?.ToString()?.IsNullOrEmpty() ?? false)
                    //{
                    //    url = response.ResponseUri.ToString();
                    //}

                    if (ConfigItems.DebugIsOpen)
                        LogHelper.Debug($"{reqUrl}-> response.Content is {response.Content} ", appendMessage: false);
                    var res = response.Content.To<ResponseResult<PagedList<T>>>();
                    return res.Data;
                }

                if (errStatusFunc != null)
                {
                    errStatusFunc.Invoke(response);
                    return defaultValue;
                }

                LogHelper.Warn(
                    $" {reqUrl}-> ResponseStatus is {response.ResponseStatus} {response.ErrorMessage} {response.StatusDescription}",
                    appendMessage: false);
            }
            catch (Exception ex)
            {
                LogHelper.Error($"{reqUrl}->  DeserializeResponse Handler Error {ex.Message}", ex);
            }

            return defaultValue;
        }


        public static IPagedList<T> DeserializePageListResponse<T>(this IRestResponse response, string domain, Func<IRestResponse, bool> errStatusFunc = null)
        {
            return response.DeserializePageListResponse(DefaultPagedConvertFunc<T>(), domain, errStatusFunc);
        }


        public static IPagedList<T> DeserializePageListResponse<T>(this IRestResponse response, Func<string, IPagedList<T>> convertFunc, string domain, Func<IRestResponse, bool> errStatusFunc = null)
        {
            var defaultValue = PagedList.Empty<T>();
            var reqUrl = response.GenReqUrl(domain);
            try
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //if (!response?.ResponseUri?.ToString()?.IsNullOrEmpty() ?? false)
                    //{
                    //    url = response.ResponseUri.ToString();
                    //}

                    if (ConfigItems.DebugIsOpen)
                    {
                        LogHelper.Debug($"{reqUrl}-> response.Content is {response.Content} ",
                            appendMessage: false);
                    }

                    convertFunc ??= DefaultPagedConvertFunc<T>();
                    return convertFunc.Invoke(response.Content);
                }

                if (errStatusFunc != null)
                {
                    errStatusFunc.Invoke(response);
                    return defaultValue;
                }

                LogHelper.Warn(
                    $" {reqUrl}->  ResponseStatus is {response.ResponseStatus} {response.ErrorMessage} {response.StatusDescription}",
                    appendMessage: false);
            }
            catch (Exception ex)
            {
                LogHelper.Error($"{reqUrl}-> DeserializeResponse Handler Error {ex.Message}", ex);
            }

            return defaultValue;
        }

        public static IEnumerable<T> DeserializeListResponse<T>(this IRestResponse response, string domain, Func<IRestResponse, bool> errStatusFunc = null)
        {
            return response.DeserializeListResponse(DefaultEnumerableResConvertFunc<T>(), domain, errStatusFunc);
        }

        private static IEnumerable<T> EmptyArray<T>()
        {
#if NET45
            return new List<T>();
#else
            return Array.Empty<T>();
#endif
        }

        public static IEnumerable<T> DeserializeListResponse<T>(this IRestResponse response, Func<string, IEnumerable<T>> convertFunc, string domain, Func<IRestResponse, bool> errStatusFunc = null)
        {
            var defaultValue = EmptyArray<T>();
            var reqUrl = response.GenReqUrl(domain);
            try
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //if (!response?.ResponseUri?.ToString()?.IsNullOrEmpty() ?? false)
                    //{
                    //    domain = response.ResponseUri.ToString();
                    //}

                    if (ConfigItems.DebugIsOpen)
                    {
                        LogHelper.Debug($"{reqUrl}-> response.Content is {response.Content} ", appendMessage: false);
                    }

                    convertFunc ??= DefaultEnumerableResConvertFunc<T>();
                    return convertFunc.Invoke(response.Content);
                }

                if (errStatusFunc != null)
                {
                    errStatusFunc.Invoke(response);
                    return defaultValue;
                }

                LogHelper.Warn(
                    $"{reqUrl}-> ResponseStatus is {response.ResponseStatus} {response.ErrorMessage} {response.StatusDescription}",
                    appendMessage: false);
            }
            catch (Exception ex)
            {
                LogHelper.Error($"{reqUrl}-> DeserializeResponse Handler Error {ex.Message}", ex);
            }

            return defaultValue;
        }

        public static T DeserializeResResponse<T>(this IRestResponse response, string domain, T defaultValue = default, Func<IRestResponse, bool> errStatusFunc = null)
        {
            return response.DeserializeResResponse(DefaultResConvertFunc<T>(), domain, defaultValue, errStatusFunc);
        }

        public static T DeserializeResResponse<T>(this IRestResponse response, Func<string, T> convertFunc, string domain, T defaultValue = default, Func<IRestResponse, bool> errStatusFunc = null)
        {
            var reqUrl = response.GenReqUrl(domain);
            try
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //if (!response?.ResponseUri?.ToString()?.IsNullOrEmpty() ?? false)
                    //{
                    //    domain = response.ResponseUri.ToString();
                    //}

                    if (ConfigItems.DebugIsOpen)
                    {
                        LogHelper.Debug($"{reqUrl}->  response.Content is {response.Content} ", appendMessage: false);
                    }

                    convertFunc ??= DefaultResConvertFunc<T>();
                    return convertFunc.Invoke(response.Content);
                }

                if (errStatusFunc != null)
                {
                    errStatusFunc.Invoke(response);
                    return defaultValue;
                }

                LogHelper.Warn(
                    $"{reqUrl}-> ResponseStatus is {response.ResponseStatus} {response.ErrorMessage} {response.StatusDescription}",
                    appendMessage: false);
            }
            catch (Exception ex)
            {
                LogHelper.Error($"{reqUrl}-> DeserializeResponse Handler Error {ex.Message}", ex);
            }

            return defaultValue;
        }

        private class ConfigItems // : ConfigItemsBase
        {
            public static bool DebugIsOpen
            {
                get
                {
                    var config = Ioc.Instance.CurrentConfigService; // Ioc.GetService<IConfigService>();
                    if (config == null)
                    {
                        return false;
                    }
                    return config.GetAppSettingValue("OpenDebug", false, false);
                }
            }
        }
    }
}
