using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WnExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WindNight.Core.Abstractions;
using WindNight.Core.ExceptionExt;
using WindNight.Core.SysLogCenter.Extensions;

namespace WindNight.AspNetCore.Hosting
{
    public partial class DefaultProgramBase
    {
        private static ILogService _logService => Ioc.GetService<ILogService>();

        public void Init(
            Func<string, string[], IHostBuilder> createHostBuilder,
            Func<string> buildTypeFunc,
            Action actBeforeRun,
            string[] args)
        {
            var buildType = buildTypeFunc();
            Init(createHostBuilder, buildType, actBeforeRun, args);
        }


        public static void Init(
            Func<string, string[], IHostBuilder> createHostBuilder,
            string buildType,
            Action actBeforeRun,
            string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionEventHandler;
            TaskScheduler.UnobservedTaskException += UnobservedTaskHandler;
            var hostBuilder = ProgramBase.CreateHostBuilder(createHostBuilder, buildType, args);
            actBeforeRun();
            hostBuilder.Run();
            DefaultLogHelperBase.LogOfflineInfo(buildType);
            Thread.Sleep(1000);
        }


        public async Task InitAsync(
            Func<string, string[], IHostBuilder> createHostBuilder,
            Func<string> buildTypeFunc,
            Func<Task> actBeforeRun,
            string[] args)
        {
            var buildType = buildTypeFunc();
            await InitAsync(createHostBuilder, buildType, actBeforeRun, args);
        }


        public static async Task InitAsync(
            Func<string, string[], IHostBuilder> createHostBuilder,
            string buildType,
            Func<Task> actBeforeRun,
            string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionEventHandler;
            TaskScheduler.UnobservedTaskException += UnobservedTaskHandler;
            var hostBuilder = ProgramBase.CreateHostBuilder(createHostBuilder, buildType, args);

            await actBeforeRun();

            await hostBuilder.RunAsync();
            DefaultLogHelperBase.LogOfflineInfo(buildType);
            Thread.Sleep(1000);
        }

        private static void UnobservedTaskHandler(object sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                var ex = e.Exception;
                _logService?.Error($"DefaultProgramBase.UnobservedTaskException {ex?.GetMessage()} ", ex);
                e.SetObserved();
            }
            catch
            {
            }
        }

