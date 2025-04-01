using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Core.Abstractions;
using WindNight.LogExtension;

namespace WindNight.Extension
{
    public static class Extensions
    {
        /// <summary>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="Configuration"></param>
        /// <param name="log4netfile"></param>
        /// <param name="configure"></param>
        /// <param name="loggerProcessor"></param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultLogService(this IServiceCollection services, IConfiguration configuration)
        {
            Ioc.Instance.InitServiceProvider(services);
            LogHelper.Debug("WindNight.LogExtension.LogHelper init");
            services.AddTransient<ILogService, DefaultLogService>();
            return services;
        }
    }
}
