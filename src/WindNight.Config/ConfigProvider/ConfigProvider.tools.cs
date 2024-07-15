using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WindNight.ConfigCenter.Extension
{
    /// <summary>
    ///     配置提供者
    /// </summary>
    internal partial class ConfigProvider
    {

        string FixDirPath(string fileDir)
        {
            return HardInfo.IsWindows
                ? fileDir.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
                : fileDir;
        }


        public IEnumerable<ConfigFileBaseInfo> FetchSelfConfigFileInfos(string fileDir)
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, FixDirPath(fileDir));
            if (!Directory.Exists(filePath))
            {

                return Empty<ConfigFileBaseInfo>();
            }

            return GetConfigFileInfos(filePath);

        }

        public IEnumerable<ConfigFileBaseInfo> FetchConfigFileInfos()
        {
            var filePath = ConfigPath;
            if (!Directory.Exists(filePath))
            {

                return Empty<ConfigFileBaseInfo>();

            }

            return GetConfigFileInfos(filePath);

        }


        IEnumerable<T> Empty<T>()
        {
#if NET45LATER
            return Array.Empty<T>();
#else
            return new List<T>();
#endif
        }


        public IEnumerable<string> FetchSelfConfigNames(string fileDir)
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, FixDirPath(fileDir));
            if (!Directory.Exists(filePath))
            {

                return Empty<string>();

            }

            var configFiles = GetFiles(filePath).Where(m => CheckFileExtension(Path.GetExtension(m)));
            var configFileNames = configFiles.Select(GetFileName).ToList();
            return configFileNames;

        }

        public IEnumerable<string> FetchConfigNames()
        {
            var filePath = ConfigPath;
            if (!Directory.Exists(filePath))
            {

                return Empty<string>();

            }

            var configFiles = GetFiles(filePath).Where(m => CheckFileExtension(Path.GetExtension(m)));
            var configFileNames = configFiles.Select(Path.GetFileName).ToList();
            return configFileNames;

        }



        DateTime GetLastWriteTime(string filePath)
        {
            try
            {
                return Directory.GetLastWriteTime(filePath);
            }
            catch (Exception ex)
            {
                return DateTime.MinValue;
            }
        }

        string ReadAllText(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath);

            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }


        string GetFileName(string filePath)
        {
            try
            {
                return Path.GetFileName(filePath);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        IEnumerable<string> GetFiles(string filePath)
        {

            try
            {

                return Directory.GetFiles(filePath);
            }
            catch (Exception ex)
            {
                return Empty<string>();
            }
        }


        IEnumerable<ConfigFileBaseInfo> GetConfigFileInfos(string filePath)
        {
            try
            {
                var list = new List<ConfigFileBaseInfo>();
                var files = GetFiles(filePath);
                foreach (var file in files)
                {
                    if (!CheckFileExtension(Path.GetExtension(file)))
                    {
                        continue;
                    }

                    var model = new ConfigFileBaseInfo
                    {
                        FileName = GetFileName(file),
                        LastModifyTime = GetLastWriteTime(file),
                        Path = file,
                    };
                    list.Add(model);
                }

                return list;
            }
            catch (Exception ex)
            {
                return Empty<ConfigFileBaseInfo>();
            }
        }





    }
}
