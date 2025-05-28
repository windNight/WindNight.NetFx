using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WindNight.Core.ConfigCenter.Extensions;

namespace WindNight.Config.Extensions
{
    public static class ServicesExtension
    {
        public static IServiceCollection AddConfigExtension(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetAppSettingValue("OpenConfigCenter", false, false))
            {
                services.AddTransient(typeof(ConfigController));
            }

            return services;
        }

    }
}
