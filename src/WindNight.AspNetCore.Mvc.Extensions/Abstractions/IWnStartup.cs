using System;
using Microsoft.AspNetCore.Mvc.WnExtensions.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc.WnExtensions.Abstractions
{
    public interface IWnStartup
    {
        IServiceProvider BuildServices(IServiceCollection services);
    }

    public interface IWnWebStartup : IWnStartup
    {
    }
}

namespace Microsoft.AspNetCore.GRpc.WnExtensions.Abstractions
{
    public interface IWnGRpcStartup : IWnStartup
    {
    }
}