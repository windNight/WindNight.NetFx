#if !NET45
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Core;

namespace Microsoft.Extensions.Configuration.Extensions
{
    /// <summary>
    /// </summary>
    public static class ConfigurationExtension
    {
        public static IServiceCollection ConfigureOption<TOptions>(this IServiceCollection services, IConfiguration configuration)
            where TOptions : class, new()
        {
            var sectionKey = typeof(TOptions).Name;
            services.ConfigureOption<TOptions>(configuration, sectionKey);
            return services;
        }

        public static IServiceCollection ConfigureOption<TOptions>(this IServiceCollection services, IConfiguration configuration, string sectionKey)
            where TOptions : class, new()
        {
            var config = configuration.GetSection(sectionKey);
            services.Configure<TOptions>(config);
            services.AddSingleton<TOptions>();
            return services;
        }

        /// <summary>
        /// </summary>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static List<ConfigBaseInfo2> GetConfiguration(IEnumerable<IConfigurationSection>? sections = null)
        {
            var _config = Ioc.GetService<IConfiguration>();
            if (sections == null) sections = _config.GetChildren();

            var list = new List<ConfigBaseInfo2>();
            foreach (var m in sections)
            {
                if (m.Value != null)
                {
                    list.Add(new ConfigBaseInfo2 { Key = m.Key, Path = m.Path, Value = m.Value });
                    continue;
                }

                var _section = _config.GetSection(m.Path);
                list.Add(new ConfigBaseInfo2
                {
                    Key = m.Key,
                    Path = m.Path,
                    Value = GetConfiguration(_section.GetChildren()),
                });
            }

            return list;
        }

        /// <summary>
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static List<ConfigBaseInfo2> GetConfiguration(this IConfiguration configuration,
            IEnumerable<IConfigurationSection>? sections = null)
        {
            if (configuration == null) return new List<ConfigBaseInfo2>();
            var _config = configuration;
            if (sections == null) sections = _config.GetChildren();

            var list = new List<ConfigBaseInfo2>();
            foreach (var m in sections)
            {
                if (m.Value != null)
                {
                    list.Add(new ConfigBaseInfo2 { Key = m.Key, Path = m.Path, Value = m.Value });
                    continue;
                }

                var _section = _config.GetSection(m.Path);
                list.Add(new ConfigBaseInfo2
                {
                    Key = m.Key,
                    Path = m.Path,
                    Value = _config.GetConfiguration(_section.GetChildren()),
                });
            }

            return list;
        }
    }
}
#endif
