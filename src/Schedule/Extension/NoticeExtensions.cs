using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Quartz;
using Schedule.Abstractions;
using Schedule.Func;

namespace Schedule.Extension
{
    public static class NoticeExtensions
    {
        static IScheduleNotice _scheduleNotice => Ioc.GetService<IScheduleNotice>();

        public static async Task DoNoticeAsync(this IJobExecutionContext context, string message, string extendInfo = "")
        {
            var jobInfo = context.GetJobBaseInfo();
            if (_scheduleNotice != null)
            {
                var msgSb = new StringBuilder(message);
                if (!extendInfo.IsNullOrEmpty())
                {
                    msgSb.AppendLine($"\n\n {extendInfo}");
                }
                await _scheduleNotice.DoNoticeAsync(jobInfo, msgSb.ToString());
                //  await _scheduleNotice.DoNoticeAsync(jobInfo, $"{message}{(extendInfo.IsNullOrEmpty() ? "" : $"\n\n{extendInfo}")}");
            }

        }


    }
}
