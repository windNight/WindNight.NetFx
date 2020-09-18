using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;
using WindNight.Core.Abstractions;

namespace WindNight.LogExtension
{
    public static partial class LogHelper
    {
        private static readonly object lockHelper = new object();
        private static ILog _log;

        static LogHelper()
        {
            Init();
        }

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

        private static void Init()
        {
            try
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
                //text.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                //text.WriteLine("<configuration>");
                //text.WriteLine("  <log4net>");
                //text.WriteLine("    <root>");
                //text.WriteLine("      <level value=\"ALL\"/>");
                //text.WriteLine("       <appender-ref ref=\"InfoAppender\" />");
                //text.WriteLine("       <appender-ref ref=\"ErrorAppender\"/>");
                //text.WriteLine("       <appender-ref ref=\"FatalAppender\"/>");
                //text.WriteLine("    </root>");
                //text.WriteLine("    <appender name=\"InfoAppender\" type=\"log4net.Appender.RollingFileAppender\">");
                //text.WriteLine("      <filter type=\"log4net.Filter.LevelRangeFilter\">");
                //text.WriteLine("          <param name=\"LevelMin\" value=\"DEBUG\" />");
                //text.WriteLine("          <param name=\"LevelMax\" value=\"INFO\" />");
                //text.WriteLine("      </filter>");
                //text.WriteLine("      <param name=\"File\" value=\"Logs/info_\"/>");
                //text.WriteLine("      <param name=\"AppendToFile\" value=\"true\"/>");
                //text.WriteLine("      <param name=\"RollingStyle\" value=\"Composite\"/>");
                //text.WriteLine("      <param name=\"DatePattern\" value=\"yyyy-MM-dd&quot;.log&quot;\"/>");
                //text.WriteLine("      <param name=\"MaximumFileSize\" value=\"10MB\"/>");
                //text.WriteLine("      <param name=\"MaxSizeRollBackups\" value=\"10\"/>");
                //text.WriteLine("      <param name=\"StaticLogFileName\" value=\"false\"/>");
                //text.WriteLine("      <lockingModel type=\"log4net.Appender.FileAppender+MinimalLock\" />");
                //text.WriteLine("      <layout type=\"log4net.Layout.PatternLayout\">");
                //text.WriteLine("        <param name=\"ConversionPattern\" value=\"[时间]:%d%n[级别]:%p%n[内容]:%m%n%n\"/>");
                //text.WriteLine("      </layout>");
                //text.WriteLine("    </appender>");
                //text.WriteLine("    <appender name=\"ErrorAppender\" type=\"log4net.Appender.RollingFileAppender\">");
                //text.WriteLine("      <filter type=\"log4net.Filter.LevelRangeFilter\">");
                //text.WriteLine("          <param name=\"LevelMin\" value=\"ERROR\" />");
                //text.WriteLine("          <param name=\"LevelMax\" value=\"ERROR\" />");
                //text.WriteLine("      </filter>");
                //text.WriteLine("      <param name=\"File\" value=\"Logs/error_\"/>");
                //text.WriteLine("      <param name=\"AppendToFile\" value=\"true\"/>");
                //text.WriteLine("      <param name=\"RollingStyle\" value=\"Composite\"/>");
                //text.WriteLine("      <param name=\"DatePattern\" value=\"yyyy-MM-dd&quot;.log&quot;\"/>");
                //text.WriteLine("      <param name=\"MaximumFileSize\" value=\"10MB\"/>");
                //text.WriteLine("      <param name=\"MaxSizeRollBackups\" value=\"10\"/>");
                //text.WriteLine("      <param name=\"StaticLogFileName\" value=\"false\"/>");
                //text.WriteLine("      <lockingModel type=\"log4net.Appender.FileAppender+MinimalLock\" />");
                //text.WriteLine("      <layout type=\"log4net.Layout.PatternLayout\">");
                //text.WriteLine("        <param name=\"ConversionPattern\" value=\"[时间]:%d%n[级别]:%p%n[内容]:%m%n%n\"/>");
                //text.WriteLine("      </layout>");
                //text.WriteLine("    </appender>");
                //text.WriteLine("    <appender name=\"FatalAppender\" type=\"log4net.Appender.RollingFileAppender\">");
                //text.WriteLine("      <filter type=\"log4net.Filter.LevelRangeFilter\">");
                //text.WriteLine("          <param name=\"LevelMin\" value=\"FATAL\" />");
                //text.WriteLine("          <param name=\"LevelMax\" value=\"FATAL\" />");
                //text.WriteLine("      </filter>");
                //text.WriteLine("      <param name=\"File\" value=\"Logs/fatal_\"/>");
                //text.WriteLine("      <param name=\"AppendToFile\" value=\"true\"/>");
                //text.WriteLine("      <param name=\"RollingStyle\" value=\"Composite\"/>");
                //text.WriteLine("      <param name=\"DatePattern\" value=\"yyyy-MM-dd&quot;.log&quot;\"/>");
                //text.WriteLine("      <param name=\"MaximumFileSize\" value=\"10MB\"/>");
                //text.WriteLine("      <param name=\"MaxSizeRollBackups\" value=\"10\"/>");
                //text.WriteLine("      <param name=\"StaticLogFileName\" value=\"false\"/>");
                //text.WriteLine("      <lockingModel type=\"log4net.Appender.FileAppender+MinimalLock\" />");
                //text.WriteLine("      <layout type=\"log4net.Layout.PatternLayout\">");
                //text.WriteLine("        <param name=\"ConversionPattern\" value=\"[时间]:%d%n[级别]:%p%n[内容]:%m%n%n\"/>");
                //text.WriteLine("      </layout>");
                //text.WriteLine("    </appender>");
                //text.WriteLine("  </log4net>");
                //text.WriteLine("</configuration>");

                text.Close();
            }
        }
    }
}