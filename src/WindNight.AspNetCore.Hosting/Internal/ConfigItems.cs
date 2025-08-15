using WindNight.Core;
using WindNight.Core.ConfigCenter.Extensions;
using WindNight.Core.Extension;

namespace Microsoft.AspNetCore.WindNight.Hosting.@internal
{
    internal class ConfigItems : DefaultConfigItemBase
    {
        static string SvrCenterDomainConfigKey = ConstantKeys.SvrCenterDomainConfigKey;



        // public static DomainConfigs DomainConfigs => GetSectionValue<DomainConfigs>() ?? new DomainConfigs();

        public static string SvrCenterDomain => QueryDomainConfig(SvrCenterDomainConfigKey);

        //static string QueryDomainConfig(string name)
        //{
        //    var config = DomainConfigs.Items.FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        //    return config?.Domain ?? "";
        //}

        //static DomainConfigDto QueryDomainInfoConfig(string name)
        //{
        //    var config = DomainConfigs.Items.FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        //    return config;
        //}


        public static DomainConfigDto SvrCenterConfig => QueryDomainInfoConfig(SvrCenterDomainConfigKey) ?? new DomainConfigDto();

        public static bool SvrMonitorOpen => SvrCenterConfig.GetValueInExtension("SvrMonitorOpen", true);



    }






}
