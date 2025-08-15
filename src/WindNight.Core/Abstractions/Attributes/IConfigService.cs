using System;
using System.Collections.Generic;
#if !NET45
using Microsoft.Extensions.Configuration;
#endif

namespace WindNight.Core.Abstractions
{
    /// <summary>
    /// </summary>
    public interface IConfigService : ISectionConfig, IConnectionConfig, IAppSettingConfig, IFileConfig
    {
        IConfiguration Configuration { get; }

        /// <summary> 服务编号 </summary>
        string SystemAppId { get; }

        /// <summary> 服务代号 </summary>
        string SystemAppCode { get; }

        /// <summary> 服务名称 </summary>
        string SystemAppName { get; }
    }

    public interface ISectionConfig
    {
        /// <summary>
        ///     指定某个根节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sectionKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        T GetSectionValue<T>(string sectionKey = "", T defaultValue = default, bool isThrow = false)
            where T : class, new();

        /// <summary>
        ///     指定任意节点 可获取单个配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        T GetConfigValue<T>(string keyName, T defaultValue = default, bool isThrow = false);
    }

    public interface IConnectionConfig
    {
        /// <summary>
        /// </summary>
        /// <param name="connKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        string GetConnString(string connKey, string defaultValue = "", bool isThrow = true);
    }

    public interface IAppSettingListConfig
    {
        IEnumerable<string> GetAppSettingList(string configKey, IEnumerable<string> defaultValue = null,
            bool isThrow = false, bool needDistinct = true);

        IEnumerable<int> GetAppSettingList(string configKey, IEnumerable<int> defaultValue = null, bool isThrow = false,
            bool needDistinct = true);

        IEnumerable<T> GetAppSettingList<T>(string configKey, Func<string, T> convert,
            IEnumerable<T> defaultValue = null, bool isThrow = true, bool needDistinct = true);
    }

    public interface IAppSettingConfig : IAppSettingListConfig
    {
        /// <summary>
        ///     config string in AppSettings
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        string GetAppSettingValue(string configKey, string defaultValue = "", bool isThrow = false);


        /// <summary>
        ///     config int in AppSettings
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        int GetAppSettingValue(string configKey, int defaultValue = 0, bool isThrow = false);

        /// <summary>
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        long GetAppSettingValue(string configKey, long defaultValue = 0L, bool isThrow = false);

        /// <summary>
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        bool GetAppSettingValue(string configKey, bool defaultValue = false, bool isThrow = false);

        decimal GetAppSettingValue(string keyName, decimal defaultValue = 0m, bool isThrow = false);





    }

    public interface IFileConfig
    {
        /// <summary>
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns> 
        string GetFileConfigString(string fileName, string defaultValue = "", bool isThrow = false);

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns> 
        T GetFileConfig<T>(string fileName, bool isThrow = false) where T : new();
    }


    public interface IConfigItems
    {
    }
}
