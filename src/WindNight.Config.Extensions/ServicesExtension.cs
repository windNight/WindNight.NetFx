using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WindNight.Config.Extensions
{
    public static class ServicesExtension
    {
        public static IServiceCollection AddConfigExtension(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddTransient(typeof(ConfigController));

            return services;
        }

    }
}
