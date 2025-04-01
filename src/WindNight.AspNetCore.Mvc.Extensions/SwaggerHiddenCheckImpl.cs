using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.HideApi;
using Swashbuckle.AspNetCore.HideApi.Abstractions;

namespace WindNight.AspNetCore.Mvc.Extensions
{
    internal class SwaggerHiddenCheckImpl : ISwaggerHiddenCheck
    {
        public bool HiddenApi(ApiDescription apiDescription)
        {
            return apiDescription.HiddenApiDefaultImpl();

            //var sysApiAttr = apiDescription.GetSysApiAttr();
            //if (sysApiAttr != null)
            //{
            //    if (!ConfigItems.ShowSysApi)
            //    {
            //        return true;
            //    }

            //    if (ConfigItems.ShowSysApiMiniLevel > 0 && sysApiAttr.SysApiLevel < ConfigItems.ShowSysApiMiniLevel)
            //    {
            //        return true;
            //    }
            //}
            //var debugApiAttr = apiDescription.GetDebugApiAttr();
            //if (debugApiAttr != null)
            //{
            //    if (!ConfigItems.ShowDebugApi)
            //    {
            //        return true;
            //    }
            //}
            //return !ConfigItems.ShowHiddenApi;
        }


    }
}
