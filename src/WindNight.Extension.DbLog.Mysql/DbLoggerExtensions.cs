using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using System.Configuration;
using WindNight.Extension.Logger.DbLog;
using WindNight.Extension.Logger.DbLog.Abstractions;

namespace WindNight.Extension.Logger.Mysql.DbLog
{
    public static class DbLoggerExtensions
    {
        /// <summary> </summary>
        public static IDbLoggerProcessor LoggerProcessor;

        /// <summary> </summary>
        public static DbLogOptions DbLogOptions => Ioc.GetService<IOptionsMonitor<DbLogOptions>>().CurrentValue;

        public static IServiceCollection AddDbLogger(this IServiceCollection services, Action<DbLogOptions> configure, IDbLoggerProcessor loggerProcessor = null)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));
            services.Configure(configure);
            services.AddSingleton<DbLogOptions>();

            services.AddInternalLog(loggerProcessor);

            return services;
        }


        public static IServiceCollection AddDbLogger(this IServiceCollection services, IConfiguration configuration = null, IDbLoggerProcessor loggerProcessor = null)
        {
            if (configuration == null)
            {
                configuration = services.BuildServiceProvider().GetService<IConfiguration>();
            }
            services.ConfigureOption<DbLogOptions>(configuration);
            // var configValue = services.BuildServiceProvider().GetService<IOptionsMonitor<DbLogOptions>>().CurrentValue;

            services.AddInternalLog(loggerProcessor);
            return services;
        }



        /// <summary>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="loggerProcessor"></param>
        /// <returns></returns>
        public static ILoggingBuilder AddDbLogger(this ILoggingBuilder builder, IConfiguration configuration = null, IDbLoggerProcessor loggerProcessor = null)
        {
            builder.AddConfiguration();
            var services = builder.Services;
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, DbLoggerProvider>());
            LoggerProviderOptions.RegisterProviderOptions<DbLogOptions, DbLoggerProvider>(builder.Services);

            //var config = services.BuildServiceProvider().GetServices<IConfiguration>().FirstOrDefault()
            //    .Get<IOptionsMonitor<DbLogOptions>>();
            //services.Configure<DbLogOptions>(services.BuildServiceProvider().GetServices<IConfiguration>()
            //    .FirstOrDefault());

            if (configuration == null)
            {
                configuration = services.BuildServiceProvider().GetService<IConfiguration>();
            }

            services.ConfigureOption<DbLogOptions>(configuration);
            // services.AddSingleton<DbLogOptions>();
            // var configValue = services.BuildServiceProvider().GetService<IOptionsMonitor<DbLogOptions>>().CurrentValue;

            services.AddInternalLog(loggerProcessor);

            return builder;
        }

        /// <summary>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configure"></param>
        /// <param name="loggerProcessor"></param>
        /// <returns></returns>
        public static ILoggingBuilder AddDbLogger(this ILoggingBuilder builder, Action<DbLogOptions> configure,
            IDbLoggerProcessor loggerProcessor = null)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var services = builder.Services;

            services.Configure(configure);
            services.AddSingleton<DbLogOptions>();

            services.AddInternalLog(loggerProcessor);

            return builder;
        }


        /// <summary>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="loggerProcessor"></param>
        public static void UseDbLoggerProcessor(this IServiceCollection services, IDbLoggerProcessor loggerProcessor = null)
        {

            services.AddInternalLog(loggerProcessor);

        }

        static void AddInternalLog(this IServiceCollection services, IDbLoggerProcessor loggerProcessor = null)
        {
            services.AddSingleton<ILoggerProvider, DbLoggerProvider>();
            services.AddDbLoggerProcessor(loggerProcessor);
            services.AddDefaultMysqlLogsProcess();
        }

        /// <summary>
        /// </summary>
        /// <param name="services"></param> 
        /// <returns></returns>
        public static IServiceCollection AddDefaultMysqlLogsProcess(this IServiceCollection services)
        {
            services.AddSingleton<ISystemLogsProcess, MysqlLogsProcess>();
            return services;
        }

        private static IServiceCollection AddDbLoggerProcessor(this IServiceCollection services, IDbLoggerProcessor loggerProcessor = null)
        {
            if (loggerProcessor != null)
            {
                LoggerProcessor = loggerProcessor;
            }
            else
            {
                services.AddSingleton<IDbLoggerProcessor, DbLoggerProcessor>();
                LoggerProcessor = services.BuildServiceProvider().GetService<IDbLoggerProcessor>();
            }
            return services;
        }

    }
}
