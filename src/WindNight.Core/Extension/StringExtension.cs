using System.Net;

namespace System
{
    /// <summary> </summary>
    public static class StringExtension
    {
        /// <summary>
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt(this string sourceString, int defaultValue = 0)
        {
            if (string.IsNullOrEmpty(sourceString))
                return defaultValue;
            return int.TryParse(sourceString, out var rlt) ? rlt : defaultValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long ToLong(this string sourceString, long defaultValue = 0)
        {
            if (string.IsNullOrEmpty(sourceString))
                return defaultValue;
            return long.TryParse(sourceString, out var rlt) ? rlt : defaultValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this string sourceString, decimal defaultValue = 0M)
        {
            if (string.IsNullOrEmpty(sourceString))
                return defaultValue;
            return decimal.TryParse(sourceString, out var rlt) ? rlt : defaultValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double ToDouble(this string sourceString, double defaultValue = 0)
        {
            if (string.IsNullOrEmpty(sourceString))
                return defaultValue;
            return double.TryParse(sourceString, out var rlt) ? rlt : defaultValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="limitLength"></param>
        /// <returns></returns>
        public static string Limit(this string sourceString, int limitLength)
        {
            if (string.IsNullOrEmpty(sourceString))
                return string.Empty;

            if (sourceString.Length <= limitLength)
                return sourceString;

            return sourceString.Substring(0, limitLength);
        }

        public static string UrlEncode(this string sourceString)
        {
            if (string.IsNullOrEmpty(sourceString)) return sourceString;
            return WebUtility.UrlEncode(sourceString);
        }

        public static string UrlDecode(this string sourceString)
        {
            if (string.IsNullOrEmpty(sourceString)) return sourceString;
            return WebUtility.UrlDecode(sourceString);
        }

    }
}