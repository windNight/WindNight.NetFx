using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using WindNight.Extension.Logger.DbLog.Extensions;
using WindNight.Extension.Logger.Mysql.DbLog;
using WindNight.LogExtension;

namespace Logger.Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            try
            {
                UseDbLog(args);

            }
            catch (Exception ex)
            {

                throw;
            }

            Console.ReadLine();
        }

        private static void UseDbLog(string[] args)
        {
            const string logAppCode = "TestLogger";
            const string logAppName = "testLogger";
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("appsettings.json", true, true);
                })
                .ConfigureLogging(logging =>
                {
                    //logging.AddDbLogger(opt =>
                    //{
                    //    opt.LogAppCode = "testAppCode";
                    //    opt.LogAppName = "testAppName";
                    //    //opt.Env = EnvEnum.Dev;
                    //    opt.DbLogVersion = "1.0.1";
                    //    opt.MinLogLevel = LogLevel.Debug;
                    //});

                    //logging.AddDbLogger();
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
                }).ConfigureServices((context, services) =>
                {
                    services.AddDbLogger(configure: opt =>
                    {
                        opt.LogAppCode = logAppCode;
                        opt.LogAppName = logAppName;
                        //   opt.DbLogVersion = "1.0.1";
                        opt.MinLogLevel = LogLevel.Debug;
                        opt.IsConsoleLog = true;
                        opt.DbConnectString = "";
                    });
                    
                    Ioc.Instance.InitServiceProvider(services.BuildServiceProvider());
                });

            var host = builder.Build();


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
                log.LogRegisterInfo(buildType, 1, logAppCode, $"{i}_测试");
                var debugMsg = $"{i} I'm Debug Log";
                var infoMsg = $"{i} I'm Info Log";
                var warnMsg = $"{i} I'm Warn Log";
                var errorMsg = $"{i} I'm Error Log";
                var fatalMsg = $"{i} I'm Fatal Log";
                log.Debug(debugMsg);
                log.Info(infoMsg);
                log.Warn(warnMsg);
                log.Error(errorMsg, new Exception("test Error exception"));
                log.Fatal(fatalMsg, new Exception("test Fatal exception"));
                log.ApiUrlCall($"{i}_aaa.html", $"{i} 测试地址统计", 100, "127.0.0.1");
                log.ApiUrlException($"{i}_aaa.html", $"{i} 测试地址统计", new Exception("test ApiUrlException exception"),
                    "127.0.0.1");
                log.LogOfflineInfo(buildType, 1, logAppCode, $"{i}_测试");
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

    }
}