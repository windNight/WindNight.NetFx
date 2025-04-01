//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection.WnExtension;
//using WindNight.ConfigCenter.Extension;
//using WindNight.Core;

//namespace WindNight.Config.Extensions
//{
//    public partial class DefaultConfigItems
//    {
//        protected const string ZeroString = ConstantKeys.ZeroString;
//        protected const int ZeroInt = ConstantKeys.ZeroInt;
//        protected const long ZeroInt64 = ConstantKeys.ZeroInt64;
//        protected const decimal ZeroDecimal = ConstantKeys.ZeroDecimal;

//        protected static readonly string[] TrueStrings = ConstantKeys.TrueStrings,
//            FalseStrings = ConstantKeys.FalseStrings;
//    }

//    public partial class DefaultConfigItems
//    {
//        protected static IEnumerable<string> GetAppSettingList(string configKey, IEnumerable<string> defaultValue = null,
//            bool isThrow = false, bool needDistinct = true)
//        {
//            if (defaultValue == null)
//            {
//                defaultValue = Enumerable.Empty<string>();
//            }

//            return GetAppSettingList(configKey, m => m, defaultValue, isThrow, needDistinct);
//        }

//        protected static IEnumerable<int> GetAppSettingList(string configKey, IEnumerable<int> defaultValue = null,
//            bool isThrow = false, bool needDistinct = true)
//        {
//            if (defaultValue == null)
//            {
//                defaultValue = Enumerable.Empty<int>();
//            }

//            return GetAppSettingList(configKey, m => m.ToInt(), defaultValue, isThrow, needDistinct);
//        }

//        protected static IEnumerable<T> GetAppSettingList<T>(string configKey, Func<string, T> convertFunc,
//            IEnumerable<T> defaultValue = null, bool isThrow = false, bool needDistinct = true)
//        {
//            if (defaultValue == null)
//            {
//                defaultValue = Enumerable.Empty<T>();
//            }
//            return Configuration.GetAppSettingList(configKey, convertFunc, defaultValue, isThrow, needDistinct);

//        }
//    }

//    public partial class DefaultConfigItems
//    {

//        /// <summary>
//        ///     AppSettings:xxx
//        /// </summary>
//        /// <param name="keyName"></param>
//        /// <param name="defaultValue"></param>
//        /// <param name="isThrow"></param>
//        /// <returns></returns>
//        protected static string GetAppSettingValue(string keyName, string defaultValue = "", bool isThrow = false)
//        {
//            return Configuration.GetAppSettingValue(keyName, defaultValue, isThrow);
//        }

//        protected static bool GetAppSettingValue(string keyName, bool defaultValue = false, bool isThrow = false)
//        {
//            return Configuration.GetAppSettingValue(keyName, defaultValue, isThrow);
//        }


//        protected static decimal GetAppSettingValue(string keyName, decimal defaultValue = 0m, bool isThrow = false)
//        {
//            return Configuration.GetAppSettingValue(keyName, defaultValue, isThrow);
//        }

//        protected static int GetAppSettingValue(string keyName, int defaultValue = 0, bool isThrow = false)
//        {
//            return Configuration.GetAppSettingValue(keyName, defaultValue, isThrow);
//        }


//        protected static long GetAppSettingValue(string keyName, long defaultValue = 0, bool isThrow = false)
//        {
//            return Configuration.GetAppSettingValue(keyName, defaultValue, isThrow);
//        }


//    }

//    public partial class DefaultConfigItems
//    {
//        protected static IConfiguration Configuration => Ioc.GetService<IConfiguration>();

//        protected static int SystemAppId => Configuration?.GetAppId() ?? 0;
//        protected static string SystemAppCode => Configuration?.GetAppCode() ?? "";
//        protected static string SystemAppName => Configuration?.GetAppName() ?? "";

//        /// <summary>
//        ///  指定任意节点 可获取单个配置
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="keyName"></param>
//        /// <param name="defaultValue"></param>
//        /// <param name="isThrow"></param>
//        /// <returns></returns>
//        protected static T GetConfigValue<T>(string keyName, T defaultValue = default, bool isThrow = false)
//        {
//            return Configuration.GetConfigValue(keyName, defaultValue, isThrow);
//        }

//        /// <summary>
//        ///  指定某个根节点
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="sectionKey"></param>
//        /// <param name="defaultValue"></param>
//        /// <param name="isThrow"></param>
//        /// <returns></returns>
//        protected static T GetSectionValue<T>(string sectionKey = "", T defaultValue = default, bool isThrow = false)
//            where T : class, new()
//        {
//            return Configuration.GetSectionValue(sectionKey, defaultValue, isThrow);
//        }

//        /// <summary>
//        ///  指定某个根节点
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="sectionKey"></param>
//        /// <param name="defaultValue"></param>
//        /// <param name="isThrow"></param>
//        /// <returns></returns>
//        protected static T GetSectionConfigValue<T>(string sectionKey, T defaultValue = default, bool isThrow = false)
//        {
//            return Configuration.GetSectionConfigValue(sectionKey, defaultValue, isThrow);
//        }

//        protected static string GetConnectionString(string configKey, string defaultValue = "", bool isThrow = true)
//        {
//            return Configuration.GetConnStr(configKey, defaultValue, isThrow);
//        }
//    }


//}
