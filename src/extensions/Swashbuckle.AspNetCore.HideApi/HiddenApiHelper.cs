using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.Extensions.@internal;

namespace Swashbuckle.AspNetCore.HideApi
{
    public static class HiddenApiHelper
    {
        public static bool HiddenApiDefaultImpl(this ApiDescription apiDescription)
        {
            try
            {
                if (apiDescription.RelativePath.Contains("svrinfo"))
                {

                }
                var sysApiAttr = apiDescription.GetSysApiAttr();
                if (sysApiAttr != null)
                {
                    if (!ConfigItems.ShowSysApi)
                    {
                        return true;
                    }

                    if (ConfigItems.ShowSysApiMiniLevel > 0)//&& sysApiAttr.SysApiLevel >= ConfigItems.ShowSysApiMiniLevel)
                    {
                        var sysApiAttrs = apiDescription.GetSysApiAttrs();
                        if (sysApiAttrs.Any(m => m.SysApiLevel >= ConfigItems.ShowSysApiMiniLevel))
                        {
                            return true;
                        }
                    }
                }
                var debugApiAttr = apiDescription.GetDebugApiAttr();
                if (debugApiAttr != null)
                {
                    if (!ConfigItems.ShowDebugApi)
                    {
                        return true;
                    }
                }

                var hiddenApiAttr = apiDescription.GetHiddenApiAttr();
                if (hiddenApiAttr != null)
                {

                    if (hiddenApiAttr.SysApi)
                    {
                        if (!ConfigItems.ShowSysApi)
                        {
                            return true;
                        }
                    }

                    if (hiddenApiAttr.DebugApi)
                    {
                        if (!ConfigItems.ShowDebugApi)
                        {
                            return true;
                        }
                    }

                    return !ConfigItems.ShowHiddenApi;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
