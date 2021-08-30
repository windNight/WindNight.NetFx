using System.Linq;
using System.Net;

namespace System
{
    public static partial class StringExtension
    {
        /// <summary>
        /// </summary>
        /// <param name="sourceString"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string sourceString)
        {
            return string.IsNullOrEmpty(sourceString);
        }

        /// <summary>
        /// </summary>
        /// <param name="sourceString"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string sourceString)
        {
            return string.IsNullOrWhiteSpace(sourceString);
        }


        /// <summary>
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Concat(this string sourceString, params string[] args)
        {
            var sL = args.ToList();
            sL.Insert(0, sourceString);
            return string.Concat(sL);
        }




    }
    /// <summary> </summary>
    public static partial class StringExtension
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