using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.Extension;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Extension;
using WindNight.Core.Abstractions;
using WindNight.Core.Extension;
using WindNight.Core.@internal;

namespace System
{
    public partial class HardInfo
    {
        public static DateTime MaxDate => DateTime.MaxValue;
        public static DateTime MinDate => DateTime.MinValue;

        public static DateTime Now => DateTime.Now;
        public static string NowFullString => DateTime.Now.FormatDateTimeFullString();
        public static DateTime Yesterday => Now.AddDays(-1);
        public static DateTime Tomorrow => Now.AddDays(1);

        public static int WeekOfYear => Now.WeekOfYear();

        public static DateTime FirstDayOfThisMonth => Now.FirstDayOfMonth();

        public static DateTime LastDayOfThisMonth => Now.LastDayOfMonth();

        public static DateTime FirstDayOfThisWeek => Now.FirstDayOfWeek();

        public static DateTime LastDayOfThisWeek => Now.LastDayOfWeek();

        public static
#if NET45LATER
            (DateTime beginDate, DateTime endDate)
#else
            Tuple<DateTime, DateTime>
#endif
            CurrentWeekDateRange
        {
            get
            {
                var beginDate = FirstDayOfThisWeek;
                var endDate = LastDayOfThisWeek;
#if NET45LATER
                return (beginDate, endDate);
#else
                return new Tuple<DateTime, DateTime>(beginDate, endDate);
#endif
            }
        }

        public static DateTime FirstDayOfThisYear => Now.FirstDayOfYear();

        public static DateTime LastDayOfThisYear => Now.LastDayOfYear();

        public static int NowDateInt => Now.ToDateInt();

        public static int YesterdayDateInt => Yesterday.ToDateInt();
        public static int TomorrowDateInt => Tomorrow.ToDateInt();

        public static long NowUnixTime => Now.ConvertToUnixTime();

        public static int NowYearInt => Now.Year;

        public static string NowString => $"{Now:yyyy-MM-dd HH:mm:ss}";

        public static int NowMonthInt => Now.TryToDateInt("yyyyMM");

        public static int LastMonthInt => Now.FirstDayOfMonth().AddMonths(-1).TryToDateInt("yyyyMM");

        public static int NextMonthInt => Now.FirstDayOfMonth().AddMonths(1).TryToDateInt("yyyyMM");

    }

    /// <summary>硬件信息</summary>
    public partial class HardInfo
    {
        private const string DefaultIp = "0.0.0.0";

