using System.Collections.Generic;

namespace WindNight.Core.Extension
{
    public static class DictionaryExtensions
    {
        /// <summary> </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int SafeGetValue(this IReadOnlyDictionary<string, int> dict, string key)
        {
            return dict.SafeGetValue(key, 0);
        }

        /// <summary> </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int SafeGetValue(this IReadOnlyDictionary<long, int> dict, long key)
        {
            return dict.SafeGetValue(key, 0);
        }

        /// <summary> </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int SafeGetValue(this IReadOnlyDictionary<ulong, int> dict, ulong key)
        {
            return dict.SafeGetValue(key, 0);
        }

        /// <summary> </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int SafeGetValue(this IReadOnlyDictionary<int, int> dict, int key)
        {
            return dict.SafeGetValue(key, 0);
        }

        /// <summary> </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int SafeGetValue(this IReadOnlyDictionary<uint, int> dict, uint key)
        {
            return dict.SafeGetValue(key, 0);
        }

        /// <summary> </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string SafeGetValue(this IReadOnlyDictionary<string, string> dict, string key)
        {
            return dict.SafeGetValue(key, string.Empty);
        }

        /// <summary> </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string SafeGetValue(this IReadOnlyDictionary<long, string> dict, long key)
        {
            return dict.SafeGetValue(key, string.Empty);
        }

        /// <summary> </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string SafeGetValue(this IReadOnlyDictionary<ulong, string> dict, ulong key)
        {
            return dict.SafeGetValue(key, string.Empty);
        }

        /// <summary> </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string SafeGetValue(this IReadOnlyDictionary<int, string> dict, int key)
        {
            return dict.SafeGetValue(key, string.Empty);
        }

        /// <summary> </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string SafeGetValue(this IReadOnlyDictionary<uint, string> dict, uint key)
        {
            return dict.SafeGetValue(key, string.Empty);
        }

        /// <summary> </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
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