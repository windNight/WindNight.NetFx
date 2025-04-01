using System;
using Newtonsoft.Json.Linq;
using WindNight.Core.@internal;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#if NETSTANDARD2_1
using System.Diagnostics.CodeAnalysis;
#endif

namespace Newtonsoft.Json.Extension
{
    /// <summary> </summary>
    public static class NJsonExtensions
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
            JsonSerializerSettings settings = null)
        {
            if (obj == null) return default;
            return obj.ToJsonStr(formatting).To<T>(settings);
        }

#if STD21 
        [return: MaybeNull]
#endif
        public static TResult To<TSource, TResult>(this TSource obj, Formatting formatting = Formatting.None,
            JsonSerializerSettings settings = null, Action<TSource, TResult> converter = null)
            where TSource : class, new()
            where TResult : class, new()
        {
            if (obj == null) return default;

            var model = obj.To<TResult>(formatting, settings);

            if (converter != null)
            {
                converter.Invoke(obj, model);
            }

            return model;
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
                        Error = (se, ev) => { ev.ErrorContext.Handled = true; },
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
                settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            //settings.Converters.Add(new IPAddressConverter());
            //settings.Converters.Add(new IPEndPointConverter());

            return JsonConvert.SerializeObject(obj, formatting, settings);
        }
    }
}


namespace System.Text.Json.Extension
{
    /// <summary> </summary>
    public static class MJsonExtensions
    {

    }

    public abstract class JsonNamingPolicy : System.Text.Json.JsonNamingPolicy
    {
        /// <summary>Gets the naming policy for PascalCase.</summary>
        public static JsonNamingPolicy PascalCase { get; } = new PascalCaseJsonNamingPolicy();

    }

    public class PascalCaseJsonNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            if (name.IsNullOrEmpty())
                return name;

            // 将名称按分隔符（如下划线、连字符）或大小写变化拆分成单词
            var words = SplitIntoWords(name);
            return words.Select(CapitalizeFirstLetter).Concat();
        }

        private static IEnumerable<string> SplitIntoWords(string name)
        {
            // 使用正则表达式分割：大写字母前、下划线、连字符
            return Regex.Split(name, @"(?<!^)(?=[A-Z])|[_\-]");
        }

        private static string CapitalizeFirstLetter(string word)
        {
            if (word.IsNullOrEmpty())
            {
                return word;
            }

            return $"{word[0].ToUpper()}{word.Substring(1)}";//.ToLower();
        }

    }


}
