
#if !NET45
using Microsoft.Extensions.DependencyInjection.WnExtension;
using System.Collections.Generic;
using WindNight.Core;

namespace Microsoft.Extensions.Configuration.Extensions
{
    /// <summary>
    /// </summary>
    public static class ConfigurationExtension
    {
        /// <summary>
        /// </summary>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static List<ConfigBaseInfo> GetConfiguration(IEnumerable<IConfigurationSection>? sections = null)
        {
            var _config = Ioc.GetService<IConfiguration>();
            if (sections == null) sections = _config.GetChildren();

            var list = new List<ConfigBaseInfo>();
            foreach (var m in sections)
            {
                if (m.Value != null)
                {
                    list.Add(new ConfigBaseInfo { Key = m.Key, Path = m.Path, Value = m.Value });
                    continue;
                }

                var _section = _config.GetSection(m.Path);
                list.Add(new ConfigBaseInfo { Key = m.Key, Path = m.Path, Value = GetConfiguration(_section.GetChildren()) });
            }

            return list;
        }

        /// <summary>
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static List<ConfigBaseInfo> GetConfiguration(this IConfiguration configuration,
            IEnumerable<IConfigurationSection>? sections = null)
        {
            if (configuration == null) return new List<ConfigBaseInfo>();
            var _config = configuration;
            if (sections == null) sections = _config.GetChildren();

            var list = new List<ConfigBaseInfo>();
            foreach (var m in sections)
            {
                if (m.Value != null)
                {
                    list.Add(new ConfigBaseInfo { Key = m.Key, Path = m.Path, Value = m.Value });
                    continue;
                }

                var _section = _config.GetSection(m.Path);
                list.Add(new ConfigBaseInfo { Key = m.Key, Path = m.Path, Value = _config.GetConfiguration(_section.GetChildren()) });
            }

            return list;
        }
    }
}
#endif
