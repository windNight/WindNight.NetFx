using WindNight.Core;
using WindNight.Core.Abstractions;
using WindNight.Core.ConfigCenter.Extensions;

namespace Microsoft.AspNetCore.WindNight.Hosting.@internal
{
    internal class ConfigItems : DefaultConfigItemBase
    {
        static string SvrCenterDomainConfigKey = ConstantKeys.SvrCenterDomainConfigKey;


        public static string SvrCenterDomain => QueryDomainConfig(SvrCenterDomainConfigKey);



        public static DomainConfigDto SvrCenterConfig => QueryDomainInfoConfig(SvrCenterDomainConfigKey) ?? new DomainConfigDto();

        public static bool SvrMonitorOpen => SvrCenterConfig.GetValueInExtension("SvrMonitorOpen", true);



    }






}
