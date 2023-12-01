using System;

namespace Schedule
{
    public static class JobLogHelper
    {

        public static void Debug(string message, string actionName = "", long millisecond = 0)
        {
            LogHelper.Debug($"{JobContext.CurrentJobBaseInfo}: {message}", url: FixActionName(actionName), millisecond: millisecond);
        }
        public static void Info(string message, string actionName = "", long millisecond = 0)
        {
            LogHelper.Info($"{JobContext.CurrentJobBaseInfo}: {message}", url: FixActionName(actionName), millisecond: millisecond);
        }

        public static void Warn(string message, Exception ex = null, string actionName = "", long millisecond = 0)
        {
            LogHelper.Warn($"{JobContext.CurrentJobBaseInfo}: {message}", ex, url: FixActionName(actionName), millisecond: millisecond);
        }

        public static void Error(string message, Exception ex, string actionName = "", long millisecond = 0)
        {
            LogHelper.Error($"{JobContext.CurrentJobBaseInfo}: {message}", ex, url: FixActionName(actionName), millisecond: millisecond);
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
