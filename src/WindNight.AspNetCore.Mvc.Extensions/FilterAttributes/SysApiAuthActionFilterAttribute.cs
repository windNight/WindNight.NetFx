using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Core.Abstractions;
using WindNight.Core.Extension;
using WindNight.Extension;

namespace WindNight.AspNetCore.Mvc.Extensions.FilterAttributes
{
    public class SysApiAuthActionFilterAttribute : ActionFilterAttribute
    {
        public SysApiAuthActionFilterAttribute()
        {
            Order = 0;
        }

        public SysApiAuthActionFilterAttribute(bool nonAuth)
        {
            Order = 0;
            NonAuth = nonAuth;
        }

        private ISysApiAuthCheck SysApiAuthCheckImpl => Ioc.GetService<ISysApiAuthCheck>();

        public bool NonAuth { get; set; } = true;


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
                            context.HttpContext.Response.StatusCode = 404;
                            context.Result = new NotFoundResult(); //new ObjectResult(ResponseResult.GenNotFoundRes(null));
                                                                   //  context.Result = new NotFoundObjectResult(null); //new ObjectResult(ResponseResult.GenNotFoundRes(null));
                            return;
                        }
                    }
                }
                else
                {
                    var ipCheck = ReqClientIpCheck(context);
                    if (!ipCheck)
                    {
                        context.HttpContext.Response.StatusCode = 404;
                        context.Result = new NotFoundResult(); // new ObjectResult(ResponseResult.GenNotFoundRes(null));
                                                               // context.Result = new NotFoundObjectResult(null); //new ObjectResult(ResponseResult.GenNotFoundRes(null));
                        return;
                    }
                }
            }


            base.OnActionExecuting(context);
        }
    }
}
