using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.WnExtensions;
using Microsoft.AspNetCore.Mvc.WnExtensions.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using WindNight.Core.ConfigCenter.Extensions;

namespace Microsoft.AspNetCore.Hosting.WnExtensions
{
    public abstract class WebStartupBase : IWnWebStartup//, IStartup
    {
        protected abstract string BuildType { get; }

        //protected abstract string NamespaceName { get; }
        protected virtual DateTime BuildDateTime
        {
            get
            {
                try
                {
                    var assembly = Assembly.GetEntryAssembly();
                    return System.IO.File.GetLastWriteTime(assembly.Location);
                }
                catch (Exception ex)
                {
                    return HardInfo.MinDate;
                }
            }
        }

        protected virtual string ToAppendDescription => "";

        /// <summary> 暂不支持 Markdown  需要使用Html语法 </summary>
        protected virtual string ApiDescription
        {
            get
            {
                try
                {
                    var appName = Configuration.GetAppName();
                    var appCode = Configuration.GetAppCode();
                    var prefix = $"{appName}({appCode}) ";
                    var apiDes =
@$"<h3 style='color: #27ae60;'>Info</h3>
`{prefix}`  
<h3 style='color: #27ae60;'>Namespace</h3>
`{NamespaceName}`  
<h3 style='color: #27ae60;'>BuildType</h3>
<span style='background:#f1c40f;padding:2px;color:red;'><strong>`{BuildType}`</strong></span>
<h3 style='color: #27ae60;'>  BuildTs  </h3>
`{BuildDateTime}`
<h3 style='color: #27ae60;'>  ServerInfo  </h3>
```
{HardInfo.ToString(Formatting.Indented)}
```
";
                    if (!ToAppendDescription.IsNullOrEmpty())
                    {
                        apiDes =
@$"{apiDes}
<h3 style='color: #27ae60;'> Other</h3>
{ToAppendDescription}";

                    }
                    return apiDes;
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
        }
        protected virtual string NamespaceName
        {
            get
            {
                try
                {
                    var t = Assembly.GetEntryAssembly()?.FullName ?? "";
                    var name = t.Substring(0, t.IndexOf(", Culture", StringComparison.Ordinal));
                    return name;
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
        }

        protected virtual string ApiVersion
        {
            get
            {
                try
                {
                    var assembly = Assembly.GetEntryAssembly();
                    var ver = assembly?.GetName()?.Version;
                    if (ver == null)
                    {
                        return "v1";
                    }

                    var verS = $"v{ver.ToString()}";
                    return verS;
                }
                catch (Exception ex)
                {
                    return "v1";
                }
            }
        }

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

            app.UseSwaggerConfig(NamespaceName, apiVersion: ApiVersion,
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
            var flag = Configuration.GetAppSettingValue("DefaultStaticFileEnable", true, false);
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
                services.AddMvcBuilderWithSelfFilters(configuration, ActionFiltersFunc.Invoke());
            }
            else
            {
                services.AddMvcBuilderWithDefaultFilters(configuration);
            }
            // var appName = configuration.GetAppConfigValue("AppName", "");
            // var appCode = configuration.GetAppConfigValue("AppCode", "");
            // var prefix = $"{appName}({appCode}) ";
            // var title = $"{prefix}{NamespaceName}";
            var signKeyDict = SelfSwaggerAuthDictFunc?.Invoke() ?? new Dictionary<string, string>();

            services.AddSwaggerConfig(NamespaceName, configuration, swaggerGenOptionsAction: SelfSwaggerGenOptionsAction, apiVersion: ApiVersion, signKeyDict: signKeyDict, apiDes: ApiDescription);

            return services;

        }



    }
}
