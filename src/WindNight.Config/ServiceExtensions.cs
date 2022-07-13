#if NET45LATER

using Microsoft.Extensions.DependencyInjection;
using WindNight.ConfigCenter.Extension;
using WindNight.Core.Abstractions;

namespace WindNight.RabbitMq
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDefaultConfigService(this IServiceCollection services)
        {

            services.AddSingleton<IConfigService, DefaultConfigService>();

            return services;
        }



    }
}
#endif

