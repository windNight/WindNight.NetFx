using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Swashbuckle.AspNetCore.HideApi.Abstractions
{
    public interface ISwaggerHiddenCheck
    {
        bool HiddenApi(ApiDescription apiDescription);
    }
}
