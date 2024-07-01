using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.Extensions;
using System.Threading;

using Newtonsoft.Json.Extension;
using WindNight.Config.@internal;
using WindNight.ConfigCenter.Extension.@internal;
using WindNight.Core;
using WindNight.Linq.Extensions.Expressions;

#if NET45
using System.Configuration;

#else

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Extensions;
using Newtonsoft.Json.Linq;
using System.Collections;
using Microsoft.Extensions.DependencyInjection.WnExtension;

#endif

namespace WindNight.ConfigCenter.Extension
{
    /// <summary>
    ///     配置提供者
    /// </summary>
    internal partial class ConfigProvider
    {
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
                    if (!configName.IsNullOrEmpty()) return LoadConfigByFileName(configName);
                    return LoadAllConfig();
                }
                catch (Exception ex)
                {
                    LogHelper.Warn($"LoadJsonConfig({configName}) handler error ,{ex.Message}", ex);
                    return new Tuple<int, string, string>(1, $"LoadJsonConfig Failed {ex.Message}", "");
                }
            }
        }


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