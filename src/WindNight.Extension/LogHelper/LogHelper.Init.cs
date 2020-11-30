using Newtonsoft.Json.Extension;
using System;
using WindNight.Core.Abstractions;

namespace WindNight.LogExtension
{
    public static partial class LogHelper
    {
        static LogHelper()
        {
            Init();
        }


        private static void Init()
        {
            try
            {
                RegisterProcessEvent(LogConsolePublish);
                InitLog4Net();
            }
            catch (Exception ex)
            {
                DoConsoleLog(LogLevels.Warning, $"LogHelper Init handler error {ex.Message}", ex);
            }
        }

        static void LogConsolePublish(LogInfo logInfo)
        {
            if (ConfigItems.LogOnConsole)
            {
                Console.WriteLine(logInfo.ToJsonStr());
            }
        }

    }
}