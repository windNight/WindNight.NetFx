using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WindNight.AspNetCore.Mvc.Extensions;
using WindNight.Config.Extensions.Attributes;
using WindNight.ConfigCenter.Extension;
using WindNight.Core.Attributes.Abstractions;
using WindNight.Core.Extension;
using WindNight.Extension;

namespace WindNight.Config.Extensions
{

    /// <summary>
    ///   配置中心默认接口
    /// </summary>
    [Route("api/configcenter")]
    [CenterApiAuth]
    [SysApi, NonAuth]
    // [ApiExplorerSettings(IgnoreApi = true)]
    public partial class ConfigController : ControllerBase
    {
        /// <summary>
        ///  获取内存里的 AppSetting 配置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("appsettings")]
        public List<AppSettingInfo> QueryAppSettingList()
        {
            if (!TokenAuthed)
            {
                return null;
            }

            return ConfigItemsBase.GetAppSettingList();
        }

        /// <summary>
        ///  获取内存里的 ConnectionString 配置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("connections")]
        public List<ConnectionStringInfo> QueryConnectionStringList()
        {
            if (!TokenAuthed)
            {
                return null;
            }
            return ConfigItemsBase.GetConnectionStringList();
        }

        /// <summary>
        ///  获取后端Json配置项
        /// </summary>
        /// <returns></returns>
        [HttpGet("jsonconfigs")]
        public List<string> QueryJsonConfigList()
        {
            if (!TokenAuthed)
            {
                return null;
            }
            return ConfigItemsBase.GetJsonConfigList().Select(m => m.FileName).ToList();
        }

        /// <summary>
        ///   获取后端Xml配置项
        /// </summary>
        /// <returns></returns>
        [HttpGet("xmlconfigs")]
        public List<string> QueryXmlConfigList()
        {
            if (!TokenAuthed)
            {
                return null;
            }
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
            if (!TokenAuthed)
            {
                return null;
            }
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
            if (!TokenAuthed)
            {
                return null;
            }
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
            if (!TokenAuthed)
            {
                return null;
            }
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
            if (!TokenAuthed)
            {
                return null;
            }
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
            if (!TokenAuthed)
            {
                return null;
            }
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
            if (!TokenAuthed)
            {
                return null;
            }
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
            if (!TokenAuthed)
            {
                return null;
            }
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
            if (!TokenAuthed)
            {
                return null;
            }
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
            if (!TokenAuthed)
            {
                return null;
            }
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
            if (!TokenAuthed)
            {
                return null;
            }
            return ConfigItemsBase.FetchSelfConfigFileInfos(fileDir).ToList();
        }


        /// <summary>
        ///  获取后端项目的配置文件名列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("config/filenames")]
        public List<string> QueryConfigNamesDirect()
        {
            if (!TokenAuthed)
            {
                return null;
            }
            return ConfigItemsBase.FetchConfigNames().ToList();
        }

        /// <summary>
        ///  获取后端项目的配置文件信息列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("config/fileinfos")]
        public List<ConfigFileBaseInfo> FetchConfigFileInfosDirect()
        {
            if (!TokenAuthed)
            {
                return null;
            }
            return ConfigItemsBase.FetchConfigFileInfos().ToList();
        }


        /// <summary>
        ///  获取内存中配置文件的更新标记 md5
        /// </summary>
        /// <returns></returns>
        [HttpGet("updateflag")]
        public Dictionary<string, string> QueryUpdateFlagDict()
        {
            if (!TokenAuthed)
            {
                return null;
            }
            return ConfigItemsBase.GetUpdateFlagDict();
        }

        /// <summary>
        ///  获取内存中各个配置文件的最后更新时间
        /// </summary>
        /// <returns></returns>
        [HttpGet("configupdatetime")]
        public Dictionary<string, DateTime> QueryConfigUpdateTime()
        {
            if (!TokenAuthed)
            {
                return null;
            }
            return ConfigItemsBase.GetConfigUpdateTime();
        }


        /// <summary>
        ///   获取内存中 所有的配置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("config/current")]
        public Dictionary<string, string> QueryCurrentConfiguration()
        {
            if (!TokenAuthed)
            {
                return null;
            }
            return ConfigItemsBase.GetCurrentConfiguration();
        }

    }

    public partial class ConfigController
    {

    }
    public partial class ConfigController
    {
        protected virtual bool TokenAuthed => AccessTokenIsAuth() || AppTokenIsAuth() || HttpClientIpIsLocal();


        protected virtual bool AccessTokenIsAuth()
        {
            var ak = GetAccessToken();
            if (ak.IsNullOrEmpty())
            {
                return false;
            }

            return true;
        }

        protected virtual bool AppTokenIsAuth()
        {
            var appToken = GetAppTokenValue();
            if (appToken.IsNullOrEmpty())
            {
                return false;
            }

            return true;
        }

        protected virtual string GetAccessToken()
        {
            var authorizationValue = GetAuthorizationValue();

            if (authorizationValue.IsNullOrEmpty())
            {
                return string.Empty;
            }

            var akArray = authorizationValue.Split(' ');
            if (akArray.Length == 2)
            {
                var akType = akArray[0];
                var ak = akArray[1];
                return ak;
            }

            return string.Empty;

        }

        protected virtual string GetUserAgentValue() => Request.GetUserAgentValue();

        protected virtual string GetAppTokenValue() => Request.GetAppTokenValue();

        protected virtual string GetAuthorizationValue() => Request.GetAuthorizationValue();
        protected virtual string GetRequestAppCodeValue() => Request.GetAppCodeValue();
        protected virtual string GetRequestAppNameValue() => Request.GetAppNameValue();

        protected virtual long GetRequestTsValue() => Request.GetTimestampValue();




        protected virtual string GetHttpServerIp(bool onlyIpV4 = true)
        {
            var value = Request.HttpContext.GetServerIp(onlyIpV4);
            return value;
        }

        protected virtual string GetHttpClientIp(bool onlyIpV4 = true)
        {
            var value = Request.HttpContext.GetClientIp(onlyIpV4);
            return value;
        }

        protected virtual Dictionary<string, string> GetHttpClientIps()
        {
            var value = Request.HttpContext.GetClientIps();
            return value;
        }

        protected virtual bool HttpClientIpIsPrivate()
        {
            var clientIp = GetHttpClientIp();
            if (clientIp.IsPrivateOrLoopback())
            {
                return true;
            }

            return false;

        }

        protected virtual bool HttpClientIpIsLocal()
        {
            var clientIp = GetHttpClientIp();
            if (clientIp.IsDefaultIp())
            {
                return true;
            }
            return false;
        }

        protected string QueryHeaderValue(string headerName, string defaultValue = "")
        {
            if (Request.Headers.TryGetValue(headerName, out var requestHeader))
            {
                var header = requestHeader.FirstOrDefault();
                if (!header.IsNullOrEmpty())
                {
                    return header.Trim();
                }
            }

            return defaultValue;
        }



    }

}
