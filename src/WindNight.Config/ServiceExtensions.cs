#if NET45LATER
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WindNight.Core.Abstractions;

namespace WindNight.ConfigCenter.Extension
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddConfig<T>(this IServiceCollection services,
            IConfiguration configuration)
            where T : class, new()
        {
            var section = configuration.GetSection(typeof(T).Name);
            services.Configure<T>(section);
            services.AddSingleton<T>();
            return services;

        }


        public static IServiceCollection AddDefaultConfigService(this IServiceCollection services)
        {

            services.AddSingleton<IConfigService, DefaultConfigService>();
            return services;
        }



    }
}
#endif

