using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Extension;
using WindNight.Core.Abstractions;
using WindNight.Core.ExceptionExt;

namespace WindNight.LogExtension
{
    public static partial class LogHelper
    {
        private static Version _version => new AssemblyName(typeof(LogHelper).Assembly.FullName).Version;
        private static DateTime _compileTime => File.GetLastWriteTime(typeof(LogHelper).Assembly.Location);

        public static string CurrentVersion => _version.ToString();

        public static DateTime CurrentCompileTime => _compileTime;
        static bool OpenDebug => ConfigItems.OpenDebug;

        static LogLevels MiniLogLevel => ConfigItems.GlobalMiniLogLevel;

        static bool CanLog(LogLevels level)
        {

            if (level == LogLevels.None)
            {
                return false;
            }

            if (level == LogLevels.Debug && !OpenDebug)
            {
                return false;
            }

            if (level < MiniLogLevel)
            {
                return false;
            }

            return true;
        }

    }
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
            if (logInfo.Level == LogLevels.Report)
            {
                return;
            }
            if (ConfigItems.LogOnConsole)
            {
                var logLevel = logInfo.Level;
                var exception = logInfo.Exceptions;
                Console.ForegroundColor = ConsoleColor.Green;
                if (logLevel > LogLevels.Information)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                if (exception != null)
                {
                    logInfo.Content = $"{logInfo.Content} {Environment.NewLine} {exception.GetMessage()}";
                }
                Console.WriteLine($"LogConsole =={HardInfo.NowString}==ConsoleLog:{Environment.NewLine}{logInfo}");
                Console.ResetColor();
            }
        }
    }
}