        private static readonly Dictionary<string, string> olsonWindowsTimes = new()
        {
            { "Africa/Bangui", "W. Central Africa Standard Time" },
            { "Africa/Cairo", "Egypt Standard Time" },
            { "Africa/Casablanca", "Morocco Standard Time" },
            { "Africa/Harare", "South Africa Standard Time" },
            { "Africa/Johannesburg", "South Africa Standard Time" },
            { "Africa/Lagos", "W. Central Africa Standard Time" },
            { "Africa/Monrovia", "Greenwich Standard Time" },
            { "Africa/Nairobi", "E. Africa Standard Time" },
            { "Africa/Windhoek", "Namibia Standard Time" },
            { "America/Anchorage", "Alaskan Standard Time" },
            { "America/Argentina/San_Juan", "Argentina Standard Time" },
            { "America/Asuncion", "Paraguay Standard Time" },
            { "America/Bahia", "Bahia Standard Time" },
            { "America/Bogota", "SA Pacific Standard Time" },
            { "America/Buenos_Aires", "Argentina Standard Time" },
            { "America/Caracas", "Venezuela Standard Time" },
            { "America/Cayenne", "SA Eastern Standard Time" },
            { "America/Chicago", "Central Standard Time" },
            { "America/Chihuahua", "Mountain Standard Time (Mexico)" },
            { "America/Cuiaba", "Central Brazilian Standard Time" },
            { "America/Denver", "Mountain Standard Time" },
            { "America/Fortaleza", "SA Eastern Standard Time" },
            { "America/Godthab", "Greenland Standard Time" },
            { "America/Guatemala", "Central America Standard Time" },
            { "America/Halifax", "Atlantic Standard Time" },
            { "America/Indianapolis", "US Eastern Standard Time" },
            { "America/Indiana/Indianapolis", "US Eastern Standard Time" },
            { "America/La_Paz", "SA Western Standard Time" },
            { "America/Los_Angeles", "Pacific Standard Time" },
            { "America/Mexico_City", "Mexico Standard Time" },
            { "America/Montevideo", "Montevideo Standard Time" },
            { "America/New_York", "Eastern Standard Time" },
            { "America/Noronha", "UTC-02" },
            { "America/Phoenix", "US Mountain Standard Time" },
            { "America/Regina", "Canada Central Standard Time" },
            { "America/Santa_Isabel", "Pacific Standard Time (Mexico)" },
            { "America/Santiago", "Pacific SA Standard Time" },
            { "America/Sao_Paulo", "E. South America Standard Time" },
            { "America/St_Johns", "Newfoundland Standard Time" },
            { "America/Tijuana", "Pacific Standard Time" },
            { "Antarctica/McMurdo", "New Zealand Standard Time" },
            { "Atlantic/South_Georgia", "UTC-02" },
            { "Asia/Almaty", "Central Asia Standard Time" },
            { "Asia/Amman", "Jordan Standard Time" },
            { "Asia/Baghdad", "Arabic Standard Time" },
            { "Asia/Baku", "Azerbaijan Standard Time" },
            { "Asia/Bangkok", "SE Asia Standard Time" },
            { "Asia/Beirut", "Middle East Standard Time" },
            { "Asia/Calcutta", "India Standard Time" },
            { "Asia/Colombo", "Sri Lanka Standard Time" },
            { "Asia/Damascus", "Syria Standard Time" },
            { "Asia/Dhaka", "Bangladesh Standard Time" },
            { "Asia/Dubai", "Arabian Standard Time" },
            { "Asia/Irkutsk", "North Asia East Standard Time" },
            { "Asia/Jerusalem", "Israel Standard Time" },
            { "Asia/Kabul", "Afghanistan Standard Time" },
            { "Asia/Kamchatka", "Kamchatka Standard Time" },
            { "Asia/Karachi", "Pakistan Standard Time" },
            { "Asia/Katmandu", "Nepal Standard Time" },
            { "Asia/Kolkata", "India Standard Time" },
            { "Asia/Krasnoyarsk", "North Asia Standard Time" },
            { "Asia/Kuala_Lumpur", "Singapore Standard Time" },
            { "Asia/Kuwait", "Arab Standard Time" },
            { "Asia/Magadan", "Magadan Standard Time" },
            { "Asia/Muscat", "Arabian Standard Time" },
            { "Asia/Novosibirsk", "N. Central Asia Standard Time" },
            { "Asia/Oral", "West Asia Standard Time" },
            { "Asia/Rangoon", "Myanmar Standard Time" },
            { "Asia/Riyadh", "Arab Standard Time" },
            { "Asia/Seoul", "Korea Standard Time" },
            { "Asia/Shanghai", "China Standard Time" },
            { "Asia/Singapore", "Singapore Standard Time" },
            { "Asia/Taipei", "Taipei Standard Time" },
            { "Asia/Tashkent", "West Asia Standard Time" },
            { "Asia/Tbilisi", "Georgian Standard Time" },
            { "Asia/Tehran", "Iran Standard Time" },
            { "Asia/Tokyo", "Tokyo Standard Time" },
            { "Asia/Ulaanbaatar", "Ulaanbaatar Standard Time" },
            { "Asia/Vladivostok", "Vladivostok Standard Time" },
            { "Asia/Yakutsk", "Yakutsk Standard Time" },
            { "Asia/Yekaterinburg", "Ekaterinburg Standard Time" },
            { "Asia/Yerevan", "Armenian Standard Time" },
            { "Atlantic/Azores", "Azores Standard Time" },
            { "Atlantic/Cape_Verde", "Cape Verde Standard Time" },
            { "Atlantic/Reykjavik", "Greenwich Standard Time" },
            { "Australia/Adelaide", "Cen. Australia Standard Time" },
            { "Australia/Brisbane", "E. Australia Standard Time" },
            { "Australia/Darwin", "AUS Central Standard Time" },
            { "Australia/Hobart", "Tasmania Standard Time" },
            { "Australia/Perth", "W. Australia Standard Time" },
            { "Australia/Sydney", "AUS Eastern Standard Time" },
            { "Etc/GMT", "UTC" },
            { "Etc/GMT+11", "UTC-11" },
            { "Etc/GMT+12", "Dateline Standard Time" },
            { "Etc/GMT+2", "UTC-02" },
            { "Etc/GMT-12", "UTC+12" },
            { "Europe/Amsterdam", "W. Europe Standard Time" },
            { "Europe/Athens", "GTB Standard Time" },
            { "Europe/Belgrade", "Central Europe Standard Time" },
            { "Europe/Berlin", "W. Europe Standard Time" },
            { "Europe/Brussels", "Romance Standard Time" },
            { "Europe/Budapest", "Central Europe Standard Time" },
            { "Europe/Dublin", "GMT Standard Time" },
            { "Europe/Helsinki", "FLE Standard Time" },
            { "Europe/Istanbul", "GTB Standard Time" },
            { "Europe/Kiev", "FLE Standard Time" },
            { "Europe/London", "GMT Standard Time" },
            { "Europe/Minsk", "E. Europe Standard Time" },
            { "Europe/Moscow", "Russian Standard Time" },
            { "Europe/Paris", "Romance Standard Time" },
            { "Europe/Sarajevo", "Central European Standard Time" },
            { "Europe/Warsaw", "Central European Standard Time" },
            { "Indian/Mauritius", "Mauritius Standard Time" },
            { "Pacific/Apia", "Samoa Standard Time" },
            { "Pacific/Auckland", "New Zealand Standard Time" },
            { "Pacific/Fiji", "Fiji Standard Time" },
            { "Pacific/Guadalcanal", "Central Pacific Standard Time" },
            { "Pacific/Guam", "West Pacific Standard Time" },
            { "Pacific/Honolulu", "Hawaiian Standard Time" },
            { "Pacific/Pago_Pago", "UTC-11" },
            { "Pacific/Port_Moresby", "West Pacific Standard Time" },
            { "Pacific/Tongatapu", "Tonga Standard Time" },
        };

