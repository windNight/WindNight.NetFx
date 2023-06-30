//using System;
//using System.Globalization;
//using Microsoft.Extensions.DependencyInjection.WnExtension;
//using Newtonsoft.Json.Extension;
//using WindNight.Core.Abstractions;
//using WindNight.Extension.Internals;

//namespace WindNight.NetCore.Extension
//{
//    public class ConfigItemsBase
//    {
//        protected const string TrueString = "1", FalseString = "0", ZeroString = "0";
//        protected const int ZeroInt = 0;

//        /// <summary>
//        ///     根据键名获取连接字符串
//        /// </summary>
//        /// <param name="connKey"></param>
//        /// <param name="isThrow"></param>
//        /// <returns></returns>
//        protected static string GetConnStringValue(string connKey, bool isThrow = true)
//            => GetConnStringValue(connKey, "", isThrow);

//        /// <summary>
//        /// </summary>
//        /// <param name="connKey"></param>
//        /// <param name="defaultValue"></param>
//        /// <param name="isThrow"></param>
//        /// <returns></returns>
//        protected static string GetConnStringValue(string connKey, string defaultValue, bool isThrow = true)
//        {
//            var configService = Ioc.Instance.CurrentConfigService;
//            var configValue = configService?.GetConnString(connKey, defaultValue, isThrow);
//            if (configValue.IsNullOrEmpty())
//            {
//                if (isThrow)
//                {
//                    if (!defaultValue.IsNullOrEmpty()) return defaultValue;
//                    throw new ArgumentException($"未能找到 【{connKey}】节点的相关配置");
//                }

//                configValue = defaultValue;
//            }

//            return configValue;
//        }

//        /// <summary>
//        /// </summary>
//        /// <param name="configKey"></param>
//        /// <param name="isThrow"></param>
//        /// <returns></returns>
//        protected static string GetConfigValue(string configKey, bool isThrow = true)
//            => GetConfigValue(configKey, "", isThrow);

//        /// <summary>
//        /// </summary>
//        /// <param name="configKey"></param>
//        /// <param name="defaultValue"></param>
//        /// <param name="isThrow"></param>
//        /// <returns></returns>
//        protected static int GetConfigValue(string configKey, int defaultValue = 0, bool isThrow = true)
//            => GetConfigValue(configKey, defaultValue.ToString(), isThrow).ToInt(defaultValue);

//        /// <summary>
//        /// </summary>
//        /// <param name="configKey"></param>
//        /// <param name="defaultValue"></param>
//        /// <param name="isThrow"></param>
//        /// <returns></returns>
//        protected static long GetConfigValue(string configKey, long defaultValue = 0L, bool isThrow = true)
//            => GetConfigValue(configKey, defaultValue.ToString(), isThrow).ToLong(defaultValue);

//        /// <summary>
//        /// </summary>
//        /// <param name="configKey"></param>
//        /// <param name="defaultValue"></param>
//        /// <param name="isThrow"></param>
//        /// <returns></returns>
//        protected static decimal GetConfigValue(string configKey, decimal defaultValue = 0M, bool isThrow = true) =>
//            GetConfigValue(configKey, defaultValue.ToString(CultureInfo.InvariantCulture), isThrow)
//                .ToDecimal(defaultValue);

//        /// <summary>
//        /// </summary>
//        /// <param name="configKey"></param>
//        /// <param name="defaultValue"></param>
//        /// <param name="isThrow"></param>
//        /// <returns></returns>
//        protected static bool GetConfigValue(string configKey, bool defaultValue, bool isThrow)
//            => GetConfigValue(configKey, defaultValue ? TrueString : FalseString, isThrow) == TrueString;

//        /// <summary>
//        /// </summary>
//        /// <param name="configKey"></param>
//        /// <param name="defaultValue"></param>
//        /// <param name="isThrow"></param>
//        /// <returns></returns>
//        protected static string GetConfigValue(string configKey, string defaultValue, bool isThrow = true)
//        {
//            string configValue;
//            try
//            {
//                var configService = Ioc.Instance.CurrentConfigService;
//                configValue = configService?.GetAppSetting(configKey, defaultValue, isThrow);

//                if (configValue.IsNullOrEmpty())
//                {
//                    if (isThrow)
//                    {
//                        if (!defaultValue.IsNullOrEmpty()) return defaultValue;
//                        throw new ArgumentException($"未能找到 【{configKey}】节点的相关配置");
//                    }

//                    configValue = defaultValue;
//                }
//            }
//            catch (Exception ex)
//            {
//                if (isThrow)
//                {
//                    if (!defaultValue.IsNullOrEmpty()) return defaultValue;
//                    throw new ArgumentException($"未能找到 【{configKey}】节点的相关配置", ex);
//                }

//                configValue = defaultValue;
//            }

//            return configValue;
//        }


//        /// <summary>
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="fileName"></param>
//        /// <param name="isThrow"></param>
//        /// <returns></returns>
//        protected static T GetComplexValue<T>(string fileName, bool isThrow = true) where T : new()
//        {
//            var configValue = GetComplexString(fileName, "", isThrow);
//            if (configValue.IsNullOrEmpty())
//            {
//                if (isThrow) throw new ArgumentException($"未能找到 【{fileName}】相关配置");
//                return new T();
//            }

//            var model = configValue.To<T>();
//            if (model == null) return new T();
//            return model;
//        }

//        /// <summary>
//        /// </summary>
//        /// <param name="fileName"></param>
//        /// <param name="defaultValue"></param>
//        /// <param name="isThrow"></param>
//        /// <returns></returns>
//        protected static string GetComplexString(string fileName, string defaultValue = "", bool isThrow = true)
//        {
//            var configService = Ioc.Instance.CurrentConfigService;
//            var configValue = configService?.GetFileConfigString(fileName, defaultValue, isThrow);
//            if (configValue.IsNullOrEmpty())
//            {
//                if (isThrow) throw new ArgumentException($"未能找到 【{fileName}】相关配置");
//                configValue = defaultValue;
//            }

//            return configValue;

//        }

//        static string GetConfig(Func<string, string> func, string configKey, string defaultValue = "", bool isThrow = false)
//        {
//            if (configKey.IsNullOrEmpty()) return defaultValue;
//            var configValue = string.Empty;
//            try
//            {
//                configValue = func.Invoke(configKey);
//            }
//            catch (Exception ex)
//            {
//                LogHelper.Warn($"ReadFromConfig({configKey}) Handler Error {ex.Message} ", ex);
//                if (isThrow)
//                    throw;
//            }
//            if (configValue.IsNullOrEmpty() && !defaultValue.IsNullOrEmpty()) configValue = defaultValue;

//            return configValue;
//        }


//    }
//}