using System;

namespace Schedule
{
    public static class JobLogHelper
    {

        public static void Debug(string message, string actionName = "")
        {
            LogHelper.Debug($"{JobContext.CurrentJobBaseInfo}: {message}", url: FixActionName(actionName));
        }
        public static void Info(string message, string actionName = "")
        {
            LogHelper.Info($"{JobContext.CurrentJobBaseInfo}: {message}", url: FixActionName(actionName));
        }

        public static void Warn(string message, Exception ex = null, string actionName = "")
        {
            LogHelper.Warn($"{JobContext.CurrentJobBaseInfo}: {message}", ex, url: FixActionName(actionName));
        }

        public static void Error(string message, Exception ex, string actionName = "")
        {
            LogHelper.Error($"{JobContext.CurrentJobBaseInfo}: {message}", ex, url: FixActionName(actionName));
        }

        static string FixActionName(string actionName = "")
        {
            var jobCode = JobContext.JobCode;
            if (!jobCode.IsNullOrEmpty())
            {
                return $"{jobCode}:{actionName}";
            }

            return actionName;
        }

    }
}
