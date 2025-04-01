
using WindNight.Core.ConfigCenter.Extensions;
using WindNight.Extension.Logger.DbLog.Abstractions;

namespace WindNight.Extension.Logger.DbLog.@internal
{
    internal class ConfigItems : DefaultConfigItemBase
    {

        //  private static IConfiguration configuration => Ioc.GetService<IConfiguration>();



        public static DbLogOptions DbLogOptions => GetSectionValue<DbLogOptions>();

        /// <summary> 是否输出日志 </summary>
        public static bool IsConsoleLog => DbLogOptions?.IsConsoleLog ?? false;
        public static bool IsOpenDebug => DbLogOptions?.IsOpenDebug ?? false;
        public static string DbConnectString => DbLogOptions?.DbConnectString ?? string.Empty;




    }
}
