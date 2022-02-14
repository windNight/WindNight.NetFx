using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using System;
using System.Linq;

namespace Schedule
{
    internal class ConfigItems
    {
        private static IConfiguration configuration => Ioc.GetService<IConfiguration>();


        internal static JobsConfig JobsConfig
        {
            get
            {
                if (_jobsConfig != null && _jobsConfig.Items.Any()) return _jobsConfig;
                if (configuration != null)
                {
                    var jobConfigs = configuration.GetSection(ConfigItemsKey.ScheduleJobNodeName).Get<JobsConfig>();
                    if (jobConfigs != null && jobConfigs.Items.Any()) return jobConfigs;
                }

                return GetFileConfig<JobsConfig>(ConfigItemsKey.ScheduleJobsFileName);
            }
            private set => _jobsConfig = value;
        }

        public static string DingtalkToken =>
            GetAppSettingConfig(ConfigItemsKey.DingtalkTokenKey, "", false);

        public static string DingtalkPhones =>
            GetAppSettingConfig(ConfigItemsKey.DingtalkPhonesKey, "", false);

        public static bool DingtalkAtAll =>
            GetAppSettingConfig(ConfigItemsKey.DingtalkAtAllKey, "false", false) == "true";

        private static JobsConfig _jobsConfig;

        public static void SetJobsConfig(JobsConfig jobsConfig)
        {
            _jobsConfig = jobsConfig;
        }

        private static string GetAppSettingConfig(string configKey, string defaultValue = "", bool isThrow = true)
        {
            var configService = Ioc.Instance.CurrentConfigService;
            if (configService == null)
            {
                if (!defaultValue.IsNullOrEmpty())
                    return defaultValue;
                if (isThrow)
                    throw new ApplicationException("Please Impl the interface of IConfigService and  register it.");
                return defaultValue;
            }

            return configService.GetAppSetting(configKey, defaultValue, isThrow);
        }

        private static T GetFileConfig<T>(string configKey, bool isThrow = true) where T : class, new()
        {
            var configService = Ioc.Instance.CurrentConfigService;

            if (configService == null)
                throw new ApplicationException("Please Impl the interface of IConfigService and  register it.");

            return configService.GetFileConfig<T>(configKey);
        }


        internal static class ConfigItemsKey
        {
            internal static string ScheduleJobsFileName = "schedulejobs.json";

            internal static string ScheduleJobNodeName = "ScheduleJobs";
            internal static string JobRemotingConfigNodeName = "ScheduleJobs:Remoting";

            internal static string DingtalkTokenKey = "JobExecuted:NoticeDingToken";
            internal static string DingtalkPhonesKey = "JobExecuted:NoticeDingPhones";
            internal static string DingtalkAtAllKey = "JobExecuted:NoticeDingAtAll";
        }
    }
}