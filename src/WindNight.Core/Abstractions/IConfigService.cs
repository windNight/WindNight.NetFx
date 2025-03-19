#if !NET45
using Microsoft.Extensions.Configuration;
#endif

namespace WindNight.Core.Abstractions
{
    /// <summary>
    /// </summary>
    public interface IConfigService
    {
        /// <summary>
        /// </summary>
        /// <param name="connKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        string GetConnString(string connKey, string defaultValue = "", bool isThrow = true);

        /// <summary>
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        string GetAppSetting(string configKey, string defaultValue = "", bool isThrow = true);

        /// <summary>
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        int GetAppSetting(string configKey, int defaultValue = 0, bool isThrow = true);

        /// <summary>
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        long GetAppSetting(string configKey, long defaultValue = 0L, bool isThrow = true);

        /// <summary>
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        bool GetAppSetting(string configKey, bool defaultValue = false, bool isThrow = true);

        /// <summary>
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        string GetFileConfigString(string fileName, string defaultValue = "", bool isThrow = true);

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        T GetFileConfig<T>(string fileName, bool isThrow = true) where T : new();

#if !NET45
        IConfiguration Configuration { get; }

        /// <summary> 服务编号 </summary>
        int SystemAppId { get; }

        /// <summary> 服务代号 </summary>
        string SystemAppCode { get; }

        /// <summary> 服务名称 </summary>
        string SystemAppName { get; }

        T GetSectionValue<T>(string sectionKey = "", T defaultValue = default, bool isThrow = false)
            where T : class, new()
            ;

        T GetConfigValue<T>(string keyName, T defaultValue = default, bool isThrow = false);
#endif
    }


    public interface IConfigItems
    {
    }
}
