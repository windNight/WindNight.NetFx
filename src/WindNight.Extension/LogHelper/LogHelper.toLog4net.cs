using System;
using WindNight.Core.Abstractions;

namespace WindNight.LogExtension
{
    public static partial class LogHelper
    {
        static void Log4NetPublish(LogInfo logInfo)
        {
            if (logInfo == null) return;
            Log(logInfo.Level, logInfo.Content, logInfo.Exceptions);
        }

        private static void Log(LogLevels level, string message, Exception logException = null)
        {
            try
            {
                switch (level)
                {
                    case LogLevels.Information:
                    case LogLevels.SysRegister:
                    case LogLevels.SysOffline:
                    case LogLevels.Report:
                        DefaultLog.Info(message, logException);
                        break;

                    case LogLevels.Warning:
                        DefaultLog.Warn(message, logException);
                        break;

                    case LogLevels.Error:
                        DefaultLog.Error(message, logException);
                        break;

                    case LogLevels.Critical:
                        DefaultLog.Fatal(message, logException);
                        break;
                    case LogLevels.ApiUrl:
                    case LogLevels.ApiUrlException:
                    case LogLevels.Trace:
                    case LogLevels.Debug:
                    case LogLevels.None:
                    default:
                        DefaultLog.Debug(message, logException);
                        break;
                }
            }
            catch (Exception ex)
            {
                DoConsoleLog(LogLevels.Warning, $"log4net 【{level}】 {message} handler error {ex.Message}", ex);
            }
        }
    }
}