using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Config.Abstractions;
using WindNight.Core;

namespace WindNight.Config.Extensions.Attributes
{
    internal class CenterApiAuthAttribute : ActionFilterAttribute
    {

        public CenterApiAuthAttribute()
        {
            Order = 0;
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {

            var isValid = Ioc.GetService<IConfigCenterAuth>()?.ConfigCenterApiAuth() ?? false;
            if (!isValid)
            {
                context.Result = new ObjectResult(ResponseResult.GenNotFoundRes(null));

                return;

            }

            base.OnActionExecuting(context);


        }






    }
}
