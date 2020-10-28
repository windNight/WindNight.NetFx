using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WnExtensions;
using Microsoft.Extensions.Logging;
using WindNight.Core.Abstractions;

namespace Microsoft.AspNetCore.Hosting.WnExtensions
{
    public static class ProgramBase
    {
        public static async Task InitAsync(Func<string[], IHostBuilder> createHostBuilder, string buildType, string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionEventHandler;
            TaskScheduler.UnobservedTaskException += UnobservedTaskHandler;
            var hostBuilder = createHostBuilder.Invoke(args);
            var host = hostBuilder.Build();
            await host.InjectionRSAsync(buildType);
            await host.RunAsync();
        }

        public static void Init(Func<string[], IHostBuilder> createHostBuilder, string buildType, string[] args)
        {
            InitAsync(createHostBuilder, buildType, args).GetAwaiter().GetResult();
        }

        /// <summary>
        ///     GenericHostBuilder
        /// </summary>
        /// <param name="appConfigurationConfigureDelegate">
        ///     used in
        ///     <see
        ///         cref="Microsoft.Extensions.Hosting.GenericHostBuilderExtensions.ConfigureWebHostDefaults(IHostBuilder, Action{IWebHostBuilder})" />
        ///     .
        /// </param>
        /// <param name="webHostConfigure">
        ///     used in
        ///     <see
        ///         cref=" Microsoft.Extensions.Hosting.GenericHostBuilderExtensions.ConfigureWebHostDefaults(IHostBuilder, Action{IWebHostBuilder})" />
        /// </param>
        /// <param name="configureLogging">
        ///     used in
        ///     <see
        ///         cref=" Microsoft.Extensions.Hosting.HostingHostBuilderExtensions.ConfigureLogging(IHostBuilder, Action{ILoggingBuilder})" />
        /// </param>
        /// <param name="configureServicesDelegate">
        ///     used in
        ///     <see
        ///         cref=" Microsoft.Extensions.Hosting.IHostBuilder.ConfigureServices(Action{HostBuilderContext, IServiceCollection})" />
        /// </param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilderDefaults(
            Action<IConfigurationBuilder> appConfigurationConfigureDelegate,
            Action<IWebHostBuilder> webHostConfigure,
            Action<ILoggingBuilder> configureLogging = null,
            Action<HostBuilderContext, IServiceCollection> configureServicesDelegate = null)
        {
            return CreateHostBuilderDefaults(null, appConfigurationConfigureDelegate, webHostConfigure,
                configureLogging, configureServicesDelegate);
        }

