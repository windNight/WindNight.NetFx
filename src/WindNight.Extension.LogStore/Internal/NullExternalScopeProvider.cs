using Microsoft.Extensions.Logging;

namespace WindNight.Extension.Logger.DcLog.Internal
{
    internal class NullExternalScopeProvider : IExternalScopeProvider
    {
        private NullExternalScopeProvider()
        {
        }

        /// <summary>
        ///     Returns a cached instance of <see cref="NullExternalScopeProvider" />.
        /// </summary>
        public static IExternalScopeProvider Instance { get; } = new NullExternalScopeProvider();

        /// <inheritdoc />
        void IExternalScopeProvider.ForEachScope<TState>(Action<object, TState> callback, TState state)
        {
        }

        /// <inheritdoc />
        IDisposable IExternalScopeProvider.Push(object state)
        {
            return NullScope.Instance;
        }
    }

    internal class NullScope : IDisposable
    {
        private NullScope()
        {
        }

        public static NullScope Instance { get; } = new NullScope();

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}
