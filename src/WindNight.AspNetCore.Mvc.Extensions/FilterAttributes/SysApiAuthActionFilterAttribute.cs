using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Core;
using WindNight.Core.Abstractions;
using WindNight.Core.Extension;
using WindNight.Extension;

namespace WindNight.AspNetCore.Mvc.Extensions.FilterAttributes
{
    public class SysApiAuthActionFilterAttribute : ActionFilterAttribute
    {
        ISysApiAuthCheck SysApiAuthCheckImpl => Ioc.GetService<ISysApiAuthCheck>();

        public bool NonAuth { get; set; } = true;

        public SysApiAuthActionFilterAttribute()
        {
            Order = 0;
        }
        public SysApiAuthActionFilterAttribute(bool nonAuth)
        {
            Order = 0;
            NonAuth = nonAuth;
        }


        protected virtual bool SelfReqClientIpCheck(string ip)
        {

            if (ip.IsInternalIp())
            {
                return true;
            }
            return false;
        }

        protected virtual bool ReqClientIpCheck(ActionExecutingContext context)
        {
            var reqIp = context.HttpContext.GetClientIp();

            if (reqIp.IsNullOrEmpty())
            {
                return false;
            }

            if (SysApiAuthCheckImpl == null)
            {
                return SelfReqClientIpCheck(reqIp);

            }

            return SysApiAuthCheckImpl.ReqClientIpCheck(reqIp);


        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (NonAuth)
            {
                if (SysApiAuthCheckImpl != null)
                {
                    if (SysApiAuthCheckImpl.OpenSysApiAuthCheck)
                    {
                        var isValid = SysApiAuthCheckImpl.SysApiAuth();
                        if (!isValid)
                        {
                            context.Result = new ObjectResult(ResponseResult.GenNotFoundRes(null));
                            return;
                        }
                    }
                }

                var ipCheck = ReqClientIpCheck(context);
                if (!ipCheck)
                {
                    context.Result = new ObjectResult(ResponseResult.GenNotFoundRes(null));
                    return;
                }
            }


            base.OnActionExecuting(context);


        }




    }
}
