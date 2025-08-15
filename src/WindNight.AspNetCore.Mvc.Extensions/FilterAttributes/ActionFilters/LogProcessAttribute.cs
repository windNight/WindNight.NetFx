using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.WnExtensions;
using Microsoft.AspNetCore.Mvc.WnExtensions.@internal;
using Newtonsoft.Json.Extension;
using Newtonsoft.Json.Linq;
using WindNight.AspNetCore.Mvc.Extensions;
using WindNight.Core;
using WindNight.Core.Extension;
using WindNight.Extension;

namespace Microsoft.AspNetCore.Mvc.Filters.Extensions
{
    public class LogProcessAttribute : ActionFilterAttribute
    {
        protected const string ACCESSTOKENKEY = "accesstoken";

        public LogProcessAttribute()
        {
            Order = -9999;
        }

        // public int ExecOrder { get; set; } = -99999;

        protected virtual void AppendHeaderInfo(HttpContext context)
        {
            foreach (var header in context.Request.Headers)
            {
                if (header.Key.ToLower().Contains(ACCESSTOKENKEY))
                {
                    CurrentItem.AddItem(WebConst.ACCESSTOKEN, header.Value.ToString());
                }
                else
                {
                    CurrentItem.AddItem(header.Key.ToLower(), header.Value.ToString());
                }
            }
        }


        string QueryTraceIdFromActionArguments(ActionExecutingContext context)
        {

            try
            {

                if (context.ActionArguments == null)
                {
                    return string.Empty;
                }

                var key = ConstantKeys.ReqTraceIdKey;
                var argument = context.ActionArguments.FirstOrDefault();
                if (argument.Value != null)
                {
                    if (argument.Value == null || argument.Value.GetType().IsValueType || argument.Value is string)
                    {
                        if (argument.Key.ToLower()
                            .Equals(ConstantKeys.ReqTraceIdKey, StringComparison.OrdinalIgnoreCase))
                        {
                            return argument.Value?.ToString() ?? "";
                        }
                    }
                    else if (argument.Value is IList)
                    {
                        //  CurrentItem.AddItem("list", argument.Value.ToJsonStr());
                    }
                    else
                    {
                        var input = JObject.FromObject(argument.Value);
                        var traceId = "";

                        if (input.ContainsKey(key))
                        {
                            traceId = input[key]?.ToString() ?? string.Empty;

                            return traceId;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error($"QueryTraceIdFromActionArguments Handler Error", ex);
            }
            return string.Empty;


        }


        protected virtual void AppendActionArguments(ActionExecutingContext context)
        {
            if (context.ActionArguments != null)
            {
                foreach (var argument in context.ActionArguments)
                {
                    try
                    {
                        if (argument.Key.ToLower().Equals(ConstantKeys.ReqTraceIdKey, StringComparison.OrdinalIgnoreCase))
                        {
                            if (argument.Value is string)
                            {
                                CurrentItem.AddItem($"{argument.Key.ToLower()}", argument.Value?.ToString());
                            }
                        }
                        else
                        {

                            if (argument.Value == null || argument.Value.GetType().IsValueType || argument.Value is string)
                            {
                                CurrentItem.AddItem($"{argument.Key.ToLower()}", argument.Value?.ToString());
                            }
                            else if (argument.Value is IList)
                            {
                                CurrentItem.AddItem("list", argument.Value.ToJsonStr());
                            }
                            else
                            {
                                var input = JObject.FromObject(argument.Value);
                                if (input == null)
                                {
                                    continue;
                                }
                                foreach (var each in input)
                                {
                                    if (each.Key.ToLower().Contains(ACCESSTOKENKEY))
                                    {
                                        CurrentItem.AddItem(WebConst.ACCESSTOKEN, each.Value.ToString());
                                    }
                                    else
                                    {
                                        CurrentItem.AddItem($"object:{each.Key.ToLower()}", each.Value.ToString());
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }


        protected virtual void DoBeforeActionExecuted(ActionExecutedContext context)
        {
            try
            {
                CurrentItem.AddItem(WebConst.ENDTIME, HardInfo.Now);
                var result = context.Result;
                if (result.ToJsonStr().Length < 1000)
                {
                    CurrentItem.AddItem(WebConst.RESPONSE, result);
                }
                var request = context.HttpContext.Request;

                var beginTime = CurrentItem.GetItem<DateTime>(WebConst.BEGINTIME);

                var ms = (long)(HardInfo.Now - beginTime).TotalMilliseconds;
                var isWarn = ms > ConfigItems.ApiWarningMis;
                if (isWarn)
                {
                    LogHelper.Warn($"请求共耗时:{ms} ms ", millisecond: ms);
                }
                else if (ConfigItems.LogProcessOpened)
                {
                    LogHelper.Info($"请求共耗时:{ms} ms ", ms);
                }

                if (ConfigItems.ApiUrlOpened)
                {
                    LogHelper.ApiUrlCall(request.Path, $"请求耗时{ms} {request.Path}", ms, appendMessage: isWarn);
                }

            }
            catch
            {
            }
        }


        protected virtual void AppendCommonContext(ActionExecutingContext context)
        {

            CurrentItem.AddItem(WebConst.HEARDER, context.HttpContext.Request.Headers);
            CurrentItem.AddItem(WebConst.BEGINTIME, HardInfo.Now);
            //   var traceId = CurrentItem.GetSerialNumber;
            //if (!CurrentItem.Items.ContainsKey(WebConst.SERIZLNUMBER))
            //{
            //    CurrentItem.AddItem(WebConst.SERIZLNUMBER, $"{GuidHelper.GenerateOrderNumber()}");
            //}

            CurrentItem.AddItem(WebConst.REQUESTPATH, context.HttpContext?.Request?.Path);
            CurrentItem.AddItem(WebConst.CLIENTIP, context.HttpContext.GetClientIp());
            CurrentItem.AddItem(WebConst.SERVERIP, context.HttpContext.GetServerIp());
            CurrentItem.AddItem(WebConst.APPCODE, ConfigItems.SystemAppCode);
        }

        #region override ActionFilterAttribute

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            DoBeforeActionExecuted(context);

            base.OnActionExecuted(context);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var reqTraceId = context.HttpContext.Request.GetReqTraceIdValue();
            if (reqTraceId.IsNullOrEmpty())
            {
                reqTraceId = QueryTraceIdFromActionArguments(context);
            }

            var _ = CurrentItem.AddSerialNumber(reqTraceId, !reqTraceId.IsNullOrEmpty());

            AppendCommonContext(context);
            AppendHeaderInfo(context.HttpContext);
            AppendActionArguments(context);


            base.OnActionExecuting(context);
        }

        #endregion
    }
}
