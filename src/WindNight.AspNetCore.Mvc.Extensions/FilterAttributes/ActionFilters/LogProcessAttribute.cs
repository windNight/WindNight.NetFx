﻿using System;
using System.Text.Extension;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.WnExtensions;
using Microsoft.AspNetCore.Mvc.WnExtensions.@internal;
using Newtonsoft.Json.Extension;
using Newtonsoft.Json.Linq;
using WindNight.Core.Abstractions;
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
                if (header.Key.ToLower().Contains(ACCESSTOKENKEY))
                    CurrentItem.AddItem(WebConst.ACCESSTOKEN, header.Value.ToString());
                else
                    CurrentItem.AddItem(header.Key.ToLower(), header.Value.ToString());
        }

        protected virtual void AppendActionArguments(ActionExecutingContext context)
        {
            if (context.ActionArguments != null)
            {
                foreach (var argument in context.ActionArguments)
                {
                    try
                    { 
                        if (argument.Value == null || argument.Value.GetType().IsValueType || argument.Value is string)
                        {
                            CurrentItem.AddItem($"{argument.Key.ToLower()}", argument.Value?.ToString());
                        }
                        else if (argument.Value is System.Collections.IList)
                        {
                            //if (argument.Value.GetType().IsGenericType && argument.Value.GetType().GetGenericTypeDefinition() == typeof(List<>))
                            //{

                            //}
                            CurrentItem.AddItem("list", argument.Value.ToJsonStr());

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
                CurrentItem.AddItem(WebConst.RESPONSE, result);
                var request = context.HttpContext.Request;
                DateTime beginTime = CurrentItem.GetItem<DateTime>(WebConst.BEGINTIME);
                var ms = (long)(HardInfo.Now - beginTime).TotalMilliseconds;

                if (ms > ConfigItems.ApiWarningMis)
                    LogHelper.Warn($"请求共耗时:{ms} ms ", millisecond: ms);
                else if (ConfigItems.LogProcessOpened)
                    LogHelper.Info($"请求共耗时:{ms} ms ", millisecond: ms);

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
            CurrentItem.AddItem(WebConst.BEGINTIME, HardInfo.Now);
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