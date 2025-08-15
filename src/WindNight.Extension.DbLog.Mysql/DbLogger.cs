using System.Reflection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Extension;
using Newtonsoft.Json.Linq;
using WindNight.Extension.Logger.DbLog.Abstractions;
using WindNight.Extension.Logger.DbLog.@internal;

namespace WindNight.Extension.Logger.Mysql.DbLog
{
    internal class DbLogger : ILogger
    {
        private readonly IDbLoggerProcessor _messageQueue;
        public static string CurrentVersion => BuildInfo.BuildVersion;

        public static string CurrentCompileTime => BuildInfo.BuildTime;
        public static string DbLoggerPluginVersion => $"{nameof(DbLogger)}/{CurrentVersion} {CurrentCompileTime}";

        private readonly string _name;
        internal DbLogOptions _options;

        public DbLogger(string name, DbLogOptions options, IDbLoggerProcessor messageQueue)
        {
            _options = options;
            _messageQueue = messageQueue;
            _name = name;
        }

        internal IExternalScopeProvider ScopeProvider { get; set; }

        public IDisposable? BeginScope<TState>(TState state) // where TState : notnull;
        // where TState : notnull;
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
            if (!IsEnabled(logLevel)) return;

            if (formatter == null) throw new ArgumentNullException(nameof(formatter));

            var message = formatter(state, exception);

            var now = HardInfo.Now;
            if (!string.IsNullOrEmpty(message) || exception != null)
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

                        Level = stateEntry.Level.ToString(),
                        LevelType = (int)stateEntry.Level,
                        NodeCode = HardInfo.NodeCode ?? "",
                        LogPluginVersion = DbLoggerPluginVersion,

                    };
                    if (exception != null)
                    {
                        messageEntity.ExceptionObj = new ExceptionData
                        {
                            Message = exception.Message,
                            StackTraceString = exception.StackTrace
                        };
                        messageEntity.Exceptions = messageEntity.ExceptionObj.ToJsonStr();
                    }
                    else
                    {
                        messageEntity.Exceptions = "{}";
                    }

                    messageEntity.Content = _options.ContentMaxLength > 0 && message.Length > _options.ContentMaxLength ? message.Substring(0, _options.ContentMaxLength) : message;
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
                LogPluginVersion = DbLoggerPluginVersion,

            };

            if (TryGetJObject(state, out var jo))
            {
                if (string.IsNullOrEmpty(jo["logAppCode"]?.ToString())) jo["logAppCode"] = _options.LogAppCode;
                if (string.IsNullOrEmpty(jo["logAppName"]?.ToString())) jo["logAppName"] = _options.LogAppName;
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
            logMsg.Content = _options.ContentMaxLength > 0 && message.Length > _options.ContentMaxLength ? message.Substring(0, _options.ContentMaxLength) : message;

            return logMsg;
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
                    Console.WriteLine(e);
                return false;
            }
        }

    }


}
