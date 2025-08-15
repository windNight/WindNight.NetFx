using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.WnExtensions.@internal;
using Newtonsoft.Json.Extension;
using WindNight.Core;

namespace Microsoft.AspNetCore.Mvc.Filters.Extensions
{
    public class ValidateInputAttribute : ActionFilterAttribute
    {
        public ValidateInputAttribute()
        {
            Order = -10;
        }

        public virtual int ExecOrder { get; set; } = 0;

        protected virtual void ValidateInput(ActionExecutingContext context)
        {
            if (!ConfigItems.IsValidateInput || context.ModelState.IsValid)
            {
                return;
            }

            var invalidValue = context.ModelState.Values
                    .Where(m => m.ValidationState == ModelValidationState.Invalid)
                    .Select(m => m.To<InvalidModel>())
                    .ToList()
                ;

            string message = "";
            if (!invalidValue.Any())
            {
                message = unknownParamError;
            }
            else
            {
                message = invalidValue.Where(m => m != null).Select(m => $"{m.Key}:{m.Errors.Select(p => p.ErrorMessage).Join(",")}").Join("|");

            }

            context.Result = new ObjectResult(new ResponseResult<object>().BadRequest(100420, message));
        }

        #region override ActionFilterAttribute

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ValidateInput(context);
            base.OnActionExecuting(context);
        }

        #endregion

        /// <summary> </summary>
        private class InvalidModel
        {
            public string Key { get; set; }
            public IList<SelfModelError> Errors { get; set; }
        }

        /// <summary> </summary>
        private class SelfModelError
        {
            public Exception Exception { get; set; }
            public string ErrorMessage { get; set; }
        }

        #region 常量

        private const string unknownParamError = "您输入的参数有误";
        private const string notSpecificErrorMessage = "您输入的参数{0}有误";

        #endregion 常量
    }
}
