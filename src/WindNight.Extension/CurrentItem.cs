using System;
using System.Collections.Concurrent;
using System.Text.Extension;
using Newtonsoft.Json.Extension;
using WindNight.Extension.Internals;
#if NET45
using System.Collections;
using HttpContext = WindNight.Extension.HttpContextExtension;
#else
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.WnExtension;
#endif
namespace WindNight.Extension
{
    // 建议在web项目中使用
    public class CurrentItem : IDisposable
    {
        private static readonly object LockSerialNumber = new object();

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

                    return Ioc.GetService<IHttpContextAccessor>()?.HttpContext?.Items ?? (_items ??= new ConcurrentDictionary<object, object>());
#endif
                }
                catch
                {
                    //TODO 需要优化
                    // return _items ?? (_items = new ConcurrentDictionary<object, object>());
                    return new ConcurrentDictionary<object, object>();
                }
            }
        }

        /// <summary>  </summary>
        public static string GetSerialNumber
        {
            get
            {
                string orderNumber = GetItem<string>(Consts.SERIZLNUMBER);
                if (orderNumber.IsNullOrEmpty())
                {
                    lock (LockSerialNumber)
                    {
                        orderNumber = GetItem<string>(Consts.SERIZLNUMBER);
                        if (orderNumber.IsNullOrEmpty())
                        {
                            orderNumber = GuidHelper.GenerateOrderNumber();
                            AddItem(Consts.SERIZLNUMBER, orderNumber);
                        }
                    }
                }
                return orderNumber;
            }
        }

        /// <summary>    </summary>
        public void Dispose()
        {
            Items?.Clear();
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
                Console.WriteLine(ex);
                return obj;
            }
        }

        public static bool ContainKey(string key)
        {
            bool flag = false;
            if (!key.IsNullOrEmpty() && Items != null)
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
                if (key.IsNullOrWhiteSpace() || value == null || Items == null)
                    return;
                if (!ContainKey(key))
                    Items.Add(key, value);
                else
                    Items[key] = value;
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
                if (!ContainKey(key))
                    return;
                Items.Add(key, value);
            }
            catch
            {
            }
        }

        public new static string ToString()
        {
            if (Items == null || Items.Count == 0) return "";
            if (ContainKey("MS_HttpRequestMessage"))
            {
                Items.Remove("MS_HttpRequestMessage");
            }
            return Items?.ToJsonStr();
        }
    }
}
