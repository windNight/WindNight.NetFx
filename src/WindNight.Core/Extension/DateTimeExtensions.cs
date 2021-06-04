using System.Collections.Generic;
using System.Linq;

namespace System
{
    /// <summary>
    /// private DefaultDateTime is <see cref="DefaultDateTime"/> (1970-01-01)
    /// </summary>
    public static class DateTimeExtensions
    {
        private static readonly DateTime DefaultDateTime = new DateTime(1970, 1, 1);

        private const int DefaultDateInt = 19700101;

        /// <summary>
        ///     DateTime 转 时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="milliseconds">默认为true,转13位时间戳</param>
        /// <example>
        ///     Simple example to use ConvertToUnixTime :
        ///     <code lang="c#">
        ///     <![CDATA[   
        ///      DateTime.Now.ConvertToUnixTime();
        ///     ]]>
        /// </code>
        /// </example>
        /// <returns></returns>
        public static long ConvertToUnixTime(this DateTime dateTime, bool milliseconds = true)
        {
#if NETSTANDARD
            var dateTimeOffset = new DateTimeOffset(dateTime);
            if (milliseconds)
            {
                return dateTimeOffset.ToUnixTimeMilliseconds();
            }
            else
            {
                return dateTimeOffset.ToUnixTimeSeconds();
            }
#else
            long timestamps = 0;
            var startTime = TimeZoneInfo.ConvertTimeFromUtc(DefaultDateTime, TimeZoneInfo.Local);
            var ts = dateTime - startTime;
            if (milliseconds)
                timestamps = (long)ts.TotalMilliseconds;
            else
                timestamps = (long)ts.TotalSeconds;

            return timestamps;
#endif

        }

        /// <summary>
        ///     时间戳转DateTime
        /// </summary>
        /// <param name="unixTime"></param>
        /// <param name="milliseconds">默认为true,当作13位时间戳转</param>
        /// <example>
        ///     Simple example to use ConvertToTimeUseUnix :
        ///     <code lang="c#">
        ///         <![CDATA[   
        ///          var ts = DateTime.Now.ConvertToUnixTime();
        ///          var dateTime = ts.ConvertToTimeUseUnix();
        ///         ]]>
        /// </code>
        /// </example>
        /// <returns>
        /// </returns>
        public static DateTime ConvertToTimeUseUnix(this long unixTime, bool milliseconds = true)
        {
#if NETSTANDARD
            if (milliseconds)
            {
                return DateTimeOffset.FromUnixTimeMilliseconds(unixTime).LocalDateTime;
            }
            else
            {
                return DateTimeOffset.FromUnixTimeSeconds(unixTime).LocalDateTime;
            }
#else
            var startTime = TimeZoneInfo.ConvertTimeFromUtc(DefaultDateTime, TimeZoneInfo.Local);

            if (milliseconds)
                return startTime.AddMilliseconds(unixTime);
            return startTime.AddSeconds(unixTime);
#endif
        }

        /// <summary>
        /// 时间戳转DateInt
        /// </summary>
        /// <param name="unixTime"></param>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static int ConvertToTimeIntUseUnix(this long unixTime, bool milliseconds = true) => unixTime.ConvertToTimeUseUnix(milliseconds).ToDateInt();

        /// <summary>
        ///     格式为 20150115的数字转成时间为 2015-01-15
        ///     try_catch
        /// </summary>
        /// <param name="dateInt"></param>
        /// <param name="linkCode"></param>
        /// <param name="defaultValue"> <see cref="DefaultDateTime"/></param>
        /// <returns></returns>
        public static DateTime? TryToDateTimeSafe(this int dateInt, string linkCode = "-", DateTime? defaultValue = null)
        {
            return dateInt <= 1970101 ? defaultValue ?? DefaultDateTime : dateInt.ToString().TryToDateTimeSafe(linkCode);
        }

