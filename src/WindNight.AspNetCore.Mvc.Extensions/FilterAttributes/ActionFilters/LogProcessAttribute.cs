using System;
using System.Text.Extension;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.WnExtensions;
using Microsoft.AspNetCore.Mvc.WnExtensions.Internals;
using Newtonsoft.Json.Linq;
using WindNight.Core.Abstractions;
using WindNight.Extension;

namespace Microsoft.AspNetCore.Mvc.Filters.Extensions
{
    public class LogProcessAttribute : ActionFilterAttribute
    {
        private const string ACCESSTOKENKEY = "accesstoken";

        public LogProcessAttribute()
        {
            Order = ExecOrder;
        }

        public int ExecOrder { get; set; } = -9999;

        protected virtual void AppendHeaderInfo(HttpContext context)
        {
            foreach (var header in context.Request.Headers)
                if (header.Key.ToLower().Contains(ACCESSTOKENKEY))
                    CurrentItem.AddItem(WebConst.ACCESSTOKEN, header.Value.ToString());
                else
                    CurrentItem.AddItem(header.Key.ToLower(), header.Value.ToString());
        }

        protected virtual void AppendActionArguments(ActionExecutingContext context)
        {
            if (context.ActionArguments != null)
                foreach (var argument in context.ActionArguments)
                    if (argument.Value == null || argument.Value.GetType().IsValueType ||  argument.Value is string)
                    {
                        CurrentItem.AddItem($"{argument.Key.ToLower()}", argument.Value?.ToString());
                    }
                    else
                    {
                        var input = JObject.FromObject(argument.Value);
                        if (input == null) continue;
                        foreach (var each in input)
                            if (each.Key.ToLower().Contains(ACCESSTOKENKEY))
                                CurrentItem.AddItem(WebConst.ACCESSTOKEN, each.Value.ToString());
                            else
                                CurrentItem.AddItem($"{each.Key.ToLower()}", each.Value.ToString());
                    }
        }


        protected virtual void DoBeforeActionExecuted(ActionExecutedContext context)
        {
            try
            {
                CurrentItem.AddItem(WebConst.ENDTIME, DateTime.Now);

                var result = context.Result;
                CurrentItem.AddItem(WebConst.RESPONSE, result);
                var request = context.HttpContext.Request;
                DateTime beginTime = CurrentItem.GetItem<DateTime>(WebConst.BEGINTIME);
                var ms = (long)(DateTime.Now - beginTime).TotalMilliseconds;

                if (ms > ConfigItems.ApiWarningMis)
                    LogHelper.Warn($"请求共耗时:{ms} ms ");
                else if (ConfigItems.LogProcessOpened)
                    LogHelper.Info($"请求共耗时:{ms} ms ");

                if (ConfigItems.ApiUrlOpened)
                    LogHelper.Add($"请求耗时{ms} {request.Path}", LogLevels.ApiUrl, url: request.Path, millisecond: ms,
                        appendMessage: true);
            }
            catch
            {
            }
        }

        protected virtual void AppendCommonContext(ActionExecutingContext context)
        {
            CurrentItem.AddItem(WebConst.HEARDER, context.HttpContext.Request.Headers);
            CurrentItem.AddItem(WebConst.BEGINTIME, DateTime.Now);
            if (!CurrentItem.Items.ContainsKey(WebConst.SERIZLNUMBER))
                CurrentItem.AddItem(WebConst.SERIZLNUMBER, $"{GuidHelper.GenerateOrderNumber()}");

            CurrentItem.AddItem(WebConst.REQUESTPATH, context.HttpContext?.Request?.Path);
            CurrentItem.AddItem(WebConst.CLIENTIP, context.HttpContext.GetClientIp());
            CurrentItem.AddItem(WebConst.SERVERIP, context.HttpContext.GetServerIp());
            CurrentItem.AddItem(WebConst.APPCODE, ConfigItems.SysAppCode);
        }

        #region override ActionFilterAttribute

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            DoBeforeActionExecuted(context);

            base.OnActionExecuted(context);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            AppendCommonContext(context);
            AppendHeaderInfo(context.HttpContext);
            AppendActionArguments(context);
            base.OnActionExecuting(context);
        }

        #endregion

    }

}