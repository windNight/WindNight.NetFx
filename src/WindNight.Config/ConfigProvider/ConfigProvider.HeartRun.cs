using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Extensions;
using System.Threading;
using Newtonsoft.Json.Extension;
using WindNight.Config.@internal;
using WindNight.ConfigCenter.Extension.@internal;
using WindNight.Core;
using WindNight.Linq.Extensions.Expressions;


#if !NET45
using Microsoft.Extensions.Configuration.Extensions;

#endif

namespace WindNight.ConfigCenter.Extension
{
    internal partial class ConfigProvider
    {

        private bool _isStop;

        /// <summary>
        ///  毫秒数
        /// </summary>
        private int SleepTime { get; set; } = 5 * 1000;

        private static Thread _thread;

        #region Private

        /// <summary>
        ///     注册心跳刷新
        /// </summary>
        private void RegisterHeartRun()
        {
            try
            {
                _thread = new Thread(HeartRun)
                {
                    Name = $"KeepAliveThread:ConfigProvider",
                    IsBackground = true,

                };
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
            var loopStartTimeTicks = DateTime.Now.Ticks;
            var loop = 0;


            while (true)
            {
                if (_isStop)
                    break;
                Thread.Sleep(SleepTime);
                var flag = false;
                var isContinue = false;
                try
                {
                    var ttl = (int)TimeSpan.FromTicks(DateTime.Now.Ticks - loopStartTimeTicks).TotalMilliseconds;
                    if (ttl < SleepTime)
                    {
                        Thread.Sleep(100);
                        isContinue = true;
                        continue;
                    }

                    LoadAllConfigs();
                    flag = true;
                    Thread.Sleep(20);
                }
                catch (Exception ex)
                {
                    LogHelper.Warn($"{_thread.Name} LoadAllConfigs handler error ,{ex.Message}", ex);
                }
                finally
                {
                    // 成功后重置
                    if (flag)
                        loopStartTimeTicks = DateTime.Now.Ticks;

                }
            }
        }

        private void LoadAllConfigs()
        {
            LoadConfigFile();

            // LoadAllDomainSwitch();
#if !NET45
            if (_configuration == null)
            {
                Thread.Sleep(5 * 1000);
                return;
            }
#endif

            LoadAllAppSettings();
            LoadAllConnectionStrings();
        }

        bool ConfigFileChanged(string fileName)
        {
            if (!CheckConfigFileIsValid(fileName))
            {
                return false;
            }

            var filePath = Path.Combine(ConfigPath, fileName);
            if (configUpdateTime.TryGetValue(fileName, out var lastModifyTime) && lastModifyTime == Directory.GetLastWriteTime(filePath))
            {
                return false;
            }

            return true;
        }

        private Tuple<int, string, string> LoadConfigByFileName(string configName)
        {
            if (!CheckConfigFileIsValid(configName))
            {
                return new Tuple<int, string, string>(0, "刷新成功", "");
            }
            if (!ConfigFileChanged(configName))
            {
                return new Tuple<int, string, string>(0, "刷新成功", "");
            }

            var config = ReadConfigFileDirect(configName);
            if (config == null)
            {
                return new Tuple<int, string, string>(0, "刷新成功", "");
            }

            var content = config.FileContent;
            if (!content.IsNullOrEmpty())
            {
                ConfigCenterContext.SetConfig(configName, content);
                configUpdateTime[configName] = config.LastModifyTime;
                var configType = configName.ParserConfigType();
                var tagName = $"{configType}:{configName}";

                _updateFlagDict.AddOrUpdate(tagName, k => content.Md5Encrypt(),
                    (t, l) => content.Md5Encrypt());

            }


            return new Tuple<int, string, string>(0, "刷新成功", content);
        }


        string FixDefaultConfigFilePath(string fileName)
        {
            return Path.Combine(ConfigPath, fileName);
        }

        private bool CheckConfigFileIsValid(string fileName)
        {
            return CheckFileExtension(fileName) && File.Exists(FixDefaultConfigFilePath(fileName));
        }
        private static List<string> ValidExtensions => new List<string> { ".json", ".config", ".xml" };

        private bool CheckFileExtension(string filePath)
        {
            return ValidExtensions.Contains(Path.GetExtension(filePath));
        }





        private Tuple<int, string, string> LoadAllConfig()
        {
            Dictionary<string, string>? dict = null;
            // read file from project /Config *.json
            var configFiles = Directory.GetFiles(ConfigPath).Where(m => CheckFileExtension(Path.GetExtension(m)));
            dict = new Dictionary<string, string>();
            foreach (var file in configFiles)
            {
                var fileName = Path.GetFileName(file);
                //var fileExt = Path.GetExtension(file);
                var configType = fileName.ParserConfigType();
                var tagName = $"{configType}:{fileName}";

                try
                {
                    if (fileName.IsNullOrEmpty()) continue;
                    if (configUpdateTime.TryGetValue(fileName, out var lastModifyTime) &&
                        lastModifyTime == Directory.GetLastWriteTime(file)) continue;

                    var text = File.ReadAllText(file);
                    dict.Add(fileName, text);
                    configUpdateTime[fileName] = Directory.GetLastWriteTime(file);
                    _updateFlagDict.AddOrUpdate(tagName, k => text.Md5Encrypt(),
                        (t, l) => text.Md5Encrypt());
                }
                catch (Exception ex)
                {
                    LogHelper.Warn($"LoadJsonConfig({fileName}) handler error ,{ex.Message}", ex);
                }
            }

            if (!dict.IsNullOrEmpty())
            {
                ConfigCenterContext.SetConfigList(dict);
            }


            return new Tuple<int, string, string>(0, "刷新成功", "");
        }

        //private void LoadAllDomainSwitch()
        //{
        //    try
        //    {
        //        var dict = DomainSwitch.GetAllDomainDict();
        //        if (dict != null && dict.Any())
        //        {
        //            var configMd5 = dict.ToJsonStr().Md5Encrypt();
        //            if (_updateFlagDict.TryGetValue(nameof(ConfigType.DomainSwitch), out var lastMd5) &&
        //                string.Equals(configMd5, lastMd5, StringComparison.OrdinalIgnoreCase)) return;
        //            ConfigCenterContext.SetDomainSwitch(dict);
        //            _updateFlagDict.TryAdd(nameof(ConfigType.DomainSwitch), configMd5);
        //            configUpdateTime[nameof(ConfigType.DomainSwitch)] = HardInfo.Now;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Warn($"LoadAllDomainSwitch handler error ,{ex.Message}", ex);
        //    }
        //}

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
                        var asList = AnalyzeAppSettings(item);// item.Value.To<List<AppSettingInfo>>();

                        if (!asList.IsNullOrEmpty())
                        {
                            LogHelper.Debug($"AnalyzeAppSettings({item.Key})->{item.ToJsonStr()}");
                            asList.ForEach(m =>
                            {
                                var key = m.Key;
                                if (m.Loop)
                                {
                                    key = m.Path.Replace($"{nameof(ConfigType.AppSettings)}:", "");
                                }
                                dict.Add(key, m.Value);
                            });
                        }

                    }
                    else
                    {
                        var aS = item.Value.To<AppSettingInfo>();
                        if (aS != null)
                            dict.Add(aS.Key, aS.Value);
                    }
                }
#endif
                if (!dict.IsNullOrEmpty())
                {
                    var configMd5 = dict.ToJsonStr().Md5Encrypt();
                    if (_updateFlagDict.TryGetValue(nameof(ConfigType.AppSettings), out var lastMd5) &&
                        string.Equals(configMd5, lastMd5, StringComparison.OrdinalIgnoreCase)) return;
                    ConfigCenterContext.SetAppSettings(dict);
                    _updateFlagDict.TryAdd(nameof(ConfigType.AppSettings), configMd5);
                    configUpdateTime[nameof(ConfigType.AppSettings)] = HardInfo.Now;
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
                        if (!asList.IsNullOrEmpty())
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
                if (!dict.IsNullOrEmpty())
                {
                    var configMd5 = dict.ToJsonStr().Md5Encrypt();
                    if (_updateFlagDict.TryGetValue(nameof(ConfigType.ConnectionStrings), out var lastMd5) &&
                        string.Equals(configMd5, lastMd5, StringComparison.OrdinalIgnoreCase)) return;
                    ConfigCenterContext.SetConnectionStrings(dict);
                    _updateFlagDict.TryAdd(nameof(ConfigType.ConnectionStrings), configMd5);
                    configUpdateTime[nameof(ConfigType.ConnectionStrings)] = HardInfo.Now;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Warn($"LoadAllConnectionStrings handler error ,{ex.Message}", ex);
            }
        }


