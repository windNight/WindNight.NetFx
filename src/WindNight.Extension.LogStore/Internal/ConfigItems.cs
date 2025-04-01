
#if !NET45
using System.Reflection.Emit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Core.Abstractions;
using WindNight.Core.ConfigCenter.Extensions;
using WindNight.Extension.Logger.DcLog.Abstractions;

namespace WindNight.Extension.Logger.DcLog.@internal
{
    internal class ConfigItems : DefaultConfigItemBase
    {

        //  private static IConfiguration configuration => Ioc.GetService<IConfiguration>();



        public static DcLogOptions DcLogOptions => GetSectionValue<DcLogOptions>();

        /// <summary> 是否输出日志 </summary>
        public static bool IsConsoleLog => DcLogOptions?.IsConsoleLog ?? false;
        public static bool IsOpenDebug => DcLogOptions?.IsOpenDebug ?? false;



    }
}
#endif
