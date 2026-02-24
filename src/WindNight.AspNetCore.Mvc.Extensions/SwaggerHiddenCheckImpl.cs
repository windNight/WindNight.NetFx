using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.HideApi;
using Swashbuckle.AspNetCore.HideApi.Abstractions;
using WindNight.Core.Attributes.Abstractions;

namespace WindNight.AspNetCore.Mvc.Extensions
{
    [Alias("default")]
    internal class SwaggerHiddenCheckImpl : ISwaggerHiddenCheck
    {
        public bool HiddenApi(ApiDescription apiDescription)
        {
            return apiDescription.HiddenApiDefaultImpl();
        }


    }
}
