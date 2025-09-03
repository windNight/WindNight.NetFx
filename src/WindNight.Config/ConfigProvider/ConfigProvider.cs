#if NET45
using System.Configuration;
#else
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.WnExtension;
#endif
using System.Collections.Concurrent;
using WindNight.ConfigCenter.Extension.@internal;

namespace WindNight.ConfigCenter.Extension
{
    /// <summary>
    ///     配置提供者
    /// </summary>
    internal partial class ConfigProvider
    {
        private static readonly object LockJson = new object();
        //private static readonly object LockDW = new object();


        private static ConcurrentDictionary<string, DateTime> configUpdateTime { get; set; } =
            new ConcurrentDictionary<string, DateTime>();

        private static ConcurrentDictionary<string, string> _updateFlagDict { get; } =
            new ConcurrentDictionary<string, string>();

        public static ConfigProvider Instance { get; }

        private static string ConfigPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");


        public Dictionary<string, DateTime> ConfigUpdateTime => configUpdateTime.ToDictionary(m => m.Key, p => p.Value);

        public Dictionary<string, string> UpdateFlagDict => _updateFlagDict.ToDictionary(m => m.Key, p => p.Value);

        /// <summary>
        /// </summary>
        /// <param name="sleepTimeInMs"> default is 5000 ms </param>
        public void SetHeartRunSleepTime(int sleepTimeInMs)
        {
            SleepTime = sleepTimeInMs;
        }

        /// <summary>
        /// </summary>
        /// <param name="sleepTimeInMs"> default is 5000 ms </param>
        public void Start(int sleepTimeInMs = 5 * 1000)
        {
            _isStop = false;
            SleepTime = sleepTimeInMs;
            LoadAllConfigs();
        }

        public void Stop()
        {
            _isStop = true;
            configUpdateTime.Clear();
            _updateFlagDict.Clear();
        }

        public void ReInit()
        {
            configUpdateTime = new ConcurrentDictionary<string, DateTime>();
            _isStop = false;
            HeartRun();
        }


        /// <summary>
        ///     加载配置
        /// </summary>
        public Tuple<int, string, string> LoadConfigFile(string configName = null)
        {
            lock (LockJson)
            {
                try
                {
                    if (configName.IsNotNullOrEmpty())
                    {
                        return LoadConfigByFileName(configName);
                    }

                    return LoadAllConfig();
                }
                catch (Exception ex)
                {
                    LogHelper.Warn($"LoadJsonConfig({configName}) handler error ,{ex.Message}", ex);
                    return new Tuple<int, string, string>(1, $"LoadJsonConfig Failed {ex.Message}", "");
                }
            }
        }
#if !NET45
        private static IConfiguration? _configuration;

        //public void SetConfiguration()
        //{
        //    _configuration = Ioc.GetService<IConfiguration>();
        //}

        public void SetConfiguration(IConfiguration configuration = null)
        {
            _configuration = configuration ?? Ioc.GetService<IConfiguration>();
        }

#endif


        ///// <summary>
        /////     加载配置
        ///// </summary>
        //public Tuple<int, string, string> LoadDomainSwitch(string nodeName = "",
        //    DomainSwitchNodeType nodeType = DomainSwitchNodeType.Unknown)
        //{
        //    lock (LockDW)
        //    {
        //        try
        //        {
        //            var configValue = string.Empty;
        //            if (!nodeName.IsNullOrEmpty() && nodeType != DomainSwitchNodeType.Unknown)
        //            {
        //                if (nodeType == DomainSwitchNodeType.ServiceUrl)
        //                    configValue = DomainSwitch.GetServiceUrl(nodeName);
        //                if (nodeType == DomainSwitchNodeType.SiteHost) configValue = DomainSwitch.GetSiteHost(nodeName);

        //                var nodePath = $"{nodeType}:{nodeName}";
        //                if (!configValue.IsNullOrEmpty())
        //                    ConfigCenterContext.SetDomainSwitch(nodePath, configValue);
        //                return new Tuple<int, string, string>(0, "刷新成功", configValue);
        //            }

        //            //  LoadAllDomainSwitch();
        //            return new Tuple<int, string, string>(0, "刷新成功", "");
        //        }
        //        catch (Exception ex)
        //        {
        //            LogHelper.Warn($"LoadDomainSwitch({nodeName},{nodeType}) handler error ,{ex.Message}", ex);
        //            return new Tuple<int, string, string>(1, $"LoadDomainSwitch Failed {ex.Message}", "");
        //        }
        //    }
        //}
    }
}