        List<AppSettingInfo> AnalyzeAppSettings(ConfigBaseInfo2 config, bool loop = false)
        {
            var list = new List<AppSettingInfo>();
            if (config.Value is IList<ConfigBaseInfo2> configList)
            {
                foreach (var item in configList)
                {
                    if (item.Value is IList<ConfigBaseInfo2>)
                    {
                        list.AddRange(AnalyzeAppSettings(item, true));
                    }
                    else
                    {
                        list.Add(new AppSettingInfo
                        {
                            Key = item.Key,
                            Path = item.Path,
                            Value = item.Value.ToString(),
                            Loop = loop,
                        });
                    }
                }
            }
            else
            {
                list.Add(new AppSettingInfo
                {
                    Key = config.Key,
                    Path = config.Path,
                    Value = config.Value.ToString(),
                    Loop = loop,
                });
            }

            return list;
        }


        #endregion //end Private

        public JsonFileConfigInfo ReadConfigFileDirect(string fileName)
        {
            if (!CheckFileExtension(fileName))
            {
                return null;
            }

            var filePath = Path.Combine(ConfigPath, fileName);

            if (!File.Exists(filePath))
            {
                return new JsonFileConfigInfo
                {
                    Path = filePath,
                    FileContent = string.Empty,
                    FileName = fileName,
                    LastModifyTime = default,
                };
            }
            var text = File.ReadAllText(filePath);
            var lastModifyTime = Directory.GetLastWriteTime(filePath);

            return new JsonFileConfigInfo
            {
                Path = filePath,
                FileContent = text,
                FileName = fileName,
                LastModifyTime = lastModifyTime,
            };

        }



        public FileConfigInfo ReadSelfConfigFileDirect(string fileDir, string fileName)
        {
            // var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileDir, fileName);
            var filePath = Path.Combine(Environment.CurrentDirectory, FixDirPath(fileDir), fileName);
           
            if (!File.Exists(filePath))
            {
                return new FileConfigInfo
                {
                    Path = filePath,
                    FileContent = string.Empty,
                    FileName = fileName,
                    LastModifyTime = default,
                };
            }
            var text = File.ReadAllText(filePath);
            var lastModifyTime = Directory.GetLastWriteTime(filePath);

            return new FileConfigInfo
            {
                Path = filePath,
                FileContent = text,
                FileName = fileName,
                LastModifyTime = lastModifyTime,
            };

        }



    }
}
