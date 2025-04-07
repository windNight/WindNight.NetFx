using WindNight.Core.ConfigCenter.Extensions;
using WindNight.Extension.Logger.DbLog.Abstractions;

namespace RepoDemos.Internal
{
    internal class ConfigItems : DefaultConfigItemBase
    {

        public static string DefaultDBConnectString => GetConnectionString("db1");


        public static DbLogOptions DbLogOptions => GetSectionValue<DbLogOptions>();

        /// <summary> 是否输出日志 </summary>
        public static bool IsConsoleLog => DbLogOptions?.IsConsoleLog ?? false;
        public static bool IsOpenDebug => DbLogOptions?.IsOpenDebug ?? false;
        public static string DbConnectString => DbLogOptions?.DbConnectString ?? string.Empty;



    }
}

