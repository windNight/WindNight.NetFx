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
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using WindNight.ConfigCenter.Extension;

namespace Microsoft.AspNetCore.Hosting.WnExtensions
{
    public abstract class WebStartupBase : IWnWebStartup//, IStartup
    {
        protected abstract string NamespaceName { get; }
        protected abstract void UseBizConfigure(IApplicationBuilder app);
        protected abstract void ConfigBizServices(IServiceCollection services);
        //protected virtual StaticFileOptions SelfFileOptions { get; } = null;
        protected virtual Func<IEnumerable<StaticFileOptions>> SelfFileOptions { get; } = null;
        protected virtual Action<IEndpointRouteBuilder> SelfRouter { get; } = null;
        protected virtual Action<IApplicationBuilder> SelfMiddlewareAction { get; } = null;
        protected virtual Action<SwaggerOptions> SelfSwaggerOptionsAction { get; } = null;
        protected virtual Action<SwaggerUIOptions> SelfSwaggerUIOptionsAction { get; private set; } = null;
        protected virtual Action<SwaggerGenOptions> SelfSwaggerGenOptionsAction { get; private set; } = null;
        protected virtual Func<Dictionary<string, string>> SelfSwaggerAuthDictFunc { get; } = null;

        protected virtual Func<IEnumerable<Type>> ActionFiltersFunc { get; } = null;

        // protected virtual Action<Mvc.MvcJsonOptions>? mvcJsonOption { get; } = null;

        public WebStartupBase(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected IConfiguration Configuration { get; }

        public virtual IServiceProvider BuildServices(IServiceCollection services)
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

            // return BuildServices(services);
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

            SelfSwaggerUIOptionsAction ??= (opt => opt.DefaultModelsExpandDepth(-1));

            app.UseRouting();

            app.UseSwaggerConfig(NamespaceName,
                swaggerOptionsAction: SelfSwaggerOptionsAction,
                swaggerUIOptionsAction: SelfSwaggerUIOptionsAction);

            UseStaticFiles(app);

            app.UseAuthorization();

            app.UseCors(options => options.SetIsOriginAllowed(x => _ = true).AllowAnyMethod().AllowAnyHeader().AllowCredentials());

            SelfMiddlewareAction?.Invoke(app);

            UseEndpoints(app);


        }

        protected virtual IApplicationBuilder UseStaticFiles(IApplicationBuilder app)
        {
            var flag = Configuration.GetAppConfigValue("DefaultStaticFileEnable", true, false);
            if (flag)
            {
                app.UseStaticFiles();
            }

            if (SelfFileOptions != null)
            {
                var selfConfigs = SelfFileOptions.Invoke();
                foreach (var item in selfConfigs)
                {
                    app.UseStaticFiles(item);
                }
            }

            return app;
        }

        protected virtual IApplicationBuilder UseEndpoints(IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                try
                {
                    SelfRouter?.Invoke(endpoints);
                }
                catch
                {

                }

            });

            return app;
        }


        protected virtual IServiceCollection ConfigSysServices(IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
            if (ActionFiltersFunc != null)
            {
                services.AddMvcBuilderWithSelfFilters(ActionFiltersFunc.Invoke());
            }
            else
            {
                services.AddMvcBuilderWithDefaultFilters();
            }
            var appName = configuration.GetAppConfigValue("AppName", "");
            var appCode = configuration.GetAppConfigValue("AppCode", "");
            var prefix = $"{appName}[{appCode}] ";
            var title = $"{prefix}{NamespaceName}";
            var signKeyDict = SelfSwaggerAuthDictFunc?.Invoke() ?? new Dictionary<string, string>();
            services.AddSwaggerConfig(title, configuration, swaggerGenOptionsAction: SelfSwaggerGenOptionsAction, signKeyDict: signKeyDict);

            return services;

        }



    }
}
