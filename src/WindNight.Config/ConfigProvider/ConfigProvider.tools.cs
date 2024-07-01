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


        public IEnumerable<string> FetchSelfConfigNames(string fileDir)
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, FixDirPath(fileDir));
            if (!Directory.Exists(filePath))
            {
#if NET45LATER
                return Array.Empty<string>();
#else
                return new List<string>();
#endif
            }

            var configFiles = Directory.GetFiles(filePath).Where(m => CheckFileExtension(Path.GetExtension(m)));
            var configFileNames = configFiles.Select(Path.GetFileName).ToList();
            return configFileNames;

        }

        public IEnumerable<string> FetchConfigNames()
        {
            var filePath = ConfigPath;
            if (!Directory.Exists(filePath))
            {

#if NET45LATER
                return Array.Empty<string>();
#else
                return new List<string>();
#endif

            }
            var configFiles = Directory.GetFiles(filePath).Where(m => CheckFileExtension(Path.GetExtension(m)));
            var configFileNames = configFiles.Select(Path.GetFileName).ToList();
            return configFileNames;

        }


    }
}
