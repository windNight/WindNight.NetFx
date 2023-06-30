using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using WindNight.Extension.Logger.DbLog.Abstractions;
using WindNight.Extension.Logger.Mysql.DbLog;
using WindNight.Extension.Logger.DbLog.Internal;

namespace WindNight.Extension.Logger.DbLog
{
    [ProviderAlias("DbLogger")]
    public class DbLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly ConcurrentDictionary<string, DbLogger> _loggers;

        private readonly IDbLoggerProcessor _messageQueue;
        private readonly IOptionsMonitor<DbLogOptions> _options;

        private readonly IDisposable _optionsReloadToken;
        private IExternalScopeProvider _scopeProvider = NullExternalScopeProvider.Instance;

        public DbLoggerProvider(IOptionsMonitor<DbLogOptions> options)
        {
            _options = options;
            _loggers = new ConcurrentDictionary<string, DbLogger>();

            ReloadLoggerOptions(options.CurrentValue);
            _optionsReloadToken = _options.OnChange(ReloadLoggerOptions);

            _messageQueue = DbLoggerExtensions.LoggerProcessor ?? new DbLoggerProcessor(_options);
        }


        public void Dispose()
        {
            _optionsReloadToken?.Dispose();
            _messageQueue.Dispose();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName,
                loggerName => new DbLogger(categoryName, _options.CurrentValue, _messageQueue));

        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;

            foreach (var logger in _loggers)
                logger.Value.ScopeProvider = _scopeProvider;

        }
        private void ReloadLoggerOptions(DbLogOptions options)
        {
            foreach (var logger in _loggers)
                logger.Value._options = options;
        }

    }
}
