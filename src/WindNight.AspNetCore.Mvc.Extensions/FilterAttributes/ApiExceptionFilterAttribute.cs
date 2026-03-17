using Microsoft.AspNetCore.Mvc.WnExtensions.@internal;
using WindNight.Core;
using WindNight.Core.ExceptionExt;

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
                var bizError = false;
                if (context.Exception is BusinessException ex)
                {
                    context.Result =
                        new ObjectResult(new ResponseResult<object>().BadRequest(ex.BusinessCode, ex.Message));
                    errMsg = ex.Message;
                    bizError = true;
                }
                else
                {
                    context.Result = new ObjectResult(new ResponseResult<object>().SystemError("系统错误22"));
                    errMsg = exception.GetMessage();
                }

                if (!bizError)
                {
                    LogHelper.Warn($"{ConfigItems.AppInfo} 业务异常未捕获 reqApi[{context?.HttpContext?.Request?.Path ?? ""}] errMsg:{errMsg}");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error($" api[{context?.HttpContext?.Request?.Path ?? ""}] ApiExceptionFilterAttribute-系统错误 ,{ex.Message}", ex);
                context.Result = new ObjectResult(new ResponseResult<object>().SystemError("系统错误23"));

                base.OnException(context);
            }
        }

        #endregion
    }
}
