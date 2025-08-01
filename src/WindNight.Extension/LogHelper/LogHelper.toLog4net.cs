using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using log4net;
using log4net.Config;
using log4net.Core;
using Newtonsoft.Json.Extension;
using WindNight.Core.Abstractions;

namespace WindNight.LogExtension
{
    public static partial class LogHelper
    {
        private static readonly object lockHelper = new object();
        private static ILog? _log;


        private static ILog? DefaultLog
        {
            get
            {
                if (_log == null)
                {
                    lock (lockHelper)
                    {
                        if (_log == null)
                        {
                            Init();
                        }
                    }
                }

                return _log;
            }
        }

        private static void InitLog4Net()
        {
            try
            {
                if (ConfigItems.Log4netOpen)
                {
                    CheckLog4netConfigPath();
                    var fileInfo = new FileInfo(Log4NetConfigPath);
                    if (!fileInfo.Exists)
                    {
                        CreateConfigFile(fileInfo);
                    }
                    var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
                    XmlConfigurator.ConfigureAndWatch(logRepository, fileInfo);
                    _log = LogManager.GetLogger(Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly(), "DefalutLogger");
                    RegisterProcessEvent(Log4NetSubscribe);
                    //RegisterProcessEvent(Log4NetPublish);
                }
            }
            catch (Exception ex)
            {
                DoConsoleLog(LogLevels.Warning, $"LogHelper Init handler error {ex.Message}", ex);
            }
        }

        private static string Log4NetConfigDir => Path.GetDirectoryName(Log4NetConfigPath) ?? "";

        private static string CurrentAppDir => Environment.CurrentDirectory;

        /// <summary>
        ///  需要区分windows 和linux 
        /// </summary>
        private static string Log4NetConfigPath => Path.Combine(
            // AppDomain.CurrentDomain.BaseDirectory,
            CurrentAppDir,
            HardInfo.IsWindows ?
                ConfigItems.Log4netConfigPath.TrimStart('/').Replace("/", "\\")
                :
                ConfigItems.Log4netConfigPath);

