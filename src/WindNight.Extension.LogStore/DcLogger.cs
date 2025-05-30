using System.Reflection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Extension;
using Newtonsoft.Json.Linq;
using WindNight.Core.ExceptionExt;
using WindNight.Extension.Logger.DcLog.Abstractions;
using WindNight.Extension.Logger.DcLog.@internal;

namespace WindNight.Extension.Logger.DcLog
{
    internal class DcLogger : ILogger
    {
        private readonly IDcLoggerProcessor _messageQueue;
        private static Version _version => new AssemblyName(typeof(DcLogger).Assembly.FullName).Version;
        private static DateTime _compileTime => File.GetLastWriteTime(typeof(DcLogger).Assembly.Location);

        public static string CurrentVersion => _version.ToString();
        public static DateTime CurrentCompileTime => _compileTime;

        private readonly string _name;
        internal DcLogOptions _options;

        public DcLogger(string name, DcLogOptions options, IDcLoggerProcessor messageQueue)
        {
            _options = options;
            _messageQueue = messageQueue;
            _name = name;
        }

        internal IExternalScopeProvider ScopeProvider { get; set; }

        public IDisposable BeginScope<TState>(TState state)
        {
            return ScopeProvider?.Push(state) ?? NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _options.MinLogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            var now = HardInfo.Now;
            if (!message.IsNullOrEmpty() || exception != null)
            {
                if (state is StateDataEntry stateEntry)
                {

                    var messageEntity = new SysLogs
                    {
                        LogAppCode = _options?.LogAppCode ?? "",
                        LogAppName = _options?.LogAppName ?? "",
                        RequestUrl = stateEntry.ApiUrl,
                        ClientIp = stateEntry.ClientIP,
                        ServerIp = stateEntry.ServerIP,
                        SerialNumber = stateEntry.SerialNumber,
                        RunMs = stateEntry.Timestamps,
                        LogTs = stateEntry.LogTs,
                        IsForce = stateEntry.IsForce,
                        Level = stateEntry.Level.ToString(),
                        LevelType = (int)stateEntry.Level,
                        NodeCode = HardInfo.NodeCode ?? "",
                        LogPluginVersion = $"{nameof(DcLogger)}/{CurrentVersion} {CurrentCompileTime:yyyy-MM-dd HH:mm:ss}",

                    };
                    if (exception != null)
                    {
                        messageEntity.ExceptionObj = new ExceptionData
                        {
                            Message = exception.Message,
                            StackTraceString = exception.StackTrace,
                        };
                        messageEntity.Exceptions = messageEntity.ExceptionObj.ToJsonStr();
                    }
                    else
                    {
                        messageEntity.Exceptions = "{}";
                    }

                    messageEntity.Content = FixContent(message);
                    // _options.ContentMaxLength > 0 && message.Length > _options.ContentMaxLength ? message.Substring(0, _options.ContentMaxLength) : message;
                    _messageQueue.EnqueueMessage(messageEntity);
                }
                else
                {

                    var logMsg = FixLogMessage(logLevel, state, message);
                    _messageQueue.EnqueueMessage(logMsg);
                }
            }

        }

        private SysLogs FixLogMessage<TState>(LogLevel logLevel, TState state, string message)
        {
            var now = HardInfo.Now;
            var logTimestamps = now.ConvertToUnixTime();
            var logDate = now.ToString("yyyyMMdd");
            var logMsg = new SysLogs
            {
                LogAppCode = _options?.LogAppCode ?? "",
                LogAppName = _options?.LogAppName ?? "",
                Level = logLevel.ToString(),
                LevelType = (int)logLevel,
                LogTs = logTimestamps,
                NodeCode = HardInfo.NodeCode ?? "",
                LogPluginVersion = $"{nameof(DcLogger)}/{CurrentVersion} {CurrentCompileTime:yyyy-MM-dd HH:mm:ss}",
            };

            if (TryGetJObject(state, out var jo))
            {
                if (string.IsNullOrEmpty(jo["logAppCode"]?.ToString()))
                {
                    jo["logAppCode"] = _options.LogAppCode;
                }

                if (string.IsNullOrEmpty(jo["logAppName"]?.ToString()))
                {
                    jo["logAppName"] = _options.LogAppName;
                }
                jo["logTimestamps"] = logTimestamps;
                jo["logDate"] = logDate;
                message = jo.ToJsonStr();
            }
            else
            {
                //var log = new
                //{
                //    // app = _options.AppCode,
                //    level = logLevel.ToString(),
                //    content = message,
                //    logAppCode = _options.LogAppCode,
                //    logAppName = _options.LogAppName,
                //    logTimestamps,
                //    logDate
                //};
                // logMsg = log.ToJsonStr();
            }

            logMsg.Content = FixContent(message);

            //;_options.ContentMaxLength > 0 && message.Length > _options.ContentMaxLength ? message.Substring(0, _options.ContentMaxLength) : message;

            return logMsg;
        }

        string FixContent(string message)
        {
            var configMaxLen = _options.ContentMaxLength;
            var msg = configMaxLen > 0 && message.Length > configMaxLen ?
                message.Substring(0, configMaxLen)
                :
                message;

            return msg;

        }

        private bool TryGetJObject(object obj, out JObject jo)
        {
            try
            {
                jo = JObject.FromObject(obj);
                return true;
            }
            catch (Exception e)
            {
                jo = null;
                if (_options.IsConsoleLog)
                {
                    Console.WriteLine(e.GetMessage());
                }
                return false;
            }
        }

    }


}