        public static string? NodeCode { get; private set; }

        public static string NodeIpAddress { get; private set; } = "";
        public static string EnvironmentName => Ioc.GetService<IHostEnvironment>()?.EnvironmentName ?? "";
        public static string ApplicationName => Ioc.GetService<IHostEnvironment>()?.ApplicationName ?? "";

        public static bool IsUnix => OperatorSys == OperatorSysEnum.Unix;
        public static bool IsWindows => OperatorSys == OperatorSysEnum.Windows;
        public static bool IsMac => OperatorSys == OperatorSysEnum.MacOSX;
        public static bool IsXBox => OperatorSys == OperatorSysEnum.XBox;

        public static ISvrHostInfo SvrHostInfo => Ioc.GetService<IQuerySvrHostInfo>()?.GetSvrHostInfo() ?? new SvrHostBaseInfo();

        public static OperatorSysEnum OperatorSys
        {
            get
            {
                OperatorSysEnum operatorSysEnum;
                switch (CurrentPlatformId)
                {
                    case PlatformID.MacOSX:
                        operatorSysEnum = OperatorSysEnum.MacOSX;
                        break;
                    case PlatformID.Unix:
                        operatorSysEnum = OperatorSysEnum.Unix;
                        break;
                    case PlatformID.Xbox:
                        operatorSysEnum = OperatorSysEnum.XBox;
                        break;
                    case PlatformID.Win32NT:
                    case PlatformID.Win32S:
                    case PlatformID.Win32Windows:
                    case PlatformID.WinCE:
                        operatorSysEnum = OperatorSysEnum.Windows;
                        break;
                    default:
                        operatorSysEnum = OperatorSysEnum.Windows;
                        break;
                }

                return operatorSysEnum;
            }
        }


        private static PlatformID CurrentPlatformId => Environment.OSVersion.Platform;

        public static void InitHardInfo(string nodeCode = "", string ip = "")
        {
            if (nodeCode.IsNullOrEmpty() && NodeCode.IsNullOrEmpty())
            {
                nodeCode = GuidHelper.GenerateOrderNumber();
            }
            if (ip.IsNullOrEmpty())
            {
                ip = GetLocalIps().Join();
            }

            if (NodeCode.IsNullOrEmpty())
            {
                NodeCode = nodeCode;
            }
            // if (string.IsNullOrEmpty(NodeIpAddress) || NodeIpAddress == DefaultIp)
            NodeIpAddress = ip;
        }