        private static void UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var ex = e.ExceptionObject as Exception;
                _logService?.Fatal($"DefaultProgramBase.UnhandledException {ex?.GetMessage()} ", ex);
            }
            catch
            {
            }
        }
    }


    public partial class DefaultProgramBase
    {
        /// <summary>GenericHostBuilder</summary>
        /// <param name="buildType"> Release|Debug </param>
        /// <param name="appConfigurationConfigureDelegate">
        ///     used in
        ///     <see
        ///         cref="M:Microsoft.Extensions.Hosting.GenericHostBuilderExtensions.ConfigureWebHostDefaults(Microsoft.Extensions.Hosting.IHostBuilder,System.Action{Microsoft.AspNetCore.Hosting.IWebHostBuilder})" />
        ///     .
        /// </param>
        /// <param name="webHostConfigure">
        ///     used in
        ///     <see
        ///         cref="M:Microsoft.Extensions.Hosting.GenericHostBuilderExtensions.ConfigureWebHostDefaults(Microsoft.Extensions.Hosting.IHostBuilder,System.Action{Microsoft.AspNetCore.Hosting.IWebHostBuilder})" />
        /// </param>
        /// <param name="configureLogging">
        ///     used in
        ///     <see
        ///         cref="M:Microsoft.Extensions.Hosting.HostingHostBuilderExtensions.ConfigureLogging(Microsoft.Extensions.Hosting.IHostBuilder,System.Action{Microsoft.Extensions.Logging.ILoggingBuilder})" />
        /// </param>
        /// <param name="configureServicesDelegate">
        ///     used in
        ///     <see
        ///         cref="M:Microsoft.Extensions.Hosting.IHostBuilder.ConfigureServices(System.Action{Microsoft.Extensions.Hosting.HostBuilderContext,Microsoft.Extensions.DependencyInjection.IServiceCollection})" />
        /// </param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilderDefaults(
            string buildType,
            Action<IConfigurationBuilder> appConfigurationConfigureDelegate,
            Action<IWebHostBuilder> webHostConfigure,
            Action<ILoggingBuilder>? configureLogging = null,
            Action<HostBuilderContext, IServiceCollection>? configureServicesDelegate = null)
        {
            return CreateHostBuilderDefaults(buildType, null, appConfigurationConfigureDelegate, webHostConfigure,
                configureLogging, configureServicesDelegate);
        }

        public static IHostBuilder CreateHostBuilderDefaults(
            string buildType,
            Action<HostBuilderContext, IConfigurationBuilder> appConfigurationConfigureDelegate,
            Action<IWebHostBuilder> webHostConfigure,
            Action<ILoggingBuilder> configureLogging = null,
            Action<HostBuilderContext, IServiceCollection> configureServicesDelegate = null)
        {
            return CreateHostBuilderDefaults(buildType, null, appConfigurationConfigureDelegate, webHostConfigure,
                configureLogging, configureServicesDelegate);
        }


        public static IHostBuilder CreateHostBuilderDefaults(
            string buildType,
            string[] args,
            Action<HostBuilderContext, IConfigurationBuilder> appConfigurationConfigureDelegate,
            Action<IWebHostBuilder> webHostConfigure,
            Action<ILoggingBuilder> configureLogging = null,
            Action<HostBuilderContext, IServiceCollection> configureServicesDelegate = null)
        {
            var defaultBuilder = Host.CreateDefaultBuilder(args);
            IHostBuilder hostBuilderDefaults;
            if (defaultBuilder == null)
            {
                hostBuilderDefaults = null;
            }
            else
            {
                var hostBuilder1 = defaultBuilder.ConfigureAppConfigurationDefaults((hostingContext, configBuilder) =>
                {
                    if (appConfigurationConfigureDelegate == null)
                    {
                        return;
                    }
                    appConfigurationConfigureDelegate(hostingContext, configBuilder);
                });
                if (hostBuilder1 == null)
                {
                    hostBuilderDefaults = null;
                }
                else
                {
                    var hostBuilder2 = hostBuilder1.ConfigureLoggingDefaults(configureLogging);
                    if (hostBuilder2 == null)
                    {
                        hostBuilderDefaults = null;
                    }
                    else
                    {
                        var builder = hostBuilder2.ConfigureServiceDefaults(buildType, configureServicesDelegate);
                        hostBuilderDefaults = builder != null ? builder.ConfigureWebHostDefaults(webBuilder =>
                            {
                                var action = webHostConfigure;
                                if (action == null)
                                {
                                    return;
                                }
                                action(webBuilder);
                            })
                            : null;
                    }
                }
            }

            return hostBuilderDefaults;
        }


        /// <summary>GenericHostBuilder</summary>
        /// <param name="buildType"> Release|Debug </param>
        /// <param name="args"></param>
        /// <param name="appConfigurationConfigureDelegate">
        ///     used in
        ///     <see
        ///         cref="M:Microsoft.Extensions.Hosting.GenericHostBuilderExtensions.ConfigureWebHostDefaults(Microsoft.Extensions.Hosting.IHostBuilder,System.Action{Microsoft.AspNetCore.Hosting.IWebHostBuilder})" />
        ///     .
        /// </param>
        /// <param name="webHostConfigure">
        ///     used in
        ///     <see
        ///         cref="M:Microsoft.Extensions.Hosting.GenericHostBuilderExtensions.ConfigureWebHostDefaults(Microsoft.Extensions.Hosting.IHostBuilder,System.Action{Microsoft.AspNetCore.Hosting.IWebHostBuilder})" />
        /// </param>
        /// <param name="configureLogging">
        ///     used in
        ///     <see
        ///         cref="M:Microsoft.Extensions.Hosting.HostingHostBuilderExtensions.ConfigureLogging(Microsoft.Extensions.Hosting.IHostBuilder,System.Action{Microsoft.Extensions.Logging.ILoggingBuilder})" />
        /// </param>
        /// <param name="configureServicesDelegate">
        ///     used in
        ///     <see
        ///         cref="M:Microsoft.Extensions.Hosting.IHostBuilder.ConfigureServices(System.Action{Microsoft.Extensions.Hosting.HostBuilderContext,Microsoft.Extensions.DependencyInjection.IServiceCollection})" />
        /// </param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilderDefaults(
            string buildType,
            string[]? args,
            Action<IConfigurationBuilder> appConfigurationConfigureDelegate,
            Action<IWebHostBuilder> webHostConfigure,
            Action<ILoggingBuilder> configureLogging = null,
            Action<HostBuilderContext, IServiceCollection> configureServicesDelegate = null)
        {
            var defaultBuilder = Host.CreateDefaultBuilder(args);
            IHostBuilder hostBuilderDefaults;
            if (defaultBuilder == null)
            {
                hostBuilderDefaults = null;
            }
            else
            {
                var hostBuilder1 = defaultBuilder.ConfigureAppConfigurationDefaults(configBuilder =>
                {
                    var action = appConfigurationConfigureDelegate;
                    if (action == null)
                    {
                        return;
                    }
                    action(configBuilder);
                });
                if (hostBuilder1 == null)
                {
                    hostBuilderDefaults = null;
                }
                else
                {
                    var hostBuilder2 = hostBuilder1.ConfigureLoggingDefaults(configureLogging);
                    if (hostBuilder2 == null)
                    {
                        hostBuilderDefaults = null;
                    }
                    else
                    {
                        var builder = hostBuilder2.ConfigureServiceDefaults(buildType, configureServicesDelegate);
                        hostBuilderDefaults = builder != null
                            ? builder.ConfigureWebHostDefaults(webBuilder =>
                            {
                                var action = webHostConfigure;
                                if (action == null)
                                {
                                    return;
                                }
                                action(webBuilder);
                            })
                            : null;
                    }
                }
            }

            return hostBuilderDefaults;
        }
    }
}
