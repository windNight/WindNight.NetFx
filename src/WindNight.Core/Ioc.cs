using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WindNight.Core.Abstractions;

namespace Microsoft.Extensions.DependencyInjection.WnExtension
{
    /// <summary> </summary>
    public partial class Ioc
    {
        private IConfigService? _currentConfigService { get; set; } = null;

        public void SetCurrentConfigService(IConfigService configService)
        {
            _currentConfigService = configService;
        }

        public IConfigService? CurrentConfigService => _currentConfigService ?? GetService<IConfigService>();


        private ILogService? _currentLogService { get; set; } = null;

        public void SetCurrentLogService(ILogService logService)
        {
            _currentLogService = logService;
        }

        public ILogService? CurrentLogService => _currentLogService ?? GetService<ILogService>();

    }

    /// <summary> </summary>
    public partial class Ioc
    {
        //private static readonly Lazy<Ioc> LazyIocInstance = new Lazy<Ioc>(() => new Ioc());
        private static readonly Lazy<Ioc> LazyIocInstance = new(() => new Ioc());

        //public static readonly Ioc Instance = new Ioc();

        private Ioc()
        {

        }

        /// <summary>
        /// </summary>
        public static Ioc Instance => LazyIocInstance.Value;

        /// <summary>
        /// </summary>
        public IServiceProvider? ServiceProvider { get; internal set; }




        /// <summary>
        /// </summary>
        /// <param name="serviceProvider"></param>
        public void InitServiceProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        /// <summary>
        ///     Get service of type T from the System.IServiceProvider.
        /// </summary>
        /// <typeparam name="T">The type of service object to get</typeparam>
        /// <param name="name"></param>
        /// <remarks>
        ///     Please Make Sure you has do Ioc.Instance.InitServiceProvider(yourIServiceProvider) when init your app.
        /// </remarks>
        /// <returns> A service object of type T or null if there is no such service. </returns>
        public static T GetService<T>(string? name = null)
        {
            if (Instance.ServiceProvider == null) return default;
            return Instance.ServiceProvider.GetService<T>(name);
        }

#if !NET45
        /// <summary>
        ///     Get service of type T from the System.IServiceProvider.
        /// </summary>
        /// <typeparam name="T">The type of service object to get</typeparam>
        /// <param name="name"></param>
        /// <remarks>
        ///     Please Make Sure you has do Ioc.Instance.InitServiceProvider(yourIServiceProvider) when init your app.
        /// </remarks>
        /// <returns> A service object of type T or null if there is no such service. </returns>
        public static IEnumerable<T> GetServices<T>()
        {
            if (Instance.ServiceProvider == null) return default;
            return Instance.ServiceProvider.GetServices<T>();
        }
#endif


    }


    public static class IocExtension
    {

        /// <summary>
        ///     Get service of type T from the System.IServiceProvider.
        /// </summary>
        /// <typeparam name="T">The type of service object to get</typeparam>
        /// <param name="serviceProvider"></param>
        /// <param name="name"></param>
        /// <remarks>
        ///     Please Make Sure you has do Ioc.Instance.InitServiceProvider(yourIServiceProvider) when init your app.
        /// </remarks>
        /// <returns> A service object of type T or null if there is no such service. </returns>
        public static T GetService<T>(this IServiceProvider? serviceProvider, string? name)
        {
            if (serviceProvider == null) return default;
#if NETSTANDARD
            if (!name.IsNullOrEmpty())
            {
                var impls = serviceProvider.GetServices<T>();
                foreach (var impl in impls)
                {
                    var alias = impl.GetType().GetCustomAttributes<AliasAttribute>().FirstOrDefault();
                    if (alias != null && alias.Name == name)
                    {
                        return impl;
                    }
                }
                return impls.FirstOrDefault();
            }
            return serviceProvider.GetService<T>();
#else
            return (T)serviceProvider.GetService(typeof(T));
#endif

        }


    }

}