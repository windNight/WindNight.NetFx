using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WindNight.Core.Extension;

namespace System
{
    /// <summary>
    /// private DefaultDateTime is <see cref="DefaultDateTime"/> (1970-01-01)
    /// </summary>
    public static partial class DateTimeExtensions
    {
        private static readonly DateTime DefaultDateTime = new DateTime(1970, 1, 1);

        private const int DefaultDateInt = 19700101;
        private const int DefaultDateMonth = 197001;

        /// <summary>
        ///     DateTime 转 时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="milliseconds">默认为true,转13位时间戳</param>
        /// <example>
        ///     Simple example to use ConvertToUnixTime :
        ///     <code lang="c#">
        ///     <![CDATA[   
        ///      HardInfo.Now.ConvertToUnixTime();
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
        ///          var ts = HardInfo.Now.ConvertToUnixTime();
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


        public static string ConvertToTimeFormatUseUnix(this long unixTime, bool milliseconds = true, string format = "yyyy-MM-dd HH:mm:ss")
        {
            return unixTime.ConvertToTimeUseUnix(milliseconds).FormatDateTime(format);
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
            if (dateStr.IsNullOrEmpty()) throw new ArgumentNullException(nameof(dateStr));
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
                return dateTime.ToDateInt(format, defaultValue);
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

        /// <summary>
        ///     格式为 2015-01-15 的时间转成为 201501  
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="defaultValue"><see cref="DefaultDateMonth"/> </param>
        /// <returns></returns>
        public static int ToDateMonth(this DateTime dateTime, int defaultValue = DefaultDateMonth)
        {
            return dateTime.ToString("yyyyMM").ToInt(defaultValue);
        }


        public static
#if NET45LATER

            (int FirstYearWeek, int EndYearWeek)
#else
                Tuple<int, int>
#endif
            CalcCurrentYearWeekRange(this DateTime date, int year = 0)
        {
            if (year == 0)
                year = date.Year;
            var beginDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year + 1, 1, 1).AddDays(-1);

            var beginWeek = beginDate.WeekOfYear();
            var endWeek = endDate.WeekOfYear();
#if NET45LATER
            return (beginWeek, endWeek);
#else
            return new Tuple<int, int>(beginWeek, endWeek);
#endif

        }


        public static
#if NET45LATER
            (DateTime beginDate, DateTime endDate)
#else
                Tuple<DateTime, DateTime>
#endif
            CalcDateRangeByWeek(this DateTime date, int week, int year = 0)
        {
            if (year == 0)
                year = date.Year;
            var firstDayOfYear = new DateTime(year, 1, 1);

            var dt = firstDayOfYear.AddDays((week - 1) * 7);
            var beginDate = dt.FirstDayOfWeek();
            var endDate = dt.LastDayOfWeek();
#if NET45LATER
            return (beginDate, endDate);
#else
            return new Tuple<DateTime, DateTime>(beginDate, endDate);
#endif
        }



    }


    public static partial class DateTimeExtensions
    {

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
            {
                return date.Date;
            }
            var dayOfWeekInt = (int)date.DayOfWeek;
            if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                dayOfWeekInt = 7;
            }
            return date.Date.AddDays(1 - dayOfWeekInt);
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


        /// <summary>
        /// TODO Reverse
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int WeekOfYear(this DateTime date)
        {
            var gregorianCalendar = new Globalization.GregorianCalendar();

            //获取指定日期是周数 CalendarWeekRule指定 第一周开始于该年的第一天，DayOfWeek指定每周第一天是星期几　
            int weekOfYear = gregorianCalendar.GetWeekOfYear(date, Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);

            return weekOfYear;

            //var dateTime = date.FirstDayOfYear();
            //var d = ((date.Date - dateTime.Date).Days + (int)dateTime.DayOfWeek) / 7.0;
            //return d.Ceiling();

        }

        public static int FirstWeekOfYear(this DateTime date)
        {
            return date.FirstDayOfYear().WeekOfYear();
        }

        public static int LastWeekOfYear(this DateTime date)
        {
            return date.LastDayOfYear().WeekOfYear();
        }



    }
    public static partial class DateTimeExtensions
    {




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
            var endDate = endDateParam ?? HardInfo.Now.Date;

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

        public static List<T> GeneratorMonthSelfList<T>(this DateTime beginDate,
            DateTime? endDateParam = null, bool withLastDay = false, Func<DateTime, T> func = null)
        {

            beginDate = beginDate.FirstDayOfMonth();
            var endDate = endDateParam ?? HardInfo.Now.Date;
            if (withLastDay)
            {
                endDate = endDate.AddDays(1);
            }

            endDate = endDate.FirstDayOfMonth();
            var list = new List<T>();
            if (beginDate > endDate) return list;
            if (func == null) return list;
            for (var i = beginDate; i < endDate; i = i.AddMonths(1))
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

            var endDate = endDateInt == 0 ? HardInfo.Now.Date : endDateInt.TryToDateTime();
            if (endDate == DefaultDateTime)
            {
                endDate = HardInfo.Now.Date;
            }
            return beginDate.GeneratorDateSelfList(endDate, withLastDay, func);

        }



        public static List<int> GeneratorDateMonthList(this DateTime beginDate, DateTime? endDateParam = null, bool withLastDay = false)
        {
            var list = beginDate.GeneratorDateSelfList(endDateParam, withLastDay, (time) => time.ToDateMonth());

            return list.Distinct().ToList();
        }



        public static List<int> GeneratorDateWeekList(this DateTime beginDate, DateTime? endDateParam = null)
        {
            var endDate = endDateParam ?? HardInfo.Now.Date;

            if (beginDate > endDate)
            {
                return new List<int>();
            }

            var beginYear = beginDate.Year;
            var endYear = endDate.Year;
            var beginWeek = beginDate.WeekOfYear();
            var endWeek = endDate.WeekOfYear();
            var list = new List<int>();
            if (beginYear == endYear)
            {
                for (var yeakWeek = beginWeek; yeakWeek <= endWeek; yeakWeek++)
                {
                    list.Add($"{beginYear}{yeakWeek:00}".ToInt());
                }

                return list;
            }

            var lastWeekOfYeark = beginDate.LastWeekOfYear();
            for (var yeakWeek = beginWeek; yeakWeek <= lastWeekOfYeark; yeakWeek++)
            {
                list.Add($"{beginYear}{yeakWeek:00}".ToInt());
            }

            for (var year = beginYear + 1; year < endYear; year++)
            {
                var day1 = new DateTime(year, 1, 1);
                var yearWeekItem = day1.CalcCurrentYearWeekRange();
#if NET45LATER
                var _1YearWeek = yearWeekItem.FirstYearWeek;
                var endYearWeek = yearWeekItem.EndYearWeek;
#else
                var _1YearWeek = yearWeekItem.Item1;
                var endYearWeek = yearWeekItem.Item2;
#endif

                for (var yeakWeek = _1YearWeek; yeakWeek <= endYearWeek; yeakWeek++)
                {
                    list.Add(yeakWeek);
                }
            }


            var firstWeekOfYear = endDate.FirstWeekOfYear();
            Trace.Write($"endDate is {endDate} endWeek is {endWeek} endYear is {endWeek} endYear FirstYearWeek is {firstWeekOfYear} ");
            for (var yeakWeek = firstWeekOfYear; yeakWeek <= endWeek; yeakWeek++)
            {
                list.Add($"{endYear}{yeakWeek:00}".ToInt());
            }

            return list;
        }





    }
}