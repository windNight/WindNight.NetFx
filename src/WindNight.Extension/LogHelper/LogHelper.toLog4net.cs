using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using WindNight.Core.Abstractions;

namespace WindNight.LogExtension
{
    public static partial class LogHelper
    {
        private static readonly object lockHelper = new object();
        private static ILog _log;


        private static ILog DefaultLog
        {
            get
            {
                if (_log == null)
                    lock (lockHelper)
                    {
                        if (_log == null)
                            Init();
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
                    var fileInfo = new FileInfo(Log4netConfigPath);
                    if (!fileInfo.Exists)
                        CreateConfigFile(fileInfo);
                    var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
                    XmlConfigurator.ConfigureAndWatch(logRepository, fileInfo);
                    _log = LogManager.GetLogger(Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly(), "DefalutLogger");
                    RegisterProcessEvent(Log4NetPublish);
                }
            }
            catch (Exception ex)
            {
                DoConsoleLog(LogLevels.Warning, $"LogHelper Init handler error {ex.Message}", ex);
            }
        }

        private static string Log4netConfigDir => Path.GetDirectoryName(Log4netConfigPath);

        private static string Log4netConfigPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            ConfigItems.Log4netConfigPath.TrimStart('/').Replace("/", "\\"));

        static void CheckLog4netConfigPath()
        {
            try
            {
                if (!File.Exists(Log4netConfigPath))
                {
                    if (!Directory.Exists(Log4netConfigDir))
                        Directory.CreateDirectory(Log4netConfigDir);
                    if (File.Exists(Log4netConfigPath)) return;
                    using (var streamWriter = new StreamWriter(Log4netConfigPath, false))
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
            catch (Exception ex)
            {

            }
        }
        private const string DefaultConfigContent =
@"<?xml version='1.0' encoding='utf-8' ?>
        <configuration>
        	<log4net>
        		<root>
        			<level value='ALL'/>
        			<appender-ref ref='DebugAppender' />
        			<appender-ref ref='InfoAppender' />
        			<appender-ref ref='WarnAppender'/>
        			<appender-ref ref='ErrorAppender'/>
        			<appender-ref ref='FatalAppender'/>
        		</root>
                <appender name='DebugAppender' type='log4net.Appender.RollingFileAppender'>
        			<filter type='log4net.Filter.LevelRangeFilter'>
        				<param name='LevelMin' value='DEBUG' />
        				<param name='LevelMax' value='FATAL' />
        			</filter>
        			<param name='File' value='Logs/debug_'/>
        			<param name = 'AppendToFile' value='true'/>
        			<param name = 'RollingStyle' value='Composite'/>
        			<param name = 'DatePattern' value='yyyy-MM-dd&quot;.log&quot;'/>
        			<param name = 'MaximumFileSize' value='10MB'/>
        			<param name = 'MaxSizeRollBackups' value='10'/>
        			<param name = 'StaticLogFileName' value='false'/>
        			<lockingModel type = 'log4net.Appender.FileAppender+MinimalLock' />
        			<layout type='log4net.Layout.PatternLayout'>
                        <param name = 'ConversionPattern' value='%n＝＝＝＝＝＝＝＝＝＝=============
        %n【日志级别】%-5level
        %n【记录时间】%date
        %n【线程编号】[%thread]
        %n【执行时间】[%r]毫秒
        %n【出错文件】%F
        %n【出错行号】%L
        %n【日志的类】%logger 属性[%property{NDC}]
        %n【日志描述】%message
        %n【错误详情】%newline'/>
        			</layout>
        		</appender>
        		<appender name='InfoAppender' type='log4net.Appender.RollingFileAppender'>
        			<filter type='log4net.Filter.LevelRangeFilter'>
        				<param name='LevelMin' value='INFO' />
        				<param name='LevelMax' value='INFO' />
        			</filter>
        			<param name='File' value='Logs/info_'/>
        			<param name = 'AppendToFile' value='true'/>
        			<param name = 'RollingStyle' value='Composite'/>
        			<param name = 'DatePattern' value='yyyy-MM-dd&quot;.log&quot;'/>
        			<param name = 'MaximumFileSize' value='10MB'/>
        			<param name = 'MaxSizeRollBackups' value='10'/>
        			<param name = 'StaticLogFileName' value='false'/>
        			<lockingModel type = 'log4net.Appender.FileAppender+MinimalLock' />
        			<layout type='log4net.Layout.PatternLayout'>
        			<param name = 'ConversionPattern' value='%n＝＝＝＝＝＝＝＝＝＝=============
        %n【日志级别】%-5level
        %n【记录时间】%date
        %n【线程编号】[%thread]
        %n【执行时间】[%r]毫秒
        %n【日志的类】%logger 属性[%property{NDC}]
        %n【日志描述】%message
        %n【日志详情】%newline'/>
        			</layout>
        		</appender>
                <appender name = 'WarnAppender' type='log4net.Appender.RollingFileAppender'>
        			<filter type = 'log4net.Filter.LevelRangeFilter' >
        				<param name='LevelMin' value='WARN' />
        				<param name = 'LevelMax' value='WARN' />
        			</filter>
        			<param name = 'File' value='Logs/warn_'/>
        			<param name = 'AppendToFile' value='true'/>
        			<param name = 'RollingStyle' value='Composite'/>
        			<param name = 'DatePattern' value='yyyy-MM-dd&quot;.log&quot;'/>
        			<param name = 'MaximumFileSize' value='10MB'/>
        			<param name = 'MaxSizeRollBackups' value='10'/>
        			<param name = 'StaticLogFileName' value='false'/>
        			<lockingModel type = 'log4net.Appender.FileAppender+MinimalLock' />
        			<layout type='log4net.Layout.PatternLayout'>
        			<param name = 'ConversionPattern' value='%n＝＝＝＝＝＝＝＝＝＝=============
        %n【日志级别】%-5level
        %n【记录时间】%date
        %n【线程编号】[%thread]
        %n【执行时间】[%r]毫秒
        %n【出错文件】%F
        %n【出错行号】%L
        %n【出错的类】%logger 属性[%property{NDC}]
        %n【错误描述】%message
        %n【错误详情】%newline'/>
        			</layout>
        		</appender>
        		<appender name = 'ErrorAppender' type='log4net.Appender.RollingFileAppender'>
        			<filter type = 'log4net.Filter.LevelRangeFilter' >
        				<param name='LevelMin' value='ERROR' />
        				<param name = 'LevelMax' value='ERROR' />
        			</filter>
        			<param name = 'File' value='Logs/error_'/>
        			<param name = 'AppendToFile' value='true'/>
        			<param name = 'RollingStyle' value='Composite'/>
        			<param name = 'DatePattern' value='yyyy-MM-dd&quot;.log&quot;'/>
        			<param name = 'MaximumFileSize' value='10MB'/>
        			<param name = 'MaxSizeRollBackups' value='10'/>
        			<param name = 'StaticLogFileName' value='false'/>
        			<lockingModel type = 'log4net.Appender.FileAppender+MinimalLock' />
        			<layout type='log4net.Layout.PatternLayout'>
        			<param name = 'ConversionPattern' value='%n＝＝＝＝＝＝＝＝＝＝=============
        %n【日志级别】%-5level
        %n【记录时间】%date
        %n【线程编号】[%thread]
        %n【执行时间】[%r]毫秒
        %n【出错文件】%F
        %n【出错行号】%L
        %n【出错的类】%logger 属性[%property{NDC}]
        %n【错误描述】%message
        %n【错误详情】%newline'/>
        			</layout>
        		</appender>
        		<appender name = 'FatalAppender' type='log4net.Appender.RollingFileAppender'>
        			<filter type = 'log4net.Filter.LevelRangeFilter' >
        				<param name='LevelMin' value='FATAL' />
        				<param name = 'LevelMax' value='FATAL' />
        			</filter>
        			<param name = 'File' value='Logs/fatal_'/>
        			<param name = 'AppendToFile' value='true'/>
        			<param name = 'RollingStyle' value='Composite'/>
        			<param name = 'DatePattern' value='yyyy-MM-dd&quot;.log&quot;'/>
        			<param name = 'MaximumFileSize' value='10MB'/>
        			<param name = 'MaxSizeRollBackups' value='10'/>
        			<param name = 'StaticLogFileName' value='false'/>
        			<lockingModel type = 'log4net.Appender.FileAppender+MinimalLock' />
        			<layout type='log4net.Layout.PatternLayout'>
        				 <param name = 'ConversionPattern' value='%n＝＝＝＝＝＝＝＝＝＝=============
        %n【日志级别】%-5level
        %n【记录时间】%date
        %n【线程编号】[%thread]
        %n【执行时间】[%r]毫秒
        %n【出错文件】%F
        %n【出错行号】%L
        %n【出错的类】%logger 属性[%property{NDC}]
        %n【错误描述】%message
        %n【错误详情】%newline'/>
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

        static void Log4NetPublish(LogInfo logInfo)
        {
            if (logInfo == null) return;
            Log(logInfo.Level, logInfo.Content, logInfo.Exceptions);
        }

        private static void Log(LogLevels level, string message, Exception logException = null)
        {
            try
            {
                if (DefaultLog == null) return;
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