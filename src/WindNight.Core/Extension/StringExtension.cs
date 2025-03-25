using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using WindNight.Linq.Extensions.Expressions;

namespace System
{
    public static partial class StringExtension
    {
        /// <summary>
        /// </summary>
        /// <param name="sourceString"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string sourceString, bool ignoreWhiteSpace = false)
        {
            if (ignoreWhiteSpace)
            {
                return string.IsNullOrEmpty(sourceString) || sourceString.IsNullOrWhiteSpace();
            }

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


        /// <summary>拆分字符串，过滤空格，无效时返回空数组</summary>
        /// <param name="value">字符串</param>
        /// <param name="separators">分组分隔符，默认逗号|分号</param>
        /// <returns></returns>
        public static string[] Split(this string value, params string[] separators)
        {
            if (value.IsNullOrEmpty()) return Array.Empty<string>();
            if (separators.IsNullOrEmpty() || separators.Length < 1 ||
                (separators.Length == 1 && separators[0].IsNullOrEmpty()))
                separators = new[] { ",", ";" };// default separators

            return value.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        }


        /// <summary>拆分字符串成为整型数组，默认逗号分号分隔，无效时返回空数组</summary>
        /// <remarks>过滤空格、过滤无效、不过滤重复</remarks>
        /// <param name="value">字符串</param>
        /// <param name="separators">分组分隔符，默认逗号|分号</param>
        /// <returns></returns>
        public static IEnumerable<int> Split2Int(this string value, params string[] separators)
        {
            var ss = value.Split(separators);
            var res = ss.Select(m => m.ToInt());
            return res;

        }

        /// <summary>把一个列表组合成为一个字符串，默认逗号分隔</summary>
        /// <param name="value"></param>
        /// <param name="separator">组合分隔符，默认逗号</param>
        /// <returns></returns>
        public static string Join(this IEnumerable value, string separator = ",")
        {
            var sb = new StringBuilder();
            if (value != null)
            {
                foreach (var item in value)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(separator);
                    }
                    sb.Append(item + "");
                }
            }
            return sb.ToString();
        }




    }

    /// <summary> </summary>
    public static partial class StringExtension
    {
        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt(this object obj, int defaultValue = 0)
        {
            var sourceString = obj?.ToString() ?? "";
            if (sourceString.IsNullOrEmpty())
                return defaultValue;
            return int.TryParse(sourceString, out var rlt) ? rlt : defaultValue;
        }


        /// <summary>
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt(this string sourceString, int defaultValue = 0)
        {
            if (sourceString.IsNullOrEmpty())
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
            if (sourceString.IsNullOrEmpty())
                return defaultValue;
            return long.TryParse(sourceString, out var rlt) ? rlt : defaultValue;
        }


        public static long ToLong(this object obj, long defaultValue = 0)
        {
            var sourceString = obj?.ToString() ?? "";
            if (sourceString.IsNullOrEmpty())
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
            if (sourceString.IsNullOrEmpty())
                return defaultValue;
            return decimal.TryParse(sourceString, out var rlt) ? rlt : defaultValue;
        }

        public static decimal ToDecimal(this object obj, decimal defaultValue = 0M)
        {
            var sourceString = obj?.ToString() ?? "";
            if (sourceString.IsNullOrEmpty())
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
            if (sourceString.IsNullOrEmpty())
                return defaultValue;
            return double.TryParse(sourceString, out var rlt) ? rlt : defaultValue;
        }

        public static double ToDouble(this object obj, double defaultValue = 0)
        {
            var sourceString = obj?.ToString() ?? "";
            if (sourceString.IsNullOrEmpty())
                return defaultValue;
            return double.TryParse(sourceString, out var rlt) ? rlt : defaultValue;
        }

        static readonly string[] TrueStrings = { "1", bool.TrueString, "T", };
        static readonly string[] FalseStrings = { "0", bool.FalseString, "F", };

        public static bool ToBoolean(this object obj, bool defaultValue = false)
        {

            var sourceString = obj?.ToString() ?? "";
            if (sourceString.IsNullOrEmpty())
                return defaultValue;

            var s = sourceString.ToUpper();
            if (TrueStrings.Contains(s))
            {
                return true;
            }

            if (FalseStrings.Contains(s))
            {
                return false;
            }
            return defaultValue;

        }

        /// <summary>
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="limitLength"></param>
        /// <returns></returns>
        public static string Limit(this string sourceString, int limitLength)
        {
            if (sourceString.IsNullOrEmpty())
                return string.Empty;

            if (sourceString.Length <= limitLength)
                return sourceString;

            return sourceString.Substring(0, limitLength);
        }

        public static string UrlEncode(this string sourceString)
        {
            if (sourceString.IsNullOrEmpty()) return sourceString;
            return WebUtility.UrlEncode(sourceString) ?? string.Empty;
        }

        public static string UrlDecode(this string sourceString)
        {
            if (sourceString.IsNullOrEmpty()) return sourceString;
            return WebUtility.UrlDecode(sourceString);
        }
    }
}
