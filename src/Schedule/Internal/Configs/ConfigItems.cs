using System;
using System.Linq;
using WindNight.Core.Abstractions;
using WindNight.Core.ConfigCenter.Extensions;
using WindNight.Core.Enums.Abstractions;

namespace Schedule.@internal
{
    internal class ConfigItems : DefaultConfigItemBase
    {
        public static LogLevels JobMiniLogLevel
        {
            get
            {
                try
                {
                    var miniLogLevel = Convert2LogLevel(JobsConfig?.MiniLogLevel);
                    if (miniLogLevel > GlobalMiniLogLevel)
                    {
                        miniLogLevel = GlobalMiniLogLevel;
                    }

                    return miniLogLevel;
                }
                catch (Exception ex)
                {
                    return LogLevels.Information;

                }
            }
        }



        internal static JobsConfig JobsConfig
        {
            get
            {
                if (_jobsConfig != null && _jobsConfig.Items.Any())
                {
                    return _jobsConfig;
                }

                var jobConfigs = GetSectionValue<JobsConfig>(ConfigItemsKey.ScheduleJobNodeName);
                return jobConfigs;
                //if (configuration != null)
                //{
                //    var jobConfigs = configuration.GetSection(ConfigItemsKey.ScheduleJobNodeName).Get<JobsConfig>();
                //    if (jobConfigs != null && jobConfigs.Items.Any()) return jobConfigs;
                //}

                //return GetFileConfig<JobsConfig>(ConfigItemsKey.ScheduleJobsFileName);
            }
            private set => _jobsConfig = value;
        }

        public static string DingtalkToken =>
            GetAppSettingValue(ConfigItemsKey.DingtalkTokenKey, "", false);
        public static string NoticeDingSignKey =>
                GetAppSettingValue(ConfigItemsKey.DingtalkSignKey, "", false);

        public static string DingtalkPhones =>
            GetAppSettingValue(ConfigItemsKey.DingtalkPhonesKey, "", false);

        public static bool DingtalkAtAll =>
            GetAppSettingValue(ConfigItemsKey.DingtalkAtAllKey, false, false);


        private static JobsConfig _jobsConfig;

        public static void SetJobsConfig(JobsConfig jobsConfig)
        {
            _jobsConfig = jobsConfig;
        }

        //private static string GetAppSettingConfig(string configKey, string defaultValue = "", bool isThrow = true)
        //{
        //    var configService = Ioc.Instance.CurrentConfigService;
        //    if (configService == null)
        //    {
        //        if (!defaultValue.IsNullOrEmpty())
        //            return defaultValue;
        //        if (isThrow)
        //            throw new ApplicationException("Please Impl the interface of IConfigService and  register it.");
        //        return defaultValue;
        //    }

        //    return configService.GetAppSettingValue(configKey, defaultValue, isThrow);
        //}


        //private static bool GetAppSettingConfig(string configKey, bool defaultValue = false, bool isThrow = true)
        //{

        //    var configValue = GetAppSettingConfig(configKey, "", isThrow);
        //    if (configValue.IsNullOrEmpty())
        //    {
        //        return defaultValue;
        //    }

        //    if (TrueStrings.Contains(configValue, StringComparer.OrdinalIgnoreCase))
        //    {
        //        return true;
        //    }

        //    if (FalseStrings.Contains(configValue, StringComparer.OrdinalIgnoreCase))
        //    {
        //        return false;
        //    }

        //    if (isThrow)
        //    {
        //        throw new ArgumentOutOfRangeException("configKey",
        //            $"configKey({configKey}) is not in TrueStrings({TrueStrings.Join()}) or FalseStrings({FalseStrings.Join()}) both IgnoreCase ");
        //    }
        //    return defaultValue;


        //}

        //private static T GetFileConfig<T>(string configKey, bool isThrow = true) where T : class, new()
        //{
        //    var configService = Ioc.Instance.CurrentConfigService;

        //    if (configService == null)
        //        throw new ApplicationException("Please Impl the interface of IConfigService and  register it.");

        //    return configService.GetFileConfig<T>(configKey);
        //}


        internal static class ConfigItemsKey
        {
            internal static string ScheduleJobsFileName = "schedulejobs.json";

            internal static string ScheduleJobNodeName = "ScheduleJobs";
            internal static string JobRemotingConfigNodeName = "ScheduleJobs:Remoting";

            internal static string DingtalkTokenKey = "JobExecuted:NoticeDingToken";
            internal static string DingtalkSignKey = "JobExecuted:NoticeDingSignKey";
            internal static string DingtalkPhonesKey = "JobExecuted:NoticeDingPhones";
            internal static string DingtalkAtAllKey = "JobExecuted:NoticeDingAtAll";
        }
    }
}
