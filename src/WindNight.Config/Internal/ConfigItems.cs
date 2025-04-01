
using System;
using WindNight.Core.Abstractions;
using WindNight.Core.ConfigCenter.Extensions;

namespace WindNight.Config.@internal
{
    internal class ConfigItems : DefaultConfigItemBase
    {
        public static bool OpenConfigLogs =>
            GetAppSettingValue(nameof(OpenConfigLogs), false);

    }
}
