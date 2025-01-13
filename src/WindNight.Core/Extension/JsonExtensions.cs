using Newtonsoft.Json.Linq;
using System;
#if NETSTANDARD2_1
using System.Diagnostics.CodeAnalysis;
#endif
using WindNight.Core.@internal;

namespace Newtonsoft.Json.Extension
{
    /// <summary> </summary>
    public static class JsonExtensions
    {
        /// <summary>
        ///     DeserializeObject Use Newtonsoft.Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="formatting"></param>
        /// <param name="settings">
        ///     The Newtonsoft.Json.JsonSerializerSettings used to deserialize the object.
        ///     If this is null, default serialization settings will be used.
        /// </param>
        /// <returns></returns>
#if STD21
        [return: MaybeNull]
#endif
        public static T To<T>(this object? obj, Formatting formatting = Formatting.None,
            JsonSerializerSettings? settings = null)
        {
            if (obj == null) return default;
            return obj.ToJsonStr(formatting).To<T>(settings);
        }

        public static JObject? ToJObject(this object? obj, JsonSerializer? jsonSerializer = null)
        {
            if (obj == null) return null;
            if (jsonSerializer == null) jsonSerializer = JsonSerializer.CreateDefault();
            try
            {
                return JObject.FromObject(obj, jsonSerializer);
            }
            catch
            {
                return null;
            }
        }
         
        /// <summary>
        ///     DeserializeObject Use Newtonsoft.Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonStr"></param>
        /// <param name="settings">
        ///     The Newtonsoft.Json.JsonSerializerSettings used to deserialize the object.
        ///     If this is null, default serialization settings will be used.
        /// </param>
        /// <returns></returns>
#if STD21
        [return: MaybeNull]
#endif

        public static T To<T>(this string jsonStr, JsonSerializerSettings? settings = null)
        {
            if (jsonStr.IsNullOrEmpty()) return default;

            try
            {
                if (settings == null)
                    settings = new JsonSerializerSettings
                    {
                        // JSON反序列化错误处理
                        Error = (se, ev) => { ev.ErrorContext.Handled = true; }
                    };
                // settings.Converters.Add(new IPAddressConverter());
                // settings.Converters.Add(new IPEndPointConverter());

                return JsonConvert.DeserializeObject<T>(jsonStr, settings);
            }
            catch (Exception ex)
            {
                LogHelper.Warn($"Json.net反序列异常 {jsonStr} T is {typeof(T).Namespace}.{typeof(T).Name} ", ex);
                return default;
            }
        }

        /// <summary>
        ///     SerializeObject Use Newtonsoft.Json
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="formatting"></param>
        /// <param name="settings">
        ///     The Newtonsoft.Json.JsonSerializerSettings used to serialize the object. If this is null, default serialization
        ///     settings will be used.
        /// </param>
        /// <returns></returns>
        public static string ToJsonStr(this object? obj, Formatting formatting = Formatting.None,
            JsonSerializerSettings? settings = null)
        {
            if (settings == null)
                settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
            //settings.Converters.Add(new IPAddressConverter());
            //settings.Converters.Add(new IPEndPointConverter());

            return JsonConvert.SerializeObject(obj, formatting, settings);
        }

    }
     
}