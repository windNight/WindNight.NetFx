using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using WindNight.Extension.Logger.DcLog.Abstractions;

namespace WindNight.Extension.Logger.DcLog
{

    public static class DcLoggerExtensions
    {
        /// <summary> </summary>
        public static IDcLoggerProcessor LoggerProcessor;

        /// <summary> </summary>
        public static DcLogOptions DcLogOptions => Ioc.GetService<IOptionsMonitor<DcLogOptions>>().CurrentValue;

        public static IServiceCollection AddDcLogger(this IServiceCollection services,
            Action<DcLogOptions> configure,
            IDcLoggerProcessor loggerProcessor = null)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));
            services.Configure(configure);
            services.AddSingleton<DcLogOptions>();

            services.AddInternalLog(loggerProcessor);

            return services;
        }


        public static IServiceCollection AddDcLogger(this IServiceCollection services,
            IConfiguration configuration = null,
            IDcLoggerProcessor loggerProcessor = null)
        {
            if (configuration == null) configuration = services.BuildServiceProvider().GetService<IConfiguration>();
            services.ConfigureOption<DcLogOptions>(configuration);
            // var configValue = services.BuildServiceProvider().GetService<IOptionsMonitor<DbLogOptions>>().CurrentValue;

            services.AddInternalLog(loggerProcessor);
            return services;
        }


        /// <summary>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="loggerProcessor"></param>
        /// <returns></returns>
        public static ILoggingBuilder AddDcLogger(this ILoggingBuilder builder,
            IConfiguration configuration = null,
            IDcLoggerProcessor loggerProcessor = null)
        {
            builder.AddConfiguration();
            var services = builder.Services;
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, DcLoggerProvider>());
            LoggerProviderOptions.RegisterProviderOptions<DcLogOptions, DcLoggerProvider>(builder.Services);

            //var config = services.BuildServiceProvider().GetServices<IConfiguration>().FirstOrDefault()
            //    .Get<IOptionsMonitor<DbLogOptions>>();
            //services.Configure<DbLogOptions>(services.BuildServiceProvider().GetServices<IConfiguration>()
            //    .FirstOrDefault());

            if (configuration == null) configuration = services.BuildServiceProvider().GetService<IConfiguration>();

            services.ConfigureOption<DcLogOptions>(configuration);
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
        public static ILoggingBuilder AddDcLogger(this ILoggingBuilder builder,
            Action<DcLogOptions> configure,
            IDcLoggerProcessor loggerProcessor = null)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var services = builder.Services;

            services.Configure(configure);
            services.AddSingleton<DcLogOptions>();

            services.AddInternalLog(loggerProcessor);

            return builder;
        }


        /// <summary>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="loggerProcessor"></param>
        public static void UseDcLoggerProcessor(this IServiceCollection services,
            IDcLoggerProcessor loggerProcessor = null)
        {
            services.AddInternalLog(loggerProcessor);
        }

        private static void AddInternalLog(this IServiceCollection services,
            IDcLoggerProcessor loggerProcessor = null)
        {
            services.AddSingleton<ILoggerProvider, DcLoggerProvider>();

            services.AddDcLoggerProcessor(loggerProcessor);

        }



        private static IServiceCollection AddDcLoggerProcessor(this IServiceCollection services,
            IDcLoggerProcessor loggerProcessor = null)
        {
            if (loggerProcessor != null)
            {
                LoggerProcessor = loggerProcessor;
            }
            else
            {
                services.AddSingleton<IDcLoggerProcessor, DcLoggerProcessor>();
                LoggerProcessor = services.BuildServiceProvider().GetService<IDcLoggerProcessor>();
            }

            return services;
        }
    }

}
