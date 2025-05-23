using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WindNight.Extension.Logger.DcLog.Abstractions;
using WindNight.Extension.Logger.DcLog.@internal;

namespace WindNight.Extension.Logger.DcLog
{
    [ProviderAlias("DcLogger")]
    public class DcLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly ConcurrentDictionary<string, DcLogger> _loggers;

        private readonly IDcLoggerProcessor _messageQueue;
        private readonly IOptionsMonitor<DcLogOptions> _options;

        private readonly IDisposable _optionsReloadToken;
        private IExternalScopeProvider _scopeProvider = NullExternalScopeProvider.Instance;

        public DcLoggerProvider(IOptionsMonitor<DcLogOptions> options)
        {
            _options = options;
            _loggers = new ConcurrentDictionary<string, DcLogger>();

            ReloadLoggerOptions(options.CurrentValue);
            _optionsReloadToken = _options.OnChange(ReloadLoggerOptions);

            _messageQueue = DcLoggerExtensions.LoggerProcessor ?? new DcLoggerProcessor(_options);
        }


        public void Dispose()
        {
            _optionsReloadToken?.Dispose();
            _messageQueue.Dispose();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName,
                loggerName => new DcLogger(categoryName, _options.CurrentValue, _messageQueue));

        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;

            foreach (var logger in _loggers)
            {
                logger.Value.ScopeProvider = _scopeProvider;
            }

        }
        private void ReloadLoggerOptions(DcLogOptions options)
        {
            foreach (var logger in _loggers)
            {
                logger.Value._options = options;
            }
        }

    }
}
