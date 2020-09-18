using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Core.Abstractions;

namespace Schedule
{
    internal class ConfigItems
    {
        internal static JobsConfig JobsConfig
        {
            get
            {
                if (_jobsConfig != null && _jobsConfig.Items.Any()) return _jobsConfig;
                var configuration = Ioc.GetService<IConfiguration>();
                if (configuration != null)
                {
                    var jobConfigs = configuration.GetSection(ConfigItemsKey.ScheduleJobNodeName).Get<JobsConfig>();
                    if (jobConfigs != null && jobConfigs.Items.Any()) return jobConfigs;
                }
                return GetFileConfig<JobsConfig>(ConfigItemsKey.ScheduleJobsFileName);
            }
            private set => _jobsConfig = value;
        }


        private static JobsConfig _jobsConfig;

        public static void SetJobsConfig(JobsConfig jobsConfig)
        {
            _jobsConfig = jobsConfig;
        }

        //internal static string DingtalkToken => GetAppSettingConfig(ConfigItemsKey.DingtalkTokenKey,
        //    "63c799ab6890b7c293ab32e94b862be21a961e4edb245f6187606e93cc39bcd1", false);

        //internal static string DingtalkPhones => GetAppSettingConfig(ConfigItemsKey.DingtalkPhonesKey, "", false);

        //internal static bool DingtalkAtAll => GetAppSettingConfig(ConfigItemsKey.DingtalkAtAllKey, "0", false) == "1";


        private static string GetAppSettingConfig(string configKey, string defaultValue = "", bool isThrow = true)
        {
            var configService = Ioc.GetService<IConfigService>();
            if (configService == null)
            {
                if (!string.IsNullOrEmpty(defaultValue))
                    return defaultValue;
                if (isThrow)
                    throw new ApplicationException("Please Impl the interface of IConfigService and  register it.");
                return defaultValue;
            }

            return configService.GetAppSetting(configKey, defaultValue, isThrow);
        }

        private static T GetFileConfig<T>(string configKey, bool isThrow = true) where T : class, new()
        {
            var configService = Ioc.GetService<IConfigService>();

            if (configService == null)
                throw new ApplicationException("Please Impl the interface of IConfigService and  register it.");

            return configService.GetFileConfig<T>(configKey);
        }

        internal static class ConfigItemsKey
        {
            internal static string ScheduleJobsFileName = "schedulejobs.json";

            internal static string ScheduleJobNodeName = "ScheduleJobs";

            internal static string DingtalkTokenKey = "JobExecuted:NoticeDingToken";
            internal static string DingtalkPhonesKey = "JobExecuted:NoticeDingPhones";
            internal static string DingtalkAtAllKey = "JobExecuted:NoticeDingAtAll";
        }
    }
}