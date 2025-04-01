using System;
using System.Linq;
using WindNight.AspNetCore.Mvc.Extensions;
using WindNight.Core;
using WindNight.Core.Attributes.Abstractions;

namespace Microsoft.AspNetCore.Mvc.Filters.Extensions
{
    /// <summary>
    ///     Provides Api result transform feature base http request header.
    /// </summary>
    public class ApiResultFilterAttribute : ResultFilterAttribute
    {
        public ApiResultFilterAttribute()
        {
            Order = 10;
        }

        public virtual int ExecOrder { get; set; } = 0;

        protected virtual void FixResultBeforeResultExecuting(ResultExecutingContext context)
        {

            //var car = context.ActionDescriptor.GetMethodAttributes<ClearResultAttribute>().FirstOrDefault();

            var clsAttr = context.ActionDescriptor.GetAttributeOnAction<ClearResultAttribute>();

            var noClear = clsAttr?.IsClear ?? false;
            if (noClear) return;
            if (!(context.Result is ObjectResult objectResult) || objectResult.Value is ResponseResult)
            {
                return;
            }
            if (objectResult.Value is null)
            {
                context.Result = new ObjectResult(new ResponseResult<object>().NotFound());
            }
            else
            {
                try
                {
                    var apiResult = Activator.CreateInstance(
                        typeof(ResponseResult<>).MakeGenericType(objectResult.DeclaredType), objectResult.Value);
                    context.Result = new ObjectResult(apiResult);
                }
                catch (Exception ex)
                {
                    context.Result = new ObjectResult(new ResponseResult<object>().SystemError(ex.Message));
                }

            }
        }

        #region override ResultFilterAttribute

        /// <summary>
        ///     Called when [result executing].
        /// </summary>
        /// <param name="context">The context.</param>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            FixResultBeforeResultExecuting(context);
            base.OnResultExecuting(context);
        }

        #endregion
    }
}
