using WindNight.ConfigCenter.Extension;

namespace WindNight.Config.@internal
{
    internal static class FileExtExtension
    {
        public static ConfigType ParserConfigType(this string fileName)
        {
            var fileExt = Path.GetExtension(fileName);
            return fileExt switch
            {
                ".json" => ConfigType.JsonConfig,
                ".xml" or ".config" => ConfigType.XmlConfig,
                var _ => ConfigType.Unknown,
            };
        }
    }
}
