using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WindNight.ConfigCenter.Extension;

namespace WindNight.Config.@internal
{
    internal static class Ex
    {

        public static ConfigType ParserConfigType(this string fileName)
        {
            var fileExt = Path.GetExtension(fileName);
            return fileExt switch
            {
                ".json" => ConfigType.JsonConfig,
                ".xml" => ConfigType.XmlConfig,
                ".config" => ConfigType.XmlConfig,
                _ => ConfigType.Unknown
            };

        }

    }
}
