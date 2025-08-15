using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text.Extension;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Extension;
using WindNight.Core.Abstractions;
using WindNight.Core.Abstractions.Ex;
using WindNight.Core.ConfigCenter.Extensions;
using WindNight.Core.Extension;
using WindNight.Core.@internal;
using WindNight.Linq.Extensions.Expressions;

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


        public static string NodeCode { get; private set; }

        public static void InitHardInfo(string nodeCode = "", string ip = "")
        {
            if (nodeCode.IsNullOrEmpty() && NodeCode.IsNullOrEmpty())
            {
                nodeCode = GuidHelper.GenerateOrderNumber();
            }
            if (ip.IsNullOrEmpty())
            {
                ip = NodeIpList.Join();
            }

            if (NodeCode.IsNullOrEmpty())
            {
                NodeCode = nodeCode;
            }

            NodeIpAddress = ip;
            SvrMonitorInfo = DefaultSvrMonitorInfo.Gen();
        }

        /// <summary>
        /// 默认的IP
        /// </summary>
        public static string NodeIpAddress { get; private set; } = "";

        public static IEnumerable<string> NodeIpList => GetLocalIps();

        public static string AppId => DefaultConfigItemBase.SystemAppId;
        public static string AppCode => DefaultConfigItemBase.SystemAppCode;
        public static string AppName => DefaultConfigItemBase.SystemAppName;
        public static string BuildType => QuerySvrHostInfoImpl?.QueryBuildType() ?? "";

        public static bool IsUnix => OperatorSys == OperatorSysEnum.Unix;
        public static bool IsWindows => OperatorSys == OperatorSysEnum.Windows;
        public static bool IsMac => OperatorSys == OperatorSysEnum.MacOSX;
        public static bool IsXBox => OperatorSys == OperatorSysEnum.XBox;

        public static OperatorSysEnum OperatorSys
        {
            get
            {
                OperatorSysEnum operatorSysEnum;
                try
                {
                    operatorSysEnum = CurrentPlatformId switch
                    {
                        PlatformID.MacOSX => OperatorSysEnum.MacOSX,
                        PlatformID.Unix => OperatorSysEnum.Unix,
                        PlatformID.Xbox => OperatorSysEnum.XBox,
                        PlatformID.Win32NT or PlatformID.Win32S or PlatformID.Win32Windows or PlatformID.WinCE =>
                            OperatorSysEnum.Windows,
                        _ => OperatorSysEnum.Windows,
                    };
                }
                catch
                {
                    operatorSysEnum = OperatorSysEnum.Unknown;
                }
                return operatorSysEnum;
            }
        }

        public static string OperatorSysName => OperatorSys.ToName();


        private static PlatformID CurrentPlatformId => Environment.OSVersion.Platform;



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
        private static IEnumerable<string> GetLocalIpsV00()
        {
            var validAddressFamilies = new List<AddressFamily>
            {
                AddressFamily.InterNetwork,
                AddressFamily.InterNetworkV6,
            };

            try
            {
                if (!NodeIpAddress.IsNullOrEmpty())
                {
                    return NodeIpAddress.Split(',');
                }

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

        static bool IsDefaultIp(string ipStr)
        {
            return ipStr.IsNullOrEmpty(true) || ipStr == "127.0.0.1" || ipStr == "0.0.0.0" || ipStr == "::1";
        }

        static bool IsNullOrEmptyIp(string ipStr)
        {
            return IsDefaultIp(ipStr);
        }

        public static IEnumerable<string> GetLocalIps()
        {
            // 优先检查是否有预配置的IP地址
            if (!NodeIpAddress.IsNullOrEmpty(true))
            {
                var ips = NodeIpAddress.Split(',').Where(ip => !IsNullOrEmptyIp(ip));
                if (!ips.IsNullOrEmpty(true))
                {
                    return ips;
                }

            }
            var validAddressFamilies = new[] { AddressFamily.InterNetwork, AddressFamily.InterNetworkV6 };
            var ipAddresses = new List<string>();
            try
            {

                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    // 跳过非活动接口和特定类型的接口
                    if (ni.OperationalStatus != OperationalStatus.Up ||
                        ni.NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                        ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel)
                    {
                        continue;
                    }

                    // 获取接口的IP属性
                    var ipProperties = ni.GetIPProperties();
                    if (ipProperties == null)
                    {
                        continue;
                    }

                    // 处理单播地址
                    foreach (var unicastAddr in ipProperties.UnicastAddresses)
                    {
                        if (unicastAddr?.Address == null)
                        {
                            continue;
                        }

                        // 检查地址族和回环状态
                        if (!validAddressFamilies.Contains(unicastAddr.Address.AddressFamily))
                        {
                            continue;
                        }

                        if (IPAddress.IsLoopback(unicastAddr.Address))
                        {
                            continue;
                        }

                        // Windows特有检查
                        if (!IsUnix && !IsDnsEligible(unicastAddr))
                        {
                            continue;
                        }

                        ipAddresses.Add(unicastAddr.Address.ToString());
                    }
                }
                // 如果没有找到IP，返回默认IP 
                var ipList = ipAddresses.Any() ? ipAddresses.Distinct().ToList() : new List<string> { DefaultIp };
                NodeIpAddress = ipList.Join();
                return ipList;
            }
            catch (PlatformNotSupportedException ex)
            {
                var msg = $"Platform OperatorSysEnum is {OperatorSys} handler error  not supported: {ex.Message}";
                msg.Log2Console(ex);
                return FallbackGetLocalIps();
            }
            catch (Exception ex)
            {
                var msg = $"Error getting local IPs: {ex.Message}";
                msg.Log2Console(ex);
                return new List<string> { DefaultIp };
            }
        }

        private static IEnumerable<string> FallbackGetLocalIps()
        {
            try
            {
                // 回退方法：使用 Dns.GetHostAddresses
                var hostAddresses = Dns.GetHostAddresses(Dns.GetHostName());
                return hostAddresses
                    .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork || ip.AddressFamily == AddressFamily.InterNetworkV6)
                    .Select(ip => ip.ToString())
                    .ToList();
            }
            catch (Exception ex)
            {
                var msg = $"  Dns.GetHostAddresses Handler Error: {ex.Message}";
                msg.Log2Console(ex);
                return new List<string> { DefaultIp };
            }
        }

        private static bool IsDnsEligible(UnicastIPAddressInformation addrInfo)
        {
            try
            {
                // 在Windows上检查地址是否适合DNS
                return addrInfo.IsDnsEligible;
            }
            catch
            {
                // 如果无法检查，默认返回true
                return true;
            }
        }



    }

    public partial class HardInfo
    {
        public static IHostEnvironment HostEnv => Ioc.GetService<IHostEnvironment>();
        public static string EnvironmentName => HostEnv?.EnvironmentName ?? "";
        public static string ApplicationName => HostEnv?.ApplicationName ?? "";
        public static string ContentRootPath => HostEnv?.ContentRootPath ?? "";

        public static IQuerySvrHostInfo QuerySvrHostInfoImpl => Ioc.GetService<IQuerySvrHostInfo>();

        public static string QueryBuildType() => QuerySvrHostInfoImpl?.QueryBuildType() ?? "";

        public static long QuerySvrRegisteredTs() => SvrMonitorInfo?.RegisteredTs ?? 0L;

        public static long QueryBuildTs() => QuerySvrBuildInfo().QueryBuildInfoItem("BuildTs", 0L);

        public static string QueryBuildDateTime() => QuerySvrBuildInfo().QueryBuildInfoItem("BuildTime", "");

        public static string QueryBuildVersion() => QuerySvrBuildInfo().QueryBuildInfoItem("BuildVersion", "");

        public static string QueryBuildUserName() => QuerySvrBuildInfo().QueryBuildInfoItem("UserName", "");

        public static string QueryBuildMachineName() => QuerySvrBuildInfo().QueryBuildInfoItem("MachineName", "");

        public static string QueryBuildProjectName() => QuerySvrBuildInfo().QueryBuildInfoItem("BuildProjectName", "");

        public static int QueryRuntimeProcessId() => QuerySvrRuntimeInfo().ProcessId;

        public static string QueryRunMachineUserName() => QuerySvrRuntimeInfo()?.RunMachineUserName ?? "";

        public static string QueryRunMachineName()
        {
            try
            {
                //var implName = QuerySvrHostInfoImpl?.QueryBuildMachineName() ?? "";
                //if (!implName.IsNullOrEmpty())
                //{
                //    return implName;
                //}
                var implName = QuerySvrRuntimeInfo()?.RunMachineName ?? "";
                if (!implName.IsNullOrEmpty())
                {
                    return implName;
                }
                return Environment.MachineName;
            }
            catch (Exception ex)
            {
                return "";
            }

        }


        public static ISvrMonitorInfo SvrMonitorInfo { get; private set; }

        /// <summary>
        ///  服务基础信息相关  AppId, AppCode, AppName 等
        /// </summary>
        public static IAppBaseInfo QueryAppBaseInfo() => DefaultAppBaseInfo.Gen();

        public static ISvrRuntimeInfo QuerySvrRuntimeInfo() => SvrRuntimeInfo.Gen();

        /// <summary>
        ///  包含服务基础信息外 包含服务的主程序的相关信息
        /// </summary>
        public static ISvrHostInfo QuerySvrHostInfo() => QuerySvrHostInfoImpl?.QuerySvrHostInfo() ?? SvrHostBaseInfo.Gen();

        public static ISvrBuildInfo QuerySvrBuildInfo() => QuerySvrHostInfoImpl?.QuerySvrBuildInfo() ?? new SvrBuildInfoDto();
        public static ISvrMonitorInfo GenSvrMonitorInfo(SvrMonitorTypeEnum monitorType) => DefaultSvrMonitorInfo.GenSvrMonitorInfo(monitorType);

        public static object Obj()
        {
            var obj = new
            {
                AppId,
                AppCode,
                AppName,
                RegisteredTs = QuerySvrRegisteredTs(),
                BuildType = QueryBuildType() ?? "",
                BuildMachine = QueryBuildMachineName() ?? "",
                ApiVersion = SvrMonitorInfo?.SvrHostInfo?.MainAssemblyInfo?.AssemblyVersion ?? "",
                BuildVersion = QueryBuildVersion() ?? "",
                BuildTime = QueryBuildDateTime() ?? "",
                PId = SvrMonitorInfo?.SvrRuntimeInfo?.ProcessId ?? -1,
                SvrHostInfo = SvrMonitorInfo?.SvrHostInfo,
                SvrBuildInfo = SvrMonitorInfo?.SvrBuildInfo,

            };

            return obj;
        }

        //{
        //    return new
        //    {
        //        NodeCode,
        //        NodeIpAddress,
        //        EnvironmentName,
        //        ApplicationName,
        //        SvrHostInfo,
        //        OperatorSys = OperatorSys.ToString(),
        //    };
        //}

        public new static string ToString() => Obj().ToJsonStr();

        public static string ToString(Formatting formatting) => Obj().ToJsonStr(formatting);

        //public static bool FillBuildExtDict(IReadOnlyDictionary<string, object> dict)
        //{

        //    return AppRegisteredInfo.FillBuildExtDict(dict);

        //}

        public static ISvrRegisterInfo TryGetAppRegInfo(string buildType)
        {
            var sysInfo = SvrMonitorInfo;

            //   sysInfo.ResetBuildType(buildType);

            return sysInfo;

        }

    }



    public partial class HardInfo
    {

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
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(olsonTimeZoneId);
                }
                var windowsTimeZoneId = olsonWindowsTimes.SafeGetValue(olsonTimeZoneId);
                if (windowsTimeZoneId.IsNullOrEmpty())
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(olsonTimeZoneId);
                }

                return TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId);

            }
            catch (Exception)
            {
                return null;
            }
        }


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

    }
}