        /// <summary>
        ///     格式为 20150115的字符串转成时间为 2015-01-15
        ///     try_catch
        /// </summary>
        /// <param name="dateStr"></param>
        /// <param name="linkCode"></param>
        /// <returns></returns>
        public static DateTime? TryToDateTimeSafe(this string dateStr, string linkCode = "-")
        {
            if (dateStr.Length != 8)
            {
                if (DateTime.TryParse(dateStr, out var dateTime)) return dateTime;
                return null;
            }
            var newstr = dateStr.Insert(4, linkCode).Insert(7, linkCode);
            DateTime date;
            if (DateTime.TryParse(newstr, out date)) return date;
            return null;
        }

        /// <summary>
        ///     格式为 20150115的数字转成时间为 2015-01-15
        /// </summary>
        /// <param name="dateInt"></param>
        /// <param name="linkCode"></param>
        /// <param name="defaultValue"> <see cref="DefaultDateTime"/></param>
        /// <returns></returns>
        public static DateTime TryToDateTime(this int dateInt, string linkCode = "-", DateTime? defaultValue = null)
        {
            return dateInt <= 1970101 ? defaultValue ?? DefaultDateTime : dateInt.ToString().TryToDateTime(linkCode);
        }

        /// <summary>
        ///     格式为 20150115的字符串转成时间为 2015-01-15
        /// </summary>
        /// <param name="dateStr"></param>
        /// <param name="linkCode"></param>
        /// <param name="defaultValue"> <see cref="DefaultDateTime"/></param>
        /// <returns></returns>
        public static DateTime TryToDateTime(this string dateStr, string linkCode = "-", DateTime? defaultValue = null)
        {
            if (dateStr.Length != 8)
            {
                if (DateTime.TryParse(dateStr, out var dateTime)) return dateTime;
                return DefaultDateTime;
            }
            var newsStr = TryToDateString(dateStr, linkCode);
            if (DateTime.TryParse(newsStr, out var date)) return date;
            return defaultValue ?? DefaultDateTime;
        }

        /// <summary>
        ///     格式为 20150115的字符串转成时间为 2015-01-15
        /// </summary>
        /// <param name="dateStr"></param>
        /// <param name="linkCode"></param>
        /// <returns></returns>
        public static string TryToDateString(this string dateStr, string linkCode = "-")
        {
            if (string.IsNullOrEmpty(dateStr)) throw new ArgumentNullException(nameof(dateStr));
            if (dateStr.Length < 8) dateStr = dateStr.PadRight(8, '0');
            if (dateStr.Length > 8) dateStr = dateStr.Substring(0, 8);
            var newstr = dateStr.Insert(4, linkCode).Insert(7, linkCode);
            return newstr;
        }

        /// <summary>
        ///     格式为 20150115的数字转成时间格式字符串为 2015-01-15
        /// </summary>
        /// <param name="dateInt"></param>
        /// <param name="linkCode"></param>
        /// <returns></returns>
        public static string TryToDateString(this int dateInt, string linkCode = "-") => dateInt.ToString().TryToDateString(linkCode);

        /// <summary>
        ///     格式化DateTime 默认为 yyyy-MM-dd
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string FormatDateTime(this DateTime dateTime, string format = "yyyy-MM-dd")
        {
            return dateTime.ToString(format);
        }

        /// <summary>
        ///     格式为 2015-01-15的字符串转成时间为 20150115
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="format"></param>
        /// <param name="defaultValue"><see cref="DefaultDateInt"/> </param>
        /// <returns></returns>
        public static int TryToDateInt(this DateTime dateTime, string format = "yyyyMMdd", int defaultValue = DefaultDateInt)
        {
            try
            {
                return dateTime.ToDateInt(format);
            }
            catch// (Exception ex)
            {
                return defaultValue;
            }
        }

        /// <summary>
        ///     格式为 2015-01-15 的时间转成为 20150115 或者指定格式<see cref="P:format" />的数字
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="format"></param>
        /// <param name="defaultValue"><see cref="DefaultDateInt"/> </param>
        /// <returns></returns>
        public static int ToDateInt(this DateTime dateTime, string format = "yyyyMMdd", int defaultValue = DefaultDateInt)
        {
            return dateTime.ToString(format).ToInt(defaultValue);
        }

