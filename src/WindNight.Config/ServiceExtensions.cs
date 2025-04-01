#if NET45LATER
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Config.@internal;
using WindNight.Core.Abstractions;

namespace WindNight.ConfigCenter.Extension
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddConfig<T>(this IServiceCollection services, IConfiguration configuration)
            where T : class, new()
        {
            var section = configuration.GetSection(typeof(T).Name);
            services.Configure<T>(section);
            services.AddSingleton<T>();
            return services;

        }


        public static IServiceCollection AddDefaultConfigService(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddSingleton<IConfigService, DefaultConfigService>();
            Ioc.Instance.InitServiceProvider(services);
            return services;
        }



    }
}
#endif

