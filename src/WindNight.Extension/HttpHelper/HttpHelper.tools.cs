using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Newtonsoft.Json.Extension;
using RestSharp;
using WindNight.Core;
using WindNight.Extension.Internals;

namespace WindNight.Extension
{
    public static partial class HttpHelper
    {
        class ConfigItems// : ConfigItemsBase
        {
            public static bool DebugIsOpen
            {
                get
                {
                    var config = Ioc.Instance.CurrentConfigService;// Ioc.GetService<IConfigService>();
                    if (config == null) return false;
                    return config.GetAppSetting("OpenDebug", false, false);
                }
            }

        }


        public static Dictionary<string, object> GenQueryDict(this object queries)
        {
            var queryDict = new Dictionary<string, object>();
            try
            {
                //queryDict = queries
                //  .GetType()
                //  .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
                //  .ToDictionary(q => q.Name, q => q.GetValue(queries));

                var properties = queries.GetType().GetProperties(
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);

                foreach (var property in properties)
                {
                    var key = property.Name;
                    var value = property.GetValue(queries);
                    if (value != null && !queryDict.ContainsKey(key))
                    {
                        queryDict.Add(key, value);
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
                    headerDict = new Dictionary<string, string>
                    {
                        {Consts.SERIZLNUMBER, CurrentItem.GetSerialNumber}
                    };
            }
            else if (!headerDict.ContainsKey(Consts.SERIZLNUMBER))
            {
                if (CurrentItem.Items != null) headerDict.Add(Consts.SERIZLNUMBER, CurrentItem.GetSerialNumber);
            }

            return headerDict ?? new Dictionary<string, string>();
        }

        static Func<string, T> DefaultConvertFunc<T>() => _ => _.To<T>();

        static Func<string, T> DefaultResConvertFunc<T>()
        {
            return s =>
            {
                var res = s.To<ResponseResult<T>>();
                if (res.Code == 0)
                {
                    return res.Data;
                }
                LogHelper.Warn($"Code({res.Code}) res is {res.ToJsonStr()}");
                return default;
            };
        }

        static Func<string, IEnumerable<T>> DefaultEnumerableResConvertFunc<T>()// => _ => _.To<ResponseResult<IEnumerable<T>>>()?.Data ?? Array.Empty<T>();
        {
            return s =>
            {
                var res = s.To<ResponseResult<IEnumerable<T>>>();//?.Data ?? Array.Empty<T>();
                if (res.Code == 0)
                {
                    return res.Data;
                }
                LogHelper.Warn($"Code({res.Code}) res is {res.ToJsonStr()}");

                return EmptyArray<T>();
            };
        }


        static Func<string, IPagedList<T>> DefaultPagedConvertFunc<T>() //=> _ => _.To<ResponseResult<PagedList<T>>>()?.Data ?? PagedList.Empty<T>();
        {
            return s =>
            {
                var res = s.To<ResponseResult<PagedList<T>>>();//?.Data ?? Array.Empty<T>();
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


        public static T DeserializeResponse<T>(this IRestResponse response, Func<string, T> convertFunc, T defaultValue = default)
        {
            try
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (ConfigItems.DebugIsOpen)
                    {
                        LogHelper.Debug($" response.Content is {response.Content} ", appendMessage: false);
                    }

                    convertFunc ??= DefaultConvertFunc<T>();
                    return convertFunc.Invoke(response.Content);

                }

                LogHelper.Warn($" ResponseStatus is {response.ResponseStatus} {response.ErrorMessage} {response.StatusDescription}", appendMessage: false);
                return defaultValue;
            }
            catch (Exception ex)
            {
                LogHelper.Error($"DeserializeResponse Handler Error {ex.Message}", ex);
                return defaultValue;
            }

        }


        public static T DeserializeResponse<T>(this IRestResponse response)
        {
            return response.DeserializeResponse<T>(DefaultConvertFunc<T>());

            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    if (ConfigItems.DebugIsOpen)
            //        LogHelper.Debug($" response.Content is {response.Content} ", appendMessage: false);
            //    return response.Content.To<T>();
            //}

            //LogHelper.Warn($" ResponseStatus is {response.ResponseStatus} {response.ErrorMessage} {response.StatusDescription}", appendMessage: false);
            //return default;

        }



        public static IPagedList<T> DeserializeResponse2PageList<T>(this IRestResponse response)
        {
            //return response.DeserializeResponse<IPagedList<T>>(_ => _.To<ResponseResult<PagedList<T>>>(), PagedList.Empty<T>());
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (ConfigItems.DebugIsOpen)
                    LogHelper.Debug($" response.Content is {response.Content} ", appendMessage: false);
                var res = response.Content.To<ResponseResult<PagedList<T>>>();
                return res.Data;
            }

            LogHelper.Warn($" ResponseStatus is {response.ResponseStatus} {response.ErrorMessage} {response.StatusDescription}", appendMessage: false);
            return default;
        }


        public static IPagedList<T> DeserializePageListResponse<T>(this IRestResponse response)
        {
            return response.DeserializePageListResponse<T>(DefaultPagedConvertFunc<T>());
        }


        public static IPagedList<T> DeserializePageListResponse<T>(this IRestResponse response, Func<string, IPagedList<T>> convertFunc)
        {
            try
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (ConfigItems.DebugIsOpen)
                    {
                        LogHelper.Debug($" response.Content is {response.Content} ", appendMessage: false);
                    }

                    convertFunc ??= DefaultPagedConvertFunc<T>();
                    return convertFunc.Invoke(response.Content);

                }

                LogHelper.Warn($" ResponseStatus is {response.ResponseStatus} {response.ErrorMessage} {response.StatusDescription}", appendMessage: false);
                //return PagedList.Empty<T>();

            }
            catch (Exception ex)
            {
                LogHelper.Error($"DeserializeResponse Handler Error {ex.Message}", ex);
            }