        public static string CurrentLog4NetConfigPath
        {
            get
            {
                try
                {
                    return Log4NetConfigPath;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                return "";
            }
        }

        static void CheckLog4netConfigPath()
        {
            try
            {
                if (!File.Exists(Log4NetConfigPath))
                {
                    DoConsoleLog(LogLevels.Warning, $" LogHelper Log4netConfigPath ({Log4NetConfigPath}) not exist use default config  ");
                    if (!Directory.Exists(Log4NetConfigDir))
                    {
                        Directory.CreateDirectory(Log4NetConfigDir);
                    }

                    if (File.Exists(Log4NetConfigPath))
                    {
                        return;
                    }
                    using (var streamWriter = new StreamWriter(Log4NetConfigPath, false))
                    {
                        try
                        {
                            streamWriter.Write(DefaultConfigContent);
                        }
                        catch
                        {

                        }
                        finally
                        {
                            streamWriter.Close();
                        }
                    }

                }
            }
            catch// (Exception ex)
            {

            }
        }
        private const string DefaultConfigContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>

<configuration>
	<configSections>
		<section name=""log4net"" type=""log4net.Config.Log4NetConfigurationSectionHandler, log4net"" />
	</configSections>

	<log4net>
		<root>
			<level value=""All"" />
			<appender-ref ref=""ConsoleAppender"" />
			<appender-ref ref=""AllFileAppender"" />
			<appender-ref ref=""InfoFileAppender"" />
			<appender-ref ref=""ErrorFileAppender"" />
			<appender-ref ref=""WarnFileAppender"" />
		</root>

		<appender name=""AllFileAppender"" type=""log4net.Appender.RollingFileAppender"">
			<param name=""File"" value=""Logs/all.log_"" />
			<param name=""AppendToFile"" value=""true""/>
			<param name=""RollingStyle"" value=""Composite""/>
			<param name=""DatePattern"" value=""yyyy-MM-dd&quot;.log&quot;""/>
			<param name=""MaximumFileSize"" value=""50MB""/>
			<param name=""MaxSizeRollBackups"" value=""10""/>
			<param name=""StaticLogFileName"" value=""false""/>
			<lockingModel type=""log4net.Appender.FileAppender+MinimalLock"" />
			<layout type=""log4net.Layout.PatternLayout"">
				<param name=""ConversionPattern"" value=""[时间]:%d%n[%thread] %-5[级别]:%p%n[内容]:%m%n%n"" />
			</layout>
			<filter type='log4net.Filter.LevelRangeFilter'>
				<param name='LevelMin' value='DEBUG' />
				<param name='LevelMax' value='FATAL' />
			</filter>
		</appender>

		<appender name=""InfoFileAppender"" type=""log4net.Appender.RollingFileAppender"">
			<param name=""File"" value=""Logs/info.log_"" />
			<param name=""AppendToFile"" value=""true""/>
			<param name=""RollingStyle"" value=""Composite""/>
			<param name=""DatePattern"" value=""yyyy-MM-dd&quot;.log&quot;""/>
			<param name=""MaximumFileSize"" value=""50MB""/>
			<param name=""MaxSizeRollBackups"" value=""10""/>
			<param name=""StaticLogFileName"" value=""false""/>
			<lockingModel type=""log4net.Appender.FileAppender+MinimalLock"" />
			<layout type=""log4net.Layout.PatternLayout"">
				<param name=""ConversionPattern"" value=""[时间]:%d%n[%thread] %-5[级别]:%p%n[内容]:%m%n%n"" />
			</layout>
			<filter type='log4net.Filter.LevelRangeFilter'>
				<param name='LevelMin' value='INFO' />
				<param name='LevelMax' value='INFO' />
			</filter>
		</appender>

		<appender name=""ErrorFileAppender"" type=""log4net.Appender.RollingFileAppender"">
			<filter type='log4net.Filter.LevelRangeFilter'>
				<param name='LevelMin' value='ERROR' />
				<param name='LevelMax' value='FATAL' />
			</filter>
			<param name=""File"" value=""Logs/error.log_"" />
			<param name=""AppendToFile"" value=""true"" />
			<param name=""RollingStyle"" value=""Composite"" />
			<param name=""DatePattern"" value=""yyyy-MM-dd&quot;.log&quot;"" />
			<param name=""MaximumFileSize"" value=""10MB"" />
			<param name=""MaxSizeRollBackups"" value=""10"" />
			<param name=""StaticLogFileName"" value=""false"" />
			<lockingModel type=""log4net.Appender.FileAppender+MinimalLock"" />
			<layout type=""log4net.Layout.PatternLayout"">
				<param name=""ConversionPattern"" value=""[时间]:%d%n[%thread] %-5[级别]:%p%n[内容]:%m%n%n"" />
			</layout>
		</appender>

		<appender name=""WarnFileAppender"" type=""log4net.Appender.RollingFileAppender"">
			<filter type='log4net.Filter.LevelRangeFilter'>
				<param name='LevelMin' value='WARN' />
				<param name='LevelMax' value='WARN' />
			</filter>
			<param name=""File"" value=""Logs/warn.log_"" />
			<param name=""AppendToFile"" value=""true"" />
			<param name=""RollingStyle"" value=""Composite"" />
			<param name=""DatePattern"" value=""yyyy-MM-dd&quot;.log&quot;"" />
			<param name=""MaximumFileSize"" value=""10MB"" />
			<param name=""MaxSizeRollBackups"" value=""10"" />
			<param name=""StaticLogFileName"" value=""false"" />
			<lockingModel type=""log4net.Appender.FileAppender+MinimalLock"" />
			<layout type=""log4net.Layout.PatternLayout"">
				<param name=""ConversionPattern"" value=""[时间]:%d%n[%thread] %-5[级别]:%p%n[内容]:%m%n%n"" />
			</layout>
		</appender>

		<appender name=""ConsoleAppender"" type=""log4net.Appender.ConsoleAppender"">
			<layout type=""log4net.Layout.PatternLayout"">
				<conversionPattern value=""%date [%thread] %-5level %logger - %message%newline"" />
			</layout>
		</appender>



	</log4net>
</configuration>";


        private static void CreateConfigFile(FileInfo fileInfo)
        {
            using (var text = File.CreateText(fileInfo.FullName))
            {
                text.Write(DefaultConfigContent);
                text.Close();
            }
        }

        static void Log4NetPublish(LogInfo? logInfo)
        {
            if (logInfo == null)
            {
                return;
            }
            Log4(logInfo.Level, logInfo.Content, logInfo.Exceptions);
        }


        public static void Log4NetSubscribe(LogInfo? logInfo)
        {
            try
            {
                if (logInfo.Level == LogLevels.Report)
                {
                    return;
                }
                Log4(logInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Log4NetSubscribe:上报日志异常:{0}", ex.ToJsonStr());
                RecordLog.WriteLog($"Log4NetSubscribe:上报日志异常:{ex.ToJsonStr()}");
            }
        }


        public static void Log4(LogLevels level, string message, Exception? logException = null)
        {
            Log4Internal(level, message, logException);

        }


        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="logException"></param> 
        public static void Log4Debug(string msg, Exception? logException = null)
        {
            Log4(LogLevels.Debug, msg, logException);
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="logException"></param> 
        public static void Log4Info(string msg, Exception? logException = null)
        {
            Log4(LogLevels.Information, msg, logException);
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="logException"></param> 
        public static void Log4Warn(string msg, Exception? logException = null)
        {
            Log4(LogLevels.Warning, msg, logException);

        }

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="logException"></param> 
        public static void Log4Error(string msg, Exception? logException)
        {
            Log4(LogLevels.Error, msg, logException);


        }

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="logException"></param> 
        public static void Log4Fatal(string msg, Exception? logException)
        {
            Log4(LogLevels.Critical, msg, logException);
        }

        public static void Log4(LogInfo? logInfo)
        {
            if (logInfo == null)
            {
                return;
            }
            Log4Internal(logInfo.Level, logInfo.Content, logInfo.Exceptions);

        }


        private static void Log4Internal(LogLevels level, string? message, Exception? logException = null)
        {
            try
            {
                if (DefaultLog == null)
                {
                    return;
                }
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
