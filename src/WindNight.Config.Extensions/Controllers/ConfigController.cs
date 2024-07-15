using System;
using System.Attributes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.WnExtensions.Abstractions.Attributes;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Config.Abstractions;
using WindNight.Config.Extensions.Attributes;
using WindNight.ConfigCenter.Extension;

namespace WindNight.Config.Extensions
{

    /// <summary>
    ///   配置中心默认接口
    /// </summary>
    [Route("api/configcenter")]
    [HiddenApi]
    [CenterApiAuth]
    [NonAuth]
    public class ConfigController : ControllerBase
    {
        /// <summary>
        ///  获取内存里的 AppSetting 配置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("appsettings")]
        public List<AppSettingInfo> QueryAppSettingList()
        {
            return ConfigItemsBase.GetAppSettingList();
        }

        /// <summary>
        ///  获取内存里的 ConnectionString 配置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("connections")]
        public List<ConnectionStringInfo> QueryConnectionStringList()
        {
            return ConfigItemsBase.GetConnectionStringList();
        }

        /// <summary>
        ///  获取后端Json配置项
        /// </summary>
        /// <returns></returns>
        [HttpGet("jsonconfigs")]
        public List<string> QueryJsonConfigList()
        {
            return ConfigItemsBase.GetJsonConfigList().Select(m => m.FileName).ToList();
        }

        /// <summary>
        ///   获取后端Xml配置项
        /// </summary>
        /// <returns></returns>
        [HttpGet("xmlconfigs")]
        public List<string> QueryXmlConfigList()
        {
            return ConfigItemsBase.GetXmlConfigList().Select(m => m.FileName).ToList();
        }

        /// <summary>
        ///   获取内存中指定json文件的配置内容
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet("jsonconfig/byfilename")]
        public JsonFileConfigInfo QueryJsonConfigContent(string fileName)
        {
            return ConfigItemsBase.GetJsonConfigList().FirstOrDefault(m => m.FileName == fileName);
        }

        /// <summary>
        ///  获取内存中指定xml文件的配置内容
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet("xmlconfig/byfilename")]
        public XmlFileConfigInfo QueryXmlConfigContent(string fileName)
        {
            return ConfigItemsBase.GetXmlConfigList().FirstOrDefault(m => m.FileName == fileName);
        }

        /// <summary>
        ///  获取内存中指定文件的配置内容
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet("config/byfilename")]
        public FileConfigInfo QueryConfigContent(string fileName)
        {
            // 根据后缀 实际获取对应的配置文件
            return ConfigItemsBase.ReadConfigFileDirect(fileName);
        }

        /// <summary>
        ///  直接获取指定文件的配置内容
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet("config/byfilename/direct")]
        public FileConfigInfo QueryConfigContentDirect(string fileName)
        {
            return ConfigItemsBase.ReadConfigFileDirect(fileName);
        }

        /// <summary>
        ///  获取自定义文件下指定文件的配置内容
        /// </summary>
        /// <param name="fileDir"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet("selfconfig/byfilename/direct")]
        public FileConfigInfo ReadSelfConfigFileDirect(string fileDir, string fileName)
        {
            return ConfigItemsBase.ReadSelfConfigFileDirect(fileDir, fileName);
        }

        /// <summary>
        ///   获取后端项目下前端项目指定文件的配置内容
        /// </summary>
        /// <param name="fileDir"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet("frontconfig/byfilename/direct")]
        public FileConfigInfo ReadFrontConfigFileDirect(string fileDir, string fileName)
        {
            fileDir = $"wwwroot/{fileDir.TrimStart('/').TrimStart(Path.DirectorySeparatorChar)}";
            return ConfigItemsBase.ReadSelfConfigFileDirect(fileDir, fileName);
        }

        /// <summary>
        ///   获取后端项目下前端项目的配置文件名列表
        /// </summary>
        /// <param name="fileDir"></param>
        /// <returns></returns>
        [HttpGet("frontconfig/filenames")]
        public List<string> QueryFrontConfigNamesDirect(string fileDir)
        {
            fileDir = $"wwwroot/{fileDir.TrimStart('/').TrimStart(Path.DirectorySeparatorChar)}";
            return ConfigItemsBase.FetchSelfConfigNames(fileDir).ToList();
        }

        /// <summary>
        ///   获取后端项目下前端项目的配置文件名列表
        /// </summary>
        /// <param name="fileDir"></param>
        /// <returns></returns>
        [HttpGet("frontconfig/fileinfos")]
        public List<ConfigFileBaseInfo> QueryFrontConfigInfosDirect(string fileDir)
        {
            fileDir = $"wwwroot/{fileDir.TrimStart('/').TrimStart(Path.DirectorySeparatorChar)}";
            return ConfigItemsBase.FetchSelfConfigFileInfos(fileDir).ToList();
        }

        /// <summary>
        ///  获取自定义文件下的配置文件名列表
        /// </summary>
        /// <param name="fileDir"></param>
        /// <returns></returns>
        [HttpGet("selfconfig/filenames")]
        public List<string> QuerySelfConfigNamesDirect(string fileDir)
        {
            return ConfigItemsBase.FetchSelfConfigNames(fileDir).ToList();
        }

        /// <summary>
        ///  获取自定义文件下的配置文件信息列表
        /// </summary>
        /// <param name="fileDir"></param>
        /// <returns></returns>
        [HttpGet("selfconfig/fileinfos")]
        public List<ConfigFileBaseInfo> QuerySelfConfigFileInfosDirect(string fileDir)
        {
            return ConfigItemsBase.FetchSelfConfigFileInfos(fileDir).ToList();
        }


        /// <summary>
        ///  获取后端项目的配置文件名列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("config/filenames")]
        public List<string> QueryConfigNamesDirect()
        {
            return ConfigItemsBase.FetchConfigNames().ToList();
        }

        /// <summary>
        ///  获取后端项目的配置文件信息列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("config/fileinfos")]
        public List<ConfigFileBaseInfo> FetchConfigFileInfosDirect()
        {
            return ConfigItemsBase.FetchConfigFileInfos().ToList();
        }


        /// <summary>
        ///  获取内存中配置文件的更新标记 md5
        /// </summary>
        /// <returns></returns>
        [HttpGet("updateflag")]
        public Dictionary<string, string> QueryUpdateFlagDict()
        {
            return ConfigItemsBase.GetUpdateFlagDict();
        }

        /// <summary>
        ///  获取内存中各个配置文件的最后更新时间
        /// </summary>
        /// <returns></returns>
        [HttpGet("configupdatetime")]
        public Dictionary<string, DateTime> QueryConfigUpdateTime()
        {
            return ConfigItemsBase.GetConfigUpdateTime();
        }


        /// <summary>
        ///   获取内存中 所有的配置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("config/current")]
        public Dictionary<string, string> QueryCurrentConfiguration()
        {
            return ConfigItemsBase.GetCurrentConfiguration();
        }

    }


}