            return PagedList.Empty<T>();

        }

        public static IEnumerable<T> DeserializeListResponse<T>(this IRestResponse response)
        {
            return response.DeserializeListResponse<T>(DefaultEnumerableResConvertFunc<T>());

        }
        static IEnumerable<T> EmptyArray<T>()
        {
#if NET45
            return new List<T>();
#else
            return Array.Empty<T>();
#endif

        }
        public static IEnumerable<T> DeserializeListResponse<T>(this IRestResponse response, Func<string, IEnumerable<T>> convertFunc)
        {
            try
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (ConfigItems.DebugIsOpen)
                    {
                        LogHelper.Debug($" response.Content is {response.Content} ", appendMessage: false);
                    }

                    convertFunc ??= DefaultEnumerableResConvertFunc<T>();
                    return convertFunc.Invoke(response.Content);

                }

                LogHelper.Warn($" ResponseStatus is {response.ResponseStatus} {response.ErrorMessage} {response.StatusDescription}", appendMessage: false);
                //return PagedList.Empty<T>();

            }
            catch (Exception ex)
            {
                LogHelper.Error($"DeserializeResponse Handler Error {ex.Message}", ex);
            }

            return EmptyArray<T>();

        }

        public static T DeserializeResResponse<T>(this IRestResponse response, T defaultValue = default)
        {
            return response.DeserializeResResponse<T>(DefaultResConvertFunc<T>(), defaultValue);

        }

        public static T DeserializeResResponse<T>(this IRestResponse response, Func<string, T> convertFunc, T defaultValue = default)
        {
            try
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (ConfigItems.DebugIsOpen)
                    {
                        LogHelper.Debug($" response.Content is {response.Content} ", appendMessage: false);
                    }

                    convertFunc ??= DefaultResConvertFunc<T>();
                    return convertFunc.Invoke(response.Content);

                }

                LogHelper.Warn($" ResponseStatus is {response.ResponseStatus} {response.ErrorMessage} {response.StatusDescription}", appendMessage: false);
                //return PagedList.Empty<T>();

            }
            catch (Exception ex)
            {
                LogHelper.Error($"DeserializeResponse Handler Error {ex.Message}", ex);
            }

            return defaultValue;

        }

    }

}
