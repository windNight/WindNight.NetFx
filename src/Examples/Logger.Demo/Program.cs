using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Extension;
using Newtonsoft.Json.Linq;
using WindNight.Extension.Logger.DbLog.Extensions;
using WindNight.Extension.Logger.DcLog;
using WindNight.Extension.Logger.DcLog.Extensions;
using WindNight.Extension.Logger.Mysql.DbLog;
using WindNight.LogExtension;
using LoggerExtensions = WindNight.Extension.Logger.DbLog.Extensions.LoggerExtensions;

namespace Logger.Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // UseDbLogInConfigureServices(args);
                UseDcLogInConfigureServices(args);
                DoTestLogV2(args);
            }
            catch (Exception ex)
            {

                throw;
            }
            Console.WriteLine("Hello, World!");

            Console.ReadLine();
        }

        const string logAppCode = "TestLogger";
        const string logAppName = "testLogger";

        private static IHost UseDbLogInConfigureLogging(string[] args)
        {

            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("appsettings.json", true, true);
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.AddFilter((provider, category, logLevel) =>
                    {
                        if (provider.IsNullOrEmpty())
                        {
                            return false;
                        }
                        if (provider.Contains("ConsoleLoggerProvider")
                            && category.Contains("Controller")
                            && logLevel >= LogLevel.Information)
                        { return true; }
                        if (provider.Contains("ConsoleLoggerProvider")
                            && category.Contains("Microsoft")
                            && logLevel >= LogLevel.Information)
                        { return true; }
                        return false;
                    });
                    var configuration = context.Configuration;
                    logging.AddDbLogger(configuration);

                }).ConfigureServices((context, services) =>
                {
                    var configuration = context.Configuration;
                    // services.AddDbLogger(configuration: configuration);
                    LogHelper.RegisterProcessEvent(DbLogSubscribe);
                    Ioc.Instance.InitServiceProvider(services.BuildServiceProvider());
                });

            var host = builder.Build();

            return host;

        }


        private static IHost UseDbLogInConfigureServices(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("appsettings.json", true, true);
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.AddFilter((provider, category, logLevel) =>
                    {
                        if (provider.Contains("ConsoleLoggerProvider")
                            && category.Contains("Controller")
                            && logLevel >= LogLevel.Information)
                            return true;
                        if (provider.Contains("ConsoleLoggerProvider")
                            && category.Contains("Microsoft")
                            && logLevel >= LogLevel.Information)
                            return true;
                        return false;
                    });
                    var configuration = context.Configuration;
                    // logging.AddDbLogger(configuration);

                }).ConfigureServices((context, services) =>
                {
                    //services.AddDbLogger(configure: opt =>
                    //{
                    //    opt.LogAppCode = logAppCode;
                    //    opt.LogAppName = logAppName;
                    //    //   opt.DbLogVersion = "1.0.1";
                    //    opt.MinLogLevel = LogLevel.Information;
                    //    opt.IsConsoleLog = true;
                    //    opt.DbConnectString = "";
                    //});
                    var configuration = context.Configuration;
                    services.AddDbLogger(configuration: configuration);
                    LogHelper.RegisterProcessEvent(DbLogSubscribe);
                    Ioc.Instance.InitServiceProvider(services.BuildServiceProvider());
                });

            var host = builder.Build();
            return host;
        }


        private static IHost UseDcLogInConfigureServices(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("appsettings.json", true, true);
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.AddFilter((provider, category, logLevel) =>
                    {
                        if (provider.Contains("ConsoleLoggerProvider")
                            && category.Contains("Controller")
                            && logLevel >= LogLevel.Information)
                            return true;
                        if (provider.Contains("ConsoleLoggerProvider")
                            && category.Contains("Microsoft")
                            && logLevel >= LogLevel.Information)
                            return true;
                        return false;
                    });
                    var configuration = context.Configuration;
                    // logging.AddDbLogger(configuration);

                }).ConfigureServices((context, services) =>
                {
                    //services.AddDbLogger(configure: opt =>
                    //{
                    //    opt.LogAppCode = logAppCode;
                    //    opt.LogAppName = logAppName;
                    //    //   opt.DbLogVersion = "1.0.1";
                    //    opt.MinLogLevel = LogLevel.Information;
                    //    opt.IsConsoleLog = true;
                    //    opt.DbConnectString = "";
                    //});
                    var configuration = context.Configuration;
                    services.AddDcLogger(configuration: configuration);
                    LogHelper.RegisterProcessEvent(DcLogSubscribe);
                    Ioc.Instance.InitServiceProvider(services.BuildServiceProvider());
                    DcLogHelper.Warn($"test warn");
                });

            var host = builder.Build();
            return host;
        }


        private static void DoTestLogV1(string[] args)
        {

            var log = Ioc.GetService<ILoggerProvider>().CreateLogger("test");


            var buildType = "";
#if DEBUG
            buildType = "Debug";
#else
            buildType = "Release";
#endif
            var limit = 1;
            Parallel.For(0, limit, i =>
            {
                LoggerExtensions.LogRegisterInfo(log, buildType, 1, logAppCode, $"{i}_测试");
                var debugMsg = $"{i} I'm Debug Log";
                var infoMsg = $"{i} I'm Info Log";
                var warnMsg = $"{i} I'm Warn Log";
                var errorMsg = $"{i} I'm Error Log";
                var fatalMsg = $"{i} I'm Fatal Log";
                LoggerExtensions.Debug(log, debugMsg);
                LoggerExtensions.Info(log, infoMsg);
                LoggerExtensions.Warn(log, warnMsg);
                LoggerExtensions.Error(log, errorMsg, new Exception("test Error exception"));
                LoggerExtensions.Fatal(log, fatalMsg, new Exception("test Fatal exception"));
                LoggerExtensions.ApiUrlCall(log, $"{i}_aaa.html", $"{i} 测试地址统计", 100, "127.0.0.1");
                LoggerExtensions.ApiUrlException(log, $"{i}_aaa.html", $"{i} 测试地址统计", new Exception("test ApiUrlException exception"),
                    "127.0.0.1");
                LoggerExtensions.LogOfflineInfo(log, buildType, 1, logAppCode, $"{i}_测试");
                var jo = JObject.FromObject(new
                {
                    Url = $"{i} I'm Url http://",
                    UA = $"{i} I'm UA",
                    Api = $"{i} I'm WebApi",
                    HttpMethod = HttpMethod.Get.ToString(),
                    Header = $"{i} I'm Header",
                    Request = $"{i} I'm Request",
                    Response = $"{i} I'm Response",
                    HttpStatusCode = 200,
                    content = $"{i} I'm Msg",
                    logAppCode,
                    logAppName
                });
                //log.Report(jo);
                DbLogHelper.LogRegisterInfo(buildType, 1, logAppCode, $"{i}_测试_DbLogHelper");
                DbLogHelper.Debug($"{debugMsg} Use DbLogHelper ");
                DbLogHelper.Info($"{infoMsg} Use DbLogHelper ");
                DbLogHelper.Warn($"{warnMsg}  Use DbLogHelper ");
                DbLogHelper.Error($"{errorMsg}  Use DbLogHelper ",
                    new Exception("test Error exception Use DbLogHelper"));
                DbLogHelper.Fatal($"{fatalMsg}  Use DbLogHelper ",
                    new Exception("test Fatal exception Use DbLogHelper"));
                DbLogHelper.ApiUrlCall($"{i}_aaa.html Use DbLogHelper", "测试地址统计 Use DbLogHelper", 100, "127.0.0.1");
                DbLogHelper.ApiUrlException($"{i}_aaa.html Use DbLogHelper", "测试地址统计 Use DbLogHelper",
                    new Exception("test ApiUrlException exception"), "127.0.0.1");
                DbLogHelper.LogOfflineInfo(buildType, 1, logAppCode, $"{i}_测试_DbLogHelper");
                var jo2 = JObject.FromObject(new
                {
                    Url = $"{i} I'm Url http:// Use DbLogHelper",
                    UA = $"{i} I'm UA Use DbLogHelper",
                    Api = $"{i} I'm WebApi Use DbLogHelper",
                    HttpMethod = HttpMethod.Get.ToString(),
                    Header = $"{i} I'm Header Use DbLogHelper",
                    Request = $"{i} I'm Request Use DbLogHelper",
                    Response = $"{i} I'm Response Use DbLogHelper",
                    HttpStatusCode = 200,
                    content = $"{i} I'm Msg Use DbLogHelper",
                    logAppCode,
                    logAppName
                });
                //   DbLogHelper.Report(jo2);

                LogHelper.LogRegisterInfo(buildType, 1, logAppCode, $"{i}_测试_LogHelper");
                LogHelper.Debug($"{debugMsg} Use LogHelper ");
                LogHelper.Info($"{infoMsg} Use LogHelper ");
                LogHelper.Warn($"{warnMsg}  Use LogHelper ");
                LogHelper.Error($"{errorMsg}  Use LogHelper ", new Exception("test Error exception Use LogHelper"));
                LogHelper.Fatal($"{fatalMsg}  Use LogHelper ", new Exception("test Fatal exception Use LogHelper"));
                LogHelper.ApiUrlCall($"{i}_aaa.html Use LogHelper", "测试地址统计 Use LogHelper", 100, "127.0.0.1");
                LogHelper.ApiUrlException($"{i}_aaa.html Use LogHelper", "测试地址统计 Use LogHelper",
                    new Exception("test ApiUrlException exception"), "127.0.0.1");
                LogHelper.LogOfflineInfo(buildType, 1, logAppCode, $"{i}_测试_LogHelper");

                var jo3 = JObject.FromObject(new
                {
                    Url = $"{i} I'm Url http:// Use LogHelper",
                    UA = $"{i} I'm UA Use LogHelper",
                    Api = $"{i} I'm WebApi Use LogHelper",
                    HttpMethod = HttpMethod.Get.ToString(),
                    Header = $"{i} I'm Header Use LogHelper",
                    Request = $"{i} I'm Request Use LogHelper",
                    Response = $"{i} I'm Response Use LogHelper",
                    HttpStatusCode = 200,
                    content = $"{i} I'm Msg Use LogHelper",
                    logAppCode,
                    logAppName
                });
            });
        }

        private static void DoTestLogV2(string[] args)
        {

            // var log = Ioc.GetService<ILoggerProvider>().CreateLogger("test");


            var buildType = "";
#if DEBUG
            buildType = "Debug";
#else
            buildType = "Release";
#endif
            var limit = 1;
            Parallel.For(0, limit, i =>
            {
                TestOnlineLog(i);

                TestDebugLog(i);
                TestInfoLog(i);
                TestWarnLog(i);
                TestErrorLog(i);
                TestFatalLog(i);

                TestOfflineLog(i);

            });
        }


        static void TestOnlineLog(int i)
        {
            var buildType = "";
#if DEBUG
            buildType = "Debug";
#else
            buildType = "Release";
#endif

            var log = Ioc.GetService<ILoggerProvider>().CreateLogger("log:online");

            var debugMsg = $"{i} I'm Debug Log";
            LoggerExtensions.Debug(log, debugMsg);

            LoggerExtensions.LogRegisterInfo(log, buildType, 1, logAppCode, $"{i}_测试");
            DbLogHelper.LogRegisterInfo(buildType, 1, logAppCode, $"{i}_测试_DbLogHelper");
            DcLogHelper.LogRegisterInfo(buildType, 1, logAppCode, $"{i}_测试_DcLogHelper");
            LogHelper.LogRegisterInfo(buildType, 1, logAppCode, $"{i}_测试_LogHelper");


        }
        static void TestOfflineLog(int i)
        {
            var buildType = "";
#if DEBUG
            buildType = "Debug";
#else
            buildType = "Release";
#endif

            var log = Ioc.GetService<ILoggerProvider>().CreateLogger("log:offline");

            var debugMsg = $"{i} I'm Debug Log";
            LoggerExtensions.Debug(log, debugMsg);

            LoggerExtensions.LogOfflineInfo(log, buildType, 1, logAppCode, $"{i}_测试");
            DbLogHelper.LogOfflineInfo(buildType, 1, logAppCode, $"{i}_测试_DbLogHelper");
            DcLogHelper.LogOfflineInfo(buildType, 1, logAppCode, $"{i}_测试_DcLogHelper");
            LogHelper.LogOfflineInfo(buildType, 1, logAppCode, $"{i}_测试_LogHelper");


        }

        static void TestDebugLog(int i)
        {
            var buildType = "";
#if DEBUG
            buildType = "Debug";
#else
            buildType = "Release";
#endif

            var log = Ioc.GetService<ILoggerProvider>().CreateLogger("log:debug");
            var debugMsg = $"{i} I'm Debug Log";

            LoggerExtensions.Debug(log, $"{debugMsg} use ILoggerProvider");
            DbLogHelper.Debug($"{debugMsg} Use DbLogHelper ");
            DcLogHelper.Debug($"{debugMsg} Use DcLogHelper ");
            LogHelper.Debug($"{debugMsg} Use LogHelper ");


        }

        static void TestInfoLog(int i)
        {
            var buildType = "";
#if DEBUG
            buildType = "Debug";
#else
            buildType = "Release";
#endif

            var log = Ioc.GetService<ILoggerProvider>().CreateLogger("log:info");
            var infoMsg = $"{i} I'm Info Log";

            LoggerExtensions.Info(log, $"{infoMsg} use ILoggerProvider");
            DbLogHelper.Info($"{infoMsg} Use DbLogHelper ");
            DcLogHelper.Info($"{infoMsg} Use DcLogHelper ");
            LogHelper.Info($"{infoMsg} Use LogHelper ");


        }

        static void TestWarnLog(int i)
        {
            var buildType = "";
#if DEBUG
            buildType = "Debug";
#else
            buildType = "Release";
#endif

            var log = Ioc.GetService<ILoggerProvider>().CreateLogger("log:warn");
            var warnMsg = $"{i} I'm Warn Log";

            LoggerExtensions.Warn(log, $"{warnMsg} use ILoggerProvider");
            DbLogHelper.Warn($"{warnMsg}  Use DbLogHelper ");
            DcLogHelper.Warn($"{warnMsg}  Use DcLogHelper ");
            LogHelper.Warn($"{warnMsg}  Use LogHelper ");


        }

        static void TestErrorLog(int i)
        {
            var buildType = "";
#if DEBUG
            buildType = "Debug";
#else
            buildType = "Release";
#endif

            var log = Ioc.GetService<ILoggerProvider>().CreateLogger("log:error");
            var errorMsg = $"{i} I'm Error Log";

            LoggerExtensions.Error(log, $"{errorMsg} use ILoggerProvider", new Exception("test Error exception"));
            DbLogHelper.Error($"{errorMsg}  Use DbLogHelper ",
                new Exception("test Error exception Use DbLogHelper"));

            DcLogHelper.Error($"{errorMsg}  Use DcLogHelper ",
                new Exception("test Error exception Use DcLogHelper"));

            LogHelper.Error($"{errorMsg}  Use LogHelper ", new Exception("test Error exception Use LogHelper"));


        }

        static void TestFatalLog(int i)
        {
            var buildType = "";
#if DEBUG
            buildType = "Debug";
#else
            buildType = "Release";
#endif

            var log = Ioc.GetService<ILoggerProvider>().CreateLogger("log:fatal");


            var fatalMsg = $"{i} I'm Fatal Log";
            LoggerExtensions.Fatal(log, $"{fatalMsg} use ILoggerProvider", new Exception("test Fatal exception"));
            DbLogHelper.Fatal($"{fatalMsg}  Use DbLogHelper ",
                new Exception("test Fatal exception Use DbLogHelper"));
            DcLogHelper.Fatal($"{fatalMsg}  Use DcLogHelper ",
                new Exception("test Fatal exception Use DcLogHelper"));


            LogHelper.Fatal($"{fatalMsg}  Use LogHelper ", new Exception("test Fatal exception Use LogHelper"));


        }

        internal static void DbLogSubscribe(LogHelper.LogInfo? logInfo)
        {
            try
            {
                if (logInfo == null) return;
                // if (!ConfigItems.IsReportToE) return;
#if !NET45
                DbLogHelper.Add(logInfo.Content, logInfo.Level, logInfo.Exceptions, logInfo.SerialNumber,
                    logInfo.Timestamps, logInfo.RequestUrl, logInfo.ServerIp, logInfo.ClientIp);
#endif
            }
            catch (Exception e)
            {
                Console.WriteLine("上报日志异常:{0}", e.ToJsonStr());
                // Log(LogLevels.Warning, $"EsLogAsync({logInfo.ToJsonStr()}) error {e.Message}", e);
            }
        }

        internal static void DcLogSubscribe(LogHelper.LogInfo? logInfo)
        {
            try
            {
                if (logInfo == null) return;
                // if (!ConfigItems.IsReportToE) return;
#if !NET45
                DcLogHelper.Add(logInfo.Content, logInfo.Level, logInfo.Exceptions, logInfo.SerialNumber,
                    logInfo.Timestamps, logInfo.RequestUrl, logInfo.ServerIp, logInfo.ClientIp);
#endif
            }
            catch (Exception e)
            {
                Console.WriteLine("上报日志异常:{0}", e.ToJsonStr());
                // Log(LogLevels.Warning, $"EsLogAsync({logInfo.ToJsonStr()}) error {e.Message}", e);
            }
        }


    }
}
