using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Extension;
using WindNight.Extension.Logger.DbLog.Abstractions;
using WindNight.Extension.Logger.Mysql.DbLog.Internal;

namespace WindNight.Extension.Logger.Mysql.DbLog
{
    internal class DbLogger : ILogger
    {
        private readonly IDbLoggerProcessor _messageQueue;

        private readonly string _name;
        internal DbLogOptions _options;

        public DbLogger(string name, DbLogOptions options, IDbLoggerProcessor messageQueue)
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
            if (!IsEnabled(logLevel)) return;

            if (formatter == null) throw new ArgumentNullException(nameof(formatter));

            var message = formatter(state, exception);

            var now = DateTime.Now;
            var logTimestamps = now.ConvertToUnixTime();
            var logDate = now.ToString("yyyyMMdd");
            if (!string.IsNullOrEmpty(message) || exception != null)
            {

                var logMsg = FixLogMessage(logLevel, state, message);
                _messageQueue.EnqueueMessage(logMsg);

            }
        }

        private string FixLogMessage<TState>(LogLevel logLevel, TState state, string message)
        {
            var logMsg = string.Empty;
            var now = DateTime.Now;
            var logTimestamps = now.ConvertToUnixTime();
            var logDate = now.ToString("yyyyMMdd");
            if (TryGetJObject(state, out var jo))
            {
                if (string.IsNullOrEmpty(jo["logAppCode"]?.ToString())) jo["logAppCode"] = _options.LogAppCode;
                if (string.IsNullOrEmpty(jo["logAppName"]?.ToString())) jo["logAppName"] = _options.LogAppName;
                jo["logTimestamps"] = logTimestamps;
                jo["logDate"] = logDate;
                logMsg = jo.ToJsonStr();
            }
            else
            {
                var log = new
                {
                    // app = _options.AppCode,
                    level = logLevel.ToString(),
                    content = message,
                    logAppCode = _options.LogAppCode,
                    logAppName = _options.LogAppName,
                    logTimestamps,
                    logDate
                };
                logMsg = log.ToJsonStr();
            }

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