        /// <summary>
        ///     GenericHostBuilder
        /// </summary>
        /// <param name="args"></param>
        /// <param name="appConfigurationConfigureDelegate">
        ///     used in
        ///     <see
        ///         cref="Microsoft.Extensions.Hosting.GenericHostBuilderExtensions.ConfigureWebHostDefaults(IHostBuilder, Action{IWebHostBuilder})" />
        ///     .
        /// </param>
        /// <param name="webHostConfigure">
        ///     used in
        ///     <see
        ///         cref=" Microsoft.Extensions.Hosting.GenericHostBuilderExtensions.ConfigureWebHostDefaults(IHostBuilder, Action{IWebHostBuilder})" />
        /// </param>
        /// <param name="configureLogging">
        ///     used in
        ///     <see
        ///         cref=" Microsoft.Extensions.Hosting.HostingHostBuilderExtensions.ConfigureLogging(IHostBuilder, Action{ILoggingBuilder})" />
        /// </param>
        /// <param name="configureServicesDelegate">
        ///     used in
        ///     <see
        ///         cref=" Microsoft.Extensions.Hosting.IHostBuilder.ConfigureServices(Action{HostBuilderContext, IServiceCollection})" />
        /// </param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilderDefaults(string[] args,
            Action<IConfigurationBuilder> appConfigurationConfigureDelegate,
            Action<IWebHostBuilder> webHostConfigure,
            Action<ILoggingBuilder> configureLogging = null,
            Action<HostBuilderContext, IServiceCollection> configureServicesDelegate = null)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfigurationDefaults(configBuilder =>
                {
                    appConfigurationConfigureDelegate?.Invoke(configBuilder);
                })
                .ConfigureLoggingDefaults(configureLogging)
                .ConfigureServiceDefaults(configureServicesDelegate) 
                .ConfigureWebHostDefaults(webBuilder => { webHostConfigure?.Invoke(webBuilder); });
        }

        public static void UnobservedTaskHandler(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Ioc.GetService<ILogService>()?.Fatal("UnobservedTaskException", e.Exception);
        }

        public static void UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Ioc.GetService<ILogService>()?.Fatal("UnhandledException", e.ExceptionObject as Exception);
        }

        /// <summary>
        ///     Sets up the configuration for the remainder of the build process and application. This can be called multiple times
        ///     and
        ///     the results will be additive. The results will be available at
        ///     <see cref="P:Microsoft.Extensions.Hosting.HostBuilderContext.Configuration" /> for
        ///     subsequent operations, as well as in <see cref="P:Microsoft.Extensions.Hosting.IHost.Services" />.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="T:Microsoft.Extensions.Hosting.IHostBuilder" /> to configure.</param>
        /// <param name="configureDelegate">
        ///     The delegate for configuring the <see cref="T:Microsoft.Extensions.Configuration.IConfigurationBuilder" /> that
        ///     will be used
        ///     to construct the <see cref="T:Microsoft.Extensions.Configuration.IConfiguration" /> for the host.
        /// </param>
        /// <returns>The same instance of the <see cref="T:Microsoft.Extensions.Hosting.IHostBuilder" /> for chaining.</returns>
        public static IHostBuilder ConfigureAppConfigurationDefaults(
            this IHostBuilder hostBuilder,
            Action<IConfigurationBuilder> configureDelegate)
        {
            return hostBuilder?.ConfigureAppConfiguration(configBuilder =>
            {
                configureDelegate?.Invoke(configBuilder);
            });
        }


        /// <summary>
        ///     Adds a delegate for configuring the provided <see cref="T:Microsoft.Extensions.Logging.ILoggingBuilder" />. This
        ///     may be called multiple times.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="T:Microsoft.Extensions.Hosting.IHostBuilder" /> to configure.</param>
        /// <param name="configureLogging">
        ///     The delegate that configures the
        ///     <see cref="T:Microsoft.Extensions.Logging.ILoggingBuilder" />.
        /// </param>
        /// <returns>The same instance of the <see cref="T:Microsoft.Extensions.Hosting.IHostBuilder" /> for chaining.</returns>
        public static IHostBuilder ConfigureLoggingDefaults(this IHostBuilder hostBuilder,
            Action<ILoggingBuilder> configureLogging = null)
        {
            return hostBuilder?.ConfigureLogging(loggerBuilder =>
            {
                loggerBuilder.AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Information)
                    .AddFilter("System", LogLevel.Warning)
                    .AddConsole()
                    .SetMinimumLevel(LogLevel.Debug);
                configureLogging?.Invoke(loggerBuilder);
            });
        }

        /// <summary>
        ///     Adds default services to the container.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="T:Microsoft.Extensions.Hosting.IHostBuilder" /> to configure.</param>
        /// <param name="configureDelegate">
        ///     The delegate for configuring the <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> that
        ///     will be used
        ///     to construct the <see cref="T:System.IServiceProvider" />.
        /// </param>
        /// <returns>The same instance of the <see cref="T:Microsoft.Extensions.Hosting.IHostBuilder" /> for chaining.</returns>
        public static IHostBuilder ConfigureServiceDefaults(this IHostBuilder hostBuilder,
            Action<HostBuilderContext, IServiceCollection> configureDelegate = null)
        {
            return hostBuilder?.ConfigureServices((context, services) =>
            {
                services.AddOptions();
                services.AddHttpContextAccessor();
                var configuration = context.Configuration;
                services.TryAddSingleton(configuration);
                configureDelegate?.Invoke(context, services);
            });
        }
    }
}