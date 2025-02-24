using System.Reflection;
using Microsoft.AspNetCore.Hosting.WnExtensions;
using WindNight.Config.Abstractions;
using WindNight.Config.Extensions;

namespace WebApiDemo
{
    public partial class Startup : WebStartupBase
    {
        protected override Action<IApplicationBuilder> SelfMiddlewareAction => app =>
        {
            var swaggerSignDict = SelfSwaggerAuthDictFunc.Invoke();
            app.UseMiddleware<SelfSwaggerSignValidMiddleware>(swaggerSignDict);

        };

        protected override Func<Dictionary<string, string>> SelfSwaggerAuthDictFunc => () => new Dictionary<string, string>
        {
            {"Authorization","格式 Bearer xx"},
            {"AppId","执行的AppId"},
            {"AppCode","执行的AppCode"},
            {"AppToken","当前请求的Token"},
            {"Ts","当前时间戳"},
        };

        protected override string NamespaceName => Assembly.GetEntryAssembly()?.FullName;

        public Startup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void UseBizConfigure(IApplicationBuilder app)
        {
            // throw new NotImplementedException();


        }

        protected override void ConfigBizServices(IServiceCollection services)
        {
            // throw new NotImplementedException();
            services.AddSingleton<IConfigCenterAuth, ConfigCenterAuth>();
            services.AddConfigExtension(Configuration);
        }



    }
}
