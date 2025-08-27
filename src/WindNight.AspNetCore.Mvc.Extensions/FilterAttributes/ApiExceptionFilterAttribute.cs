using Microsoft.AspNetCore.Mvc.WnExtensions.@internal;
using Newtonsoft.Json.Extension;
using WindNight.Core;

namespace Microsoft.AspNetCore.Mvc.Filters.Extensions
{
    /// <summary>
    ///     Represents a filter to handle Api exception.
    /// </summary>
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public ApiExceptionFilterAttribute()
        {
            Order = 0;
        }

        public virtual int ExecOrder { get; set; } = 0;

        #region override ExceptionFilterAttribute

        public override void OnException(ExceptionContext context)
        {
            //TODO 日志等级分级

            var errMsg = string.Empty;
            try
            {
                var exception = context.Exception;

                context.HttpContext.Response.StatusCode = 200;
                ;
                if (context.Exception is BusinessException ex)
                {
                    context.Result =
                        new ObjectResult(new ResponseResult<object>().BadRequest(ex.BusinessCode, ex.Message));
                    errMsg = ex.Message;
                }
                else
                {
                    context.Result = new ObjectResult(new ResponseResult<object>().SystemError(exception.Message));
                    errMsg = exception.ToJsonStr();
                }

                LogHelper.Warn(errMsg);
            }
            catch (Exception ex)
            {
                LogHelper.Error("ApiExceptionFilterAttribute-系统错误", ex);
                context.Result = new ObjectResult(new ResponseResult<object>().SystemError("系统错误"));

                base.OnException(context);
            }
        }

        #endregion
    }
}
