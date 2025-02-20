using System.Reflection;
using Microsoft.AspNetCore.Hosting.WnExtensions;
using Microsoft.AspNetCore.Mvc.WnExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Swashbuckle.AspNetCore.Extensions;
using WindNight.ConfigCenter.Extension;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Net8ApiDemo
{
    public class Startup : WebStartupBase
    {
        public Startup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override string NamespaceName => Assembly.GetEntryAssembly()?.FullName;
        protected override void UseBizConfigure(IApplicationBuilder app)
        {
        }

        protected override void ConfigBizServices(IServiceCollection services)
        {

        }

        protected override Func<Dictionary<string, string>> SelfSwaggerAuthDictFunc => () => new Dictionary<string, string>
        {
            {"Authorization","格式 Bearer xx"},
            {"AppId","执行的AppId"},
            {"AppCode","执行的AppCode"},
            {"AppToken","当前请求的Token"},
            {"Ts","当前时间戳"},
        };

        protected override Action<IApplicationBuilder> SelfMiddlewareAction => app =>
        {
            var swaggerSignDict = SelfSwaggerAuthDictFunc.Invoke();
            app.UseMiddleware<SelfSwaggerSignValidMiddleware>(swaggerSignDict);
        };



    }

    //public class Startup222 : WebStartupBase
    //{

    //    public Startup222(IConfiguration configuration) : base(configuration)
    //    {

    //    }

    //    protected override Action<SwaggerUIOptions> SelfSwaggerUIOptionsAction => (opt => opt.DefaultModelsExpandDepth(-1));


    //    protected override string NamespaceName => Assembly.GetEntryAssembly()?.FullName;
    //    protected override Func<Dictionary<string, string>> SelfSwaggerAuthDictFunc => () => new Dictionary<string, string>
    //    {
    //        {"Authorization","格式 Bearer xx"},
    //        {"AppId","执行的AppId"},
    //        {"AppCode","执行的AppCode"},
    //        {"AppToken","当前请求的Token"},
    //        {"Ts","当前时间戳"},
    //    };

    //    protected override void UseBizConfigure(IApplicationBuilder app)
    //    {
    //        //  var swaggerSignDict = SelfSwaggerAuthDictFunc.Invoke();
    //        //  app.UseMiddleware<SelfSwaggerSignValidMiddleware>(swaggerSignDict);

    //    }


    //    protected override void ConfigBizServices(IServiceCollection services)
    //    {

    //    }



    //    public override IServiceProvider BuildServices(IServiceCollection services)
    //    {
    //        var serviceProvider = CreateServiceProvider(services);
    //        Ioc.Instance.InitServiceProvider(serviceProvider);
    //        return serviceProvider;
    //    }

    //    public override void ConfigureServices(IServiceCollection services)
    //    {
    //        ConfigSysServices(services, Configuration);
    //        ConfigBizServices(services);
    //        BuildServices(services);

    //        // return BuildServices(services);
    //    }

    //    public override void Configure(IApplicationBuilder app)
    //    {
    //        UseBizConfigure(app);
    //        UseSysConfigure(app);
    //    }

    //    public override IServiceProvider CreateServiceProvider(
    //        IServiceCollection services)
    //    {
    //        var serviceProvider = services.BuildServiceProvider();
    //        return serviceProvider;
    //    }

    //    protected override void UseSysConfigure(IApplicationBuilder app)
    //    {
    //        var env = Ioc.GetService<IWebHostEnvironment>();
    //        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();



    //        app.UseRouting();

    //        app.UseSwaggerConfig(NamespaceName,
    //            swaggerOptionsAction: SelfSwaggerOptionsAction,
    //            swaggerUIOptionsAction: SelfSwaggerUIOptionsAction);

    //        //app.UseSwaggerConfig(NamespaceName);

    //        UseStaticFiles(app);


    //        app.UseAuthorization();

    //        app.UseCors(options => options.SetIsOriginAllowed(x => _ = true).AllowAnyMethod().AllowAnyHeader().AllowCredentials());


    //        var swaggerSignDict = SelfSwaggerAuthDictFunc.Invoke();
    //        app.UseMiddleware<SelfSwaggerSignValidMiddleware>(swaggerSignDict);

    //        UseEndpoints(app);
    //        //app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

    //    }

    //    protected override IApplicationBuilder UseStaticFiles(IApplicationBuilder app)
    //    {
    //        var flag = Configuration.GetAppConfigValue("DefaultStaticFileEnable", true, false);
    //        if (flag)
    //        {
    //            app.UseStaticFiles();
    //        }

    //        if (SelfFileOptions != null)
    //        {
    //            var selfConfigs = SelfFileOptions.Invoke();
    //            foreach (var item in selfConfigs)
    //            {
    //                app.UseStaticFiles(item);
    //            }
    //        }

    //        return app;
    //    }

    //    protected override IApplicationBuilder UseEndpoints(IApplicationBuilder app)
    //    {
    //        app.UseEndpoints(endpoints =>
    //        {
    //            endpoints.MapControllers();
    //            try
    //            {
    //                SelfRouter?.Invoke(endpoints);
    //            }
    //            catch
    //            {

    //            }

    //        });

    //        return app;
    //    }


    //    protected override IServiceCollection ConfigSysServices(IServiceCollection services,
    //        IConfiguration configuration)
    //    {
    //        services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
    //        if (ActionFiltersFunc != null)
    //        {
    //            services.AddMvcBuilderWithSelfFilters(ActionFiltersFunc.Invoke());
    //        }
    //        else
    //        {
    //            services.AddMvcBuilderWithDefaultFilters();
    //        }
    //        var appName = configuration.GetAppConfigValue("AppName", "");
    //        var appCode = configuration.GetAppConfigValue("AppCode", "");
    //        var prefix = $"{appName}[{appCode}] ";
    //        var title = $"{prefix}{NamespaceName}";
    //        var signKeyDict = SelfSwaggerAuthDictFunc?.Invoke() ?? new Dictionary<string, string>();
    //        services.AddSwaggerConfig(title, configuration, swaggerGenOptionsAction: SelfSwaggerGenOptionsAction, signKeyDict: signKeyDict);

    //        return services;

    //    }





    //}


    //public class Startup111
    //{
    //    private static readonly string NamespaceName = Assembly.GetEntryAssembly()?.FullName;

    //    public Startup111(IConfiguration configuration)
    //    {
    //        Configuration = configuration;
    //    }

    //    public IConfiguration Configuration { get; }

    //    // This method gets called by the runtime. Use this method to add services to the container.
    //    public void ConfigureServices(IServiceCollection services)
    //    {
    //        services.AddControllers();

    //        services.AddSwaggerConfig(NamespaceName, Configuration, signKeyDict: SwaggerSignDict);
    //    }

    //    private Dictionary<string, string> SwaggerSignDict1 = new Dictionary<string, string>();

    //    Dictionary<string, string> SwaggerSignDict = new Dictionary<string, string>
    //    {
    //        {"Authorization","格式 Bearer xx"},
    //        {"AppId","执行的AppId"},
    //        {"AppCode","执行的AppCode"},
    //        {"AppToken","当前请求的Token"},
    //        {"H1","H1"},
    //        {"Ts","当前时间戳"},
    //    };

    //    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    //    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    //    {
    //        if (env.IsDevelopment())
    //        {
    //            app.UseDeveloperExceptionPage();
    //        }

    //        //app.UseHttpsRedirection();

    //        app.UseRouting();
    //        app.UseSwaggerConfig(NamespaceName);

    //        app.UseAuthorization();

    //        //  app.UseMiddleware<SignatureValidationMiddleware>(SignDict);
    //        app.UseMiddleware<SelfSwaggerSignValidMiddleware>(SwaggerSignDict);

    //        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    //    }

    //}


}
