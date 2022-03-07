using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Extensions;
using System.Threading;

using Newtonsoft.Json.Extension;
using WindNight.ConfigCenter.Extension.Internal;

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
    internal class ConfigProvider
    {
#if !NET45
        private static IConfiguration? _configuration;

        public void SetConfiguration()
        {
            _configuration = Ioc.GetService<IConfiguration>();
        }

        public void SetConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

#endif

        private static Thread _thread;
        private static readonly object LockJson = new object();
        private static readonly object LockDW = new object();
        private static readonly object Lockinstance = new object();
        private bool _isStop;

        static ConfigProvider()
        {
            if (Instance == null)
                lock (Lockinstance)
                {
                    if (Instance == null)
                        Instance = new ConfigProvider();
                }
        }

        private ConfigProvider()
        {
            _isStop = false;
            RegisterHeartRun();
        }

        private static ConcurrentDictionary<string, DateTime> configUpdateTime { get; set; } =
            new ConcurrentDictionary<string, DateTime>();

        private static ConcurrentDictionary<string, string> _updateFlagDict { get; } =
            new ConcurrentDictionary<string, string>();

        public static ConfigProvider Instance { get; }

        private static string ConfigPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");

        private static List<string> ValidExtensions => new List<string> { ".json" };

        private int SleepTime { get; set; } = 5 * 1000;

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


        ~ConfigProvider()
        {
            Stop();
        }

        /// <summary>
        ///     加载配置
        /// </summary>
        public Tuple<int, string, string> LoadJsonConfig(string configName = null)
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

        /// <summary>
        ///     加载配置
        /// </summary>
        public Tuple<int, string, string> LoadDomainSwitch(string nodeName = "",
            DomainSwitchNodeType nodeType = DomainSwitchNodeType.Unknown)
        {
            lock (LockDW)
            {
                try
                {
                    var configValue = string.Empty;
                    if (!nodeName.IsNullOrEmpty() && nodeType != DomainSwitchNodeType.Unknown)
                    {
                        if (nodeType == DomainSwitchNodeType.ServiceUrl)
                            configValue = DomainSwitch.GetServiceUrl(nodeName);
                        if (nodeType == DomainSwitchNodeType.SiteHost) configValue = DomainSwitch.GetSiteHost(nodeName);

                        var nodePath = $"{nodeType}:{nodeName}";
                        if (!configValue.IsNullOrEmpty())
                            ConfigCenterContext.SetDomainSwitch(nodePath, configValue);
                        return new Tuple<int, string, string>(0, "刷新成功", configValue);
                    }

                    LoadAllDomainSwitch();
                    return new Tuple<int, string, string>(0, "刷新成功", "");
                }
                catch (Exception ex)
                {
                    LogHelper.Warn($"LoadDomainSwitch({nodeName},{nodeType}) handler error ,{ex.Message}", ex);
                    return new Tuple<int, string, string>(1, $"LoadDomainSwitch Failed {ex.Message}", "");
                }
            }
        }

        #region Private

        /// <summary>
        ///     注册心跳刷新
        /// </summary>
        private void RegisterHeartRun()
        {
            try
            {
                _thread = new Thread(HeartRun);
                _thread.Start();
            }
            catch (Exception ex)
            {
                LogHelper.Warn($"RegisterHeartRun handler error ,{ex.Message}", ex);
            }
        }

        /// <summary>
        ///     心跳刷新启动
        /// </summary>
        private void HeartRun()
        {
            while (true)
            {
                if (_isStop)
                    break;
                Thread.Sleep(SleepTime);
                try
                {
                    LoadAllConfigs();
                }
                catch (Exception ex)
                {
                    LogHelper.Warn($"LoadAllConfigs handler error ,{ex.Message}", ex);
                }
            }
        }

        private void LoadAllConfigs()
        {
            LoadJsonConfig();
#if !NET45
            if (_configuration == null)
            {
                Thread.Sleep(5 * 1000);
                return;
            }
#endif
            LoadAllDomainSwitch();
            LoadAllAppSettings();
            LoadAllConnectionStrings();
        }

        private Tuple<int, string, string> LoadConfigByFileName(string configName)
        {
            var filePath = Path.Combine(ConfigPath, configName);
            if (!CheckFileExtension(filePath)) return new Tuple<int, string, string>(0, "刷新成功", "");
            var content = string.Empty;
            if (File.Exists(filePath))
            {
                content = File.ReadAllText(filePath);
                if (!content.IsNullOrEmpty())
                {
                    ConfigCenterContext.SetJsonConfig(configName, content);
                    configUpdateTime[configName] = Directory.GetLastWriteTime(filePath);
                }
            }

            return new Tuple<int, string, string>(0, "刷新成功", content);
        }


        private bool CheckFileExtension(string filePath)
        {
            return ValidExtensions.Contains(Path.GetExtension(filePath));
        }

        private Tuple<int, string, string> LoadAllConfig()
        {
            Dictionary<string, string>? dict = null;
            // read file from project /Config *.json
            foreach (var file in Directory.GetFiles(ConfigPath).Where(m => CheckFileExtension(Path.GetExtension(m))))
            {
                var fileName = Path.GetFileName(file);
                try
                {
                    if (fileName.IsNullOrEmpty()) continue;
                    if (configUpdateTime.TryGetValue(fileName, out var lastModifyTime) &&
                        lastModifyTime == Directory.GetLastWriteTime(file)) continue;

                    dict = new Dictionary<string, string>();
                    var text = File.ReadAllText(file);
                    dict.Add(fileName, text);
                    configUpdateTime[fileName] = Directory.GetLastWriteTime(file);
                    _updateFlagDict.AddOrUpdate($"{nameof(ConfigType.JsonConfig)}:{fileName}", k => text.Md5Encrypt(),
                        (t, l) => text.Md5Encrypt());
                }
                catch (Exception ex)
                {
                    LogHelper.Warn($"LoadJsonConfig({fileName}) handler error ,{ex.Message}", ex);
                }
            }

            if (dict != null && dict.Any()) ConfigCenterContext.SetJsonConfigList(dict);


            return new Tuple<int, string, string>(0, "刷新成功", "");
        }

        private void LoadAllDomainSwitch()
        {
            try
            {
                var dict = DomainSwitch.GetAllDomainDict();
                if (dict != null && dict.Any())
                {
                    var configMd5 = dict.ToJsonStr().Md5Encrypt();
                    if (_updateFlagDict.TryGetValue(nameof(ConfigType.DomainSwitch), out var lastMd5) &&
                        string.Equals(configMd5, lastMd5, StringComparison.OrdinalIgnoreCase)) return;
                    ConfigCenterContext.SetDomainSwitch(dict);
                    _updateFlagDict.TryAdd(nameof(ConfigType.DomainSwitch), configMd5);
                    configUpdateTime[nameof(ConfigType.DomainSwitch)] = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Warn($"LoadAllDomainSwitch handler error ,{ex.Message}", ex);
            }
        }

        private void LoadAllAppSettings()
        {
            try
            {
                var dict = new Dictionary<string, string>();
#if NET45
                var allKeys = ConfigurationManager.AppSettings.AllKeys;
                foreach (var key in allKeys) dict.Add(key, ConfigurationManager.AppSettings[key]);
#else
                var appsettingSection = _configuration.GetSection(nameof(ConfigType.AppSettings));
                var appsettings = _configuration.GetConfiguration(new[] { appsettingSection });
                foreach (var item in appsettings)
                {
                    if (item.Value is string)
                    {
                        dict.Add(item.Key, item.Value.ToString());
                    }
                    else if (item.Value is IList)
                    {
                        var asList = item.Value.To<List<AppSettingInfo>>();
                        if (asList != null)
                            asList.ForEach(m => dict.Add(m.Key, m.Value));
                    }
                    else
                    {
                        var aS = item.Value.To<AppSettingInfo>();
                        if (aS != null)
                            dict.Add(aS.Key, aS.Value);
                    }
                }
#endif
                if (dict != null && dict.Any())
                {
                    var configMd5 = dict.ToJsonStr().Md5Encrypt();
                    if (_updateFlagDict.TryGetValue(nameof(ConfigType.AppSettings), out var lastMd5) &&
                        string.Equals(configMd5, lastMd5, StringComparison.OrdinalIgnoreCase)) return;
                    ConfigCenterContext.SetAppSettings(dict);
                    _updateFlagDict.TryAdd(nameof(ConfigType.AppSettings), configMd5);
                    configUpdateTime[nameof(ConfigType.AppSettings)] = DateTime.Now;
                }

            }
            catch (Exception ex)
            {
                LogHelper.Warn($"LoadAllAppSettings handler error ,{ex.Message}", ex);
            }
        }

        private void LoadAllConnectionStrings()
        {
            try
            {
                var dict = new Dictionary<string, string>();
#if NET45
                var count = ConfigurationManager.ConnectionStrings.Count;
                for (var i = 0; i < count; i++)
                {
                    var key = ConfigurationManager.ConnectionStrings[i].Name;
                    var value = ConfigurationManager.ConnectionStrings[i].ConnectionString;
                    dict.Add(key, value);
                }
#else
                var connectionStringSection = _configuration.GetSection(nameof(ConfigType.ConnectionStrings));
                var connectionStrings = _configuration.GetConfiguration(new[] { connectionStringSection });
                foreach (var item in connectionStrings)
                {
                    if (item.Value is string)
                    {
                        dict.Add(item.Key, item.Value.ToString());
                    }
                    else if (item.Value is IList)
                    {
                        var asList = item.Value.To<List<ConnectionStringInfo>>();
                        if (asList != null)
                            asList.ForEach(m => dict.Add(m.Key, m.Value));
                    }
                    else
                    {
                        var aS = item.Value.To<ConnectionStringInfo>();
                        if (aS != null)
                            dict.Add(aS.Key, aS.Value);
                    }
                }
#endif
                if (dict != null && dict.Any())
                {
                    var configMd5 = dict.ToJsonStr().Md5Encrypt();
                    if (_updateFlagDict.TryGetValue(nameof(ConfigType.ConnectionStrings), out var lastMd5) &&
                        string.Equals(configMd5, lastMd5, StringComparison.OrdinalIgnoreCase)) return;
                    ConfigCenterContext.SetConnectionStrings(dict);
                    _updateFlagDict.TryAdd(nameof(ConfigType.ConnectionStrings), configMd5);
                    configUpdateTime[nameof(ConfigType.ConnectionStrings)] = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Warn($"LoadAllConnectionStrings handler error ,{ex.Message}", ex);
            }
        }

        #endregion //end Private

#if !NET45



#endif

    }
}