        public static DateTime FirstDayOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        public static DateTime LastDayOfMonth(this DateTime dateTime)
        {
            return dateTime.FirstDayOfMonth().AddMonths(1).AddDays(-1);
        }

        public static DateTime FirstDayOfYear(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, 1, 1);
        }

        public static DateTime LastDayOfYear(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, 12, 31);
        }

        /// <summary>
        ///  周一为第一天
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime FirstDayOfWeek(this DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Monday)
                return date.Date;
            return date.Date.AddDays(1 - (int)date.DayOfWeek);
        }

        /// <summary>
        /// 周日为最后一天
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime LastDayOfWeek(this DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Sunday)
                return date.Date;
            return date.Date.AddDays(7 - (int)date.DayOfWeek);
        }

        public static int WeekOfYear(this DateTime date)
        {
            var dateTime = date.FirstDayOfYear();
            return (int)((date - dateTime).Days + dateTime.DayOfWeek) / 7 + 1;
        }

        public static List<int> GeneratorDateIntList(this DateTime beginDate, DateTime? endDateParam = null, bool withLastDay = false)
        {
            return beginDate.GeneratorDateSelfList(endDateParam, withLastDay, (time) => time.ToDateInt());
        }

        public static List<string> GeneratorDateStringList(this DateTime beginDate, DateTime? endDateParam = null, string format = "yyyyMMdd", bool withLastDay = false)
        {
            return beginDate.GeneratorDateSelfList(endDateParam, withLastDay, (time) => time.FormatDateTime(format));

        }
        public static List<DateTime> GeneratorDateTimeList(this DateTime beginDate, DateTime? endDateParam = null, bool withLastDay = false)
        {
            return beginDate.GeneratorDateSelfList(endDateParam, withLastDay, (time) => time);
        }

        public static List<T> GeneratorDateSelfList<T>(this DateTime beginDate, DateTime? endDateParam = null, bool withLastDay = false, Func<DateTime, T> func = null)
        {

            var endDate = endDateParam ?? DateTime.Now.Date;

            if (withLastDay)
            {
                endDate = endDate.AddDays(1);
            }
            var list = new List<T>();
            if (beginDate > endDate) return list;
            if (func == null) return list;
            for (var i = beginDate; i < endDate; i = i.AddDays(1))
            {
                var m = func.Invoke(i);
                list.Add(m);
            }

            return list;
        }

        public static List<int> GeneratorDateIntList(this int beginDateInt, DateTime? endDateParam = null, bool withLastDay = false)
        {
            return beginDateInt.GeneratorDateSelfList(endDateParam, withLastDay, (time) => time.ToDateInt());
        }
        public static List<string> GeneratorDateStringList(this int beginDateInt, DateTime? endDateParam = null, string format = "yyyyMMdd", bool withLastDay = false)
        {
            return beginDateInt.GeneratorDateSelfList(endDateParam, withLastDay, (time) => time.FormatDateTime(format));

        }

        public static List<DateTime> GeneratorDateTimeList(this int beginDateInt, DateTime? endDateParam = null, bool withLastDay = false)
        {
            return beginDateInt.GeneratorDateSelfList(endDateParam, withLastDay, (time) => time);
        }


        public static List<T> GeneratorDateSelfList<T>(this int beginDateInt, DateTime? endDateParam = null, bool withLastDay = false, Func<DateTime, T> func = null)
        {
            var beginDate = beginDateInt.TryToDateTime();
            if (beginDate == DefaultDateTime) return new List<T>();
            return beginDate.GeneratorDateSelfList(endDateParam, withLastDay, func);
        }

        public static List<T> GeneratorDateSelfList<T>(this int beginDateInt, int endDateInt = 0, bool withLastDay = false, Func<DateTime, T> func = null)
        {
            var beginDate = beginDateInt.TryToDateTime();
            if (beginDate == DefaultDateTime) return new List<T>();

            var endDate = endDateInt == 0 ? DateTime.Now.Date : endDateInt.TryToDateTime();
            if (endDate == DefaultDateTime)
            {
                endDate = DateTime.Now.Date;
            }
            return beginDate.GeneratorDateSelfList(endDate, withLastDay, func);

        }


    }
}