        public static string GetLocalIp(bool onlyIpV4 = true)
        {
            var ip = GetLocalIps().FirstOrDefault() ?? string.Empty;
            if (onlyIpV4)
            {
                ip = ip.IpV6ToIpV4();
            }
            return ip;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetLocalIps()
        {
            var validAddressFamilies = new List<AddressFamily>
            {
                AddressFamily.InterNetwork,
                AddressFamily.InterNetworkV6,
            };

            try
            {
                if (!NodeIpAddress.IsNullOrEmpty()) return NodeIpAddress.Split(',');

                var ipList = NetworkInterface.GetAllNetworkInterfaces()?
                    .Where(m => m.OperationalStatus == OperationalStatus.Up)?
                    .SelectMany(m => m.GetIPProperties().UnicastAddresses)
                    ?.Where(m =>
                        validAddressFamilies.Contains(m.Address.AddressFamily) && IPAddress.IsLoopback(m.Address) &&
                        IsUnix
                            ? true
                            : m.IsDnsEligible)
                    ?.Select(m => m.Address.ToString())
                    ?.ToList() ?? new List<string>();

                if (!ipList.Any())
                {
                    ipList.Add(DefaultIp);
                }

                return ipList;
            }
            catch (PlatformNotSupportedException ex)
            {
                try
                {
                    // UnixUnicastIPAddressInformation 未实现 IsDnsEligible.get
                    $"OperatorSysEnum is {OperatorSys} handler error {nameof(PlatformNotSupportedException)} : {ex.Message}".Log2Console(ex);

                    //LogHelper.Warn(
                    //    $"OperatorSysEnum is {OperatorSys} handler error {nameof(PlatformNotSupportedException)} : {ex.Message}",
                    //    ex, appendMessage: false);

                    return NetworkInterface.GetAllNetworkInterfaces()
                        ?.Select(m => m.GetIPProperties())
                        ?.SelectMany(m => m.UnicastAddresses)
                        ?.Where(m =>
                            validAddressFamilies.Contains(m.Address.AddressFamily) && IPAddress.IsLoopback(m.Address))
                        ?.Select(m => m.Address.ToString())
                        ?.ToList() ?? new List<string> { DefaultIp };
                }
                catch
                {
                    return new List<string> { DefaultIp };
                }
            }
            catch (Exception ex)
            {
                $"NetworkInterface handler error {ex.Message}".Log2Console(ex);
                // LogHelper.Error($"NetworkInterface handler error {ex.Message}", ex, appendMessage: false);
                return new List<string> { DefaultIp };
            }
        }

        /// <summary>
        ///     Converts an Olson time zone ID to a Windows time zone ID.
        /// </summary>
        /// <param name="olsonTimeZoneId">
        ///     An Olson time zone ID. See
        ///     http://unicode.org/repos/cldr-tmp/trunk/diff/supplemental/zone_tzid.html.
        /// </param>
        /// <returns>
        ///     The TimeZoneInfo corresponding to the Olson time zone ID,
        ///     or null if you passed in an invalid Olson time zone ID.
        /// </returns>
        /// <remarks>
        ///     See http://unicode.org/repos/cldr-tmp/trunk/diff/supplemental/zone_tzid.html
        /// </remarks>
        public static TimeZoneInfo? OlsonTimeZoneToTimeZoneInfo(string olsonTimeZoneId)
        {
            try
            {
                if (OperatorSys != OperatorSysEnum.Windows)
                    return TimeZoneInfo.FindSystemTimeZoneById(olsonTimeZoneId);
                var windowsTimeZoneId = olsonWindowsTimes.SafeGetValue(olsonTimeZoneId);
                if (windowsTimeZoneId.IsNullOrEmpty())
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(olsonTimeZoneId);
                }

                return TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId);

                //if (olsonWindowsTimes.TryGetValue(olsonTimeZoneId, out var windowsTimeZoneId))
                //{
                //    return TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId);
                //}
                //else
                //{
                //    return TimeZoneInfo.FindSystemTimeZoneById(olsonTimeZoneId);
                //}
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static object Obj()
        {
            return new
            {
                NodeCode,
                NodeIpAddress,
                EnvironmentName,
                ApplicationName,
                SvrHostInfo,
                OperatorSys = OperatorSys.ToString(),
            };
        }

        public new static string ToString() => Obj().ToJsonStr();

        public static string ToString(Formatting formatting) => Obj().ToJsonStr(formatting);


    }


}
