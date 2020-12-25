using System.Collections.Generic;

namespace WindNight.Core.Extension
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int SafeGetValue(this IReadOnlyDictionary<string, int> dict, string key)
        {
            return dict.SafeGetValue(key, 0);
        }


        /// <summary>
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int SafeGetValue(this IReadOnlyDictionary<long, int> dict, long key)
        {
            return dict.SafeGetValue(key, 0);
        }


        public static T SafeGetValue<TKey, T>(this IReadOnlyDictionary<TKey, T> dict, TKey key,
            T defaultValue = default)
        {
#pragma warning disable CS8603 // 可能的 null 引用返回。
            if (key == null) return defaultValue;
            return dict.ContainsKey(key) ? dict[key] : defaultValue;
#pragma warning restore CS8603 // 可能的 null 引用返回。
        }
    }
}