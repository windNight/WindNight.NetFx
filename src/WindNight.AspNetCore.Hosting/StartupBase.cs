using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.WnExtensions;
using Microsoft.AspNetCore.Mvc.WnExtensions.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Extensions;
using System;

namespace Microsoft.AspNetCore.Hosting.WnExtensions
{
    public abstract class WebStartupBase : IWnWebStartup
    {
        protected abstract string NamespaceName { get; }
        protected abstract void UseBizConfigure(IApplicationBuilder app);
        protected abstract void ConfigBizServices(IServiceCollection services);
        protected virtual StaticFileOptions FileOptions { get; } = null;
        protected virtual Action<IEndpointRouteBuilder> SelfRouter { get; } = null;

        public WebStartupBase(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected IConfiguration Configuration { get; }

        public IServiceProvider BuildServices(IServiceCollection services)
        {
            var serviceProvider = CreateServiceProvider(services);
            Ioc.Instance.InitServiceProvider(serviceProvider);
            return serviceProvider;
        }


        public virtual void ConfigureServices(IServiceCollection services)
        {
            ConfigSysServices(services, Configuration);
            ConfigBizServices(services);
            BuildServices(services);
        }

        public virtual void Configure(IApplicationBuilder app)
        {
            UseBizConfigure(app);
            UseSysConfigure(app);
        }

        public virtual IServiceProvider CreateServiceProvider(
            IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }


        protected virtual void UseSysConfigure(IApplicationBuilder app)
        {
            var env = Ioc.GetService<IWebHostEnvironment>();
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseSwaggerConfig(NamespaceName);

            app.UseRouting();
            UseStaticFiles(app);


            app.UseAuthorization();

            app.UseCors(options => options.SetIsOriginAllowed(x => _ = true).AllowAnyMethod().AllowAnyHeader().AllowCredentials());

            UseEndpoints(app);

        }

        protected virtual IApplicationBuilder UseStaticFiles(IApplicationBuilder app)
        {
            if (FileOptions == null)
                app.UseStaticFiles();
            else
            {
                app.UseStaticFiles(FileOptions);
            }

            return app;
        }
        protected virtual IApplicationBuilder UseEndpoints(IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                SelfRouter?.Invoke(endpoints);
                endpoints.MapControllers();

            });

            return app;
        }


        protected virtual IServiceCollection ConfigSysServices(IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
            services.AddMvcBuilderWithDefaultFilters();

            services.AddSwaggerConfig(NamespaceName, configuration);
            return services;
        }
    }
}