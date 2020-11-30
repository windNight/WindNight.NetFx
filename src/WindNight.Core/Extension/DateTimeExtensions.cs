using System.Collections.Generic;

namespace System
{
    /// <summary> </summary>
    public static class DateTimeExtensions
    {
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
            long timestamps = 0;
            var startTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            var ts = dateTime - startTime;
            if (milliseconds)
                timestamps = (long)ts.TotalMilliseconds;
            else
                timestamps = (long)ts.TotalSeconds;

            return timestamps;
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
            var startTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Local);

            if (milliseconds)
                return startTime.AddMilliseconds(unixTime);
            return startTime.AddSeconds(unixTime);
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
            if (dateStr.Length != 8) return null;
            var newstr = dateStr.Insert(4, linkCode).Insert(7, linkCode);
            DateTime date;
            if (DateTime.TryParse(newstr, out date)) return date;
            return null;
        }

        /// <summary>
        ///     格式为 20150115的字符串转成时间为 2015-01-15
        /// </summary>
        /// <param name="dateStr"></param>
        /// <param name="linkCode"></param>
        /// <returns></returns>
        public static DateTime TryToDateTime(this string dateStr, string linkCode = "-")
        {
            var newstr = TryToDateString(dateStr, linkCode);
            if (DateTime.TryParse(newstr, out var date)) return date;
            return new DateTime(1970, 1, 1);
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
        /// <returns></returns>
        public static int TryToDateInt(this DateTime dateTime, string format = "yyyyMMdd")
        {
            try
            {
                return dateTime.ToDateInt(format);
            }
            catch (Exception ex)
            {
                return 19700101;
            }
        }

        /// <summary>
        ///     格式为 2015-01-15 的时间转成为 20150115 或者指定格式<see cref="P:format" />的数字
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static int ToDateInt(this DateTime dateTime, string format = "yyyyMMdd")
        {
            return dateTime.ToString(format).ToInt(19700101);
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

        public static List<int> GeneratorDateIntList(this DateTime beginDate, DateTime? endDateParam = null)
        {
            var endDate = DateTime.Now.Date;
            if (endDateParam.HasValue)
            {
                endDate = endDateParam.Value;
            }
            if (beginDate > endDate) return new List<int>();
            var list = new List<int>();
            for (var i = beginDate; i < endDate; i = i.AddDays(1))
            {
                list.Add(i.ToDateInt());
            }
            return list;
        }


    }
}