using System;
using System.Collections.Concurrent;
using System.Text.Extension;
using Newtonsoft.Json.Extension;
using WindNight.Extension.@internal;
using WindNight.Core.Abstractions;
using WindNight.LogExtension;



#if NET45
using System.Collections;
using HttpContext = WindNight.Extension.HttpContextExtension;
#else
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using System.Threading;
using WindNight.Core.Extension;
#endif



namespace WindNight.Extension
{

    public class DefaultCurrentContext : ICurrentContext
    {

        public string SerialNumber => CurrentItem.GetSerialNumber;

        public
#if NET45
            IDictionary
#else

                    IDictionary<object, object>
#endif
                  CurrentItems => CurrentItem.Items;

        public T GetItem<T>(string key)
        {
            return CurrentItem.GetItem<T>(key);
        }

        private const string SERIZLNUMBER = Consts.SERIZLNUMBER;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"> SERIZLNUMBER 新版本不支持 外部维护 统一使用 <see cref="SerialNumber"/></param>
        /// <param name="value"></param>
        public void AddItem(string key, object value)
        {
            if (key.Equals(SERIZLNUMBER, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            CurrentItem.AddItem(key, value);
        }

        public void AddItemIfNotExits(string key, object value)
        {
            if (key.Equals(SERIZLNUMBER, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            CurrentItem.AddItemIfNotExits(key, value);
        }

#if !NET45

        public T GetItemsFromAsyncLocal<T>(string key, T defaultValue = default)
        {
            var cacheValue = CurrentItem.GetItemsFromAsyncLocal(key);
            return cacheValue.To<T>();
        }


        public T SetItems2AsyncLocal<T>(string key, T setValue = default, bool isForce = false)
        {
            try
            {
                CurrentItem.SetItems2AsyncLocal(key, setValue, isForce);
                return setValue;
            }
            catch (Exception e)
            {
                return default;
            }

        }

#endif

    }



    // 建议在web项目中使用
    public class CurrentItem : IDisposable
    {
        private static readonly object LockSerialNumber = new object();

#if !NET45

        private bool UseAsyncLocal = false;
        private static readonly AsyncLocal<ConcurrentDictionary<object, object>> ItemsAsyncLocal = new AsyncLocal<ConcurrentDictionary<object, object>>();

        public static ConcurrentDictionary<object, object> CurrentItemsAsyncLocal => ItemsAsyncLocal.Value ?? new ConcurrentDictionary<object, object>();

        public static object GetItemsFromAsyncLocal(object key, object defaultValue = null)
        {
            try
            {
                var value = ItemsAsyncLocal.Value.SafeGetValue(key);
                return value;
            }
            catch (Exception e)
            {
                return defaultValue;
            }
        }

        public static object SetItems2AsyncLocal(object key, object setValue = null, bool isForce = false)
        {
            try
            {
                if (ItemsAsyncLocal.Value == null)
                {
                    return null;
                }
                if (!ItemsAsyncLocal.Value.ContainsKey(key))
                {
                    ItemsAsyncLocal.Value.TryAdd(key, setValue);
                    return setValue;
                }
                else if (ItemsAsyncLocal.Value.ContainsKey(key) && isForce)
                {
                    ItemsAsyncLocal.Value[key] = setValue;
                    return setValue;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        bool ClearItemsAsyncLocal()
        {
            try
            {
                ItemsAsyncLocal.Value?.Clear();
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }

        }
#endif

        private static
#if NET45
            IDictionary
#else

            IDictionary<object, object>
#endif
            _items;



        /// <summary>
        ///     Gets or sets a key/value collection that can be used to share data within the scope of this request.
        ///     请勿在异步环境中使用此属性
        /// </summary>
        public static
#if NET45
            IDictionary
#else

            IDictionary<object, object>
#endif
        Items
        {
            get
            {
                try
                {
#if NET45
                    return HttpContext.GetHttpContext()?.Items ?? (_items ??= new ConcurrentDictionary<object, object>());
#else

                    return Ioc.GetService<IHttpContextAccessor>()?.HttpContext?.Items ?? (_items ??= CurrentItemsAsyncLocal);
#endif
                }
                catch
                {
#if NET45
    //TODO 需要优化
                    // return _items ?? (_items = new ConcurrentDictionary<object, object>());
                    return new ConcurrentDictionary<object, object>();
#else
                    return CurrentItemsAsyncLocal;
#endif

                }
            }
        }


        /// <summary>  </summary>
        public static string GetSerialNumber => GetSerialNumberInternal();

        //        {
        //            get
        //            {
        //                string orderNumber = GetItem<string>(Consts.SERIZLNUMBER);
        //                if (orderNumber.IsNullOrEmpty())
        //                {
        //                    lock (LockSerialNumber)
        //                    {
        //                        orderNumber = GetItem<string>(Consts.SERIZLNUMBER);
        //                        if (orderNumber.IsNullOrEmpty())
        //                        {
        //                            orderNumber = GuidHelper.GenerateOrderNumber();
        //                            // AddItem(Consts.SERIZLNUMBER, orderNumber);
        //                            Items[Consts.SERIZLNUMBER] = orderNumber;
        //                        }
        //}
        //                }
        //                return orderNumber;
        //            }
        //        }

        public static string AddSerialNumber(string traceId, bool isForce = false) => GetSerialNumberInternal(traceId, isForce);

        static string GetSerialNumberInternal(string traceId = "", bool isForce = false)
        {
            try
            {
                if (traceId.IsNotNullOrEmpty() && isForce)
                {
                    isForce = true;
                }
                var orderNumber = GetItem<string>(Consts.SERIZLNUMBER);
                if (isForce || orderNumber.IsNullOrEmpty())
                {
                    lock (LockSerialNumber)
                    {
                        if (isForce)
                        {
                            Items[Consts.SERIZLNUMBER] = traceId;
                        }
                        else
                        {
                            orderNumber = GetTraceIdFromItems();
                            if (orderNumber.IsNullOrEmpty())
                            {
                                if (traceId.IsNullOrEmpty())
                                {
                                    traceId = GuidHelper.GenerateOrderNumber();
                                }
                                Items[Consts.SERIZLNUMBER] = traceId;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error($"Gen TraceId Handler Error {ex.Message}", ex);
            }

            return GetTraceIdFromItems();
        }

        static string GetTraceIdFromItems()
        {
            return GetItem<string>(Consts.SERIZLNUMBER);
        }

        /// <summary>    </summary>
        public void Dispose()
        {
            Items?.Clear();

#if !NET45
            ClearItemsAsyncLocal();
#endif
        }

        /// <summary>    </summary>
        ~CurrentItem()
        {
            Dispose();
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetItem<T>(string key)
        {
            T obj = default(T);
            try
            {
                if (!ContainKey(key))
                    return obj;
                return Items[key].To<T>();
            }
            catch (Exception ex)
            {
                LogHelper.Error($"GetItem({key}) handler Error {ex.Message}", ex);
                return obj;
            }
        }

        public static bool ContainKey(string key)
        {
            bool flag = false;
            if (key.IsNotNullOrEmpty() && Items != null)
            {
#if NET45
                flag = Items.Contains(key);
#else
                flag = Items.ContainsKey(key);
#endif
            }

            return flag;
        }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddItem(string key, object value)
        {
            try
            {
                if (key.Equals(Consts.SERIZLNUMBER, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                if (key.IsNullOrEmpty(true) || value == null || Items == null)
                {
                    return;
                }

                if (!ContainKey(key))
                {
                    Items.Add(key, value);
                }
                else
                {
                    Items[key] = value;
                }

#if !NET45
                SetItems2AsyncLocal(key, value, true);
#endif
            }
            catch
            {
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddItemIfNotExits(string key, object value)
        {
            try
            {
                if (key.Equals(Consts.SERIZLNUMBER, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                if (!ContainKey(key))
                {
                    return;
                }

                Items.Add(key, value);
#if !NET45
                SetItems2AsyncLocal(key, value, false);
#endif
            }
            catch
            {
            }
        }

        public new static string ToString()
        {
            try
            {
                if (Items == null || Items.Count == 0)
                {
                    return "";
                }
                if (ContainKey("MS_HttpRequestMessage"))
                {
                    Items.Remove("MS_HttpRequestMessage");
                }
                return Items?.ToJsonStr();
            }
            catch
            {
                return "";
            }

        }
    }
}
