using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.GRpc.WnExtensions.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.GRpc.WnExtensions
{
    public abstract partial class GRpcStartupBase : IWnGRpcStartup
    {
        protected static readonly string NamespaceName = Assembly.GetEntryAssembly()?.FullName;

        public GRpcStartupBase(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected IConfiguration Configuration { get; }

        protected abstract void UseBizConfigure(IApplicationBuilder app);
        protected abstract void ConfigBizServices(IServiceCollection services);
        protected abstract void MapGRpcServices(IEndpointRouteBuilder endpoints);

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigSysServices(services, Configuration);
            ConfigBizServices(services);
            BuildServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            UseBizConfigure(app);
            UseSysConfigure(app);
        }
    }
}