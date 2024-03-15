
using System.Configuration;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Newtonsoft.Json.Extension;
using WindNight.ConfigCenter.Extension;
using WindNight.Core.Abstractions;
using WindNight.Extension.Logger.DcLog;
using WindNight.Extension.Logger.DcLog.Abstractions;
using WindNight.Extension.Logger.DcLog.Extensions;
using WindNight.LogExtension;

namespace WebApiDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var buildType = "";
#if DEBUG
            buildType = "Debug";
#else
            buildType = "Release";
#endif

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            IServiceCollection services = builder.Services;
            IConfiguration configuration = builder.Configuration;

            builder.Configuration.SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("Config/AppSettings.json", false, true)

                ;

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            services.AddTransient<ILogService, DefaultLogService>();

            LogHelper.RegisterProcessEvent(LogHelper.Log4NetSubscribe);

            services.AddDefaultEgDcLogService(configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            Ioc.Instance.InitServiceProvider(app.Services);

            LogHelper.LogRegisterInfo(buildType);
            app.Run();
            LogHelper.LogOfflineInfo(buildType);


        }
    }

    public static partial class LogsExtension
    {
        public static IServiceCollection AddDefaultEgDcLogService(this IServiceCollection services,
    IConfiguration configuration,
    IDcLoggerProcessor loggerProcessor = null)
        {

            //LogHelper.RegisterProcessEvent(EgLogHelper.Log4NetSubscribe);
            //services.AddTransient<ILogService, LogService>();

            //  var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
            var configValue = configuration.GetSectionValue<DcLogOptions>();
            Action<DcLogOptions> configure;
            if (configValue != null && !configValue.HostName.IsNullOrEmpty())
            {
                var hostName = configValue.HostName;
                configure = opt =>
                {
                    opt.LogAppCode = configuration.GetValue<string>("AppSettings:AppCode");
                    opt.LogAppName = configuration.GetValue<string>("AppSettings:AppName");
                    opt.LogAppId = configuration.GetValue<int>("AppSettings:AppId");
                    opt.MinLogLevel = configValue.MinLogLevel;

                    opt.HostName = hostName;
                    opt.Port = configValue.Port;

                    opt.IsOpenDebug = configValue.IsOpenDebug;
                    opt.OpenGZip = configValue.OpenGZip;
                    opt.IsConsoleLog = configValue.IsConsoleLog;
                    opt.DcLogVersion = configValue.DcLogVersion;
                    opt.ContentMaxLength = configValue.ContentMaxLength;
                    opt.QueuedMaxMessageCount = configValue.QueuedMaxMessageCount;


                };
            }
            else
            {
                configure = opt =>
                {
                    opt.LogAppCode = configuration.GetValue<string>("AppSettings:AppCode");
                    opt.LogAppName = configuration.GetValue<string>("AppSettings:AppName");
                    opt.LogAppId = configuration.GetValue<int>("AppSettings:AppId");
                    opt.MinLogLevel = LogLevel.Information;

                    opt.HostName = "192.168.2.103";
                    opt.Port = 2555;


                    opt.IsOpenDebug = false;
                    opt.OpenGZip = false;
                    opt.IsConsoleLog = false;
                    opt.DcLogVersion = "1.0.0";
                    opt.ContentMaxLength = 2000;
                    opt.QueuedMaxMessageCount = 1024;


                };
            }

            if (configure == null)
            {
                throw new ArgumentNullException($"DcLogOptions");
            }
            LogHelper.RegisterProcessEvent(DcLogSubscribe);
            services.AddDcLogger(configure, loggerProcessor);

            return services;

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
