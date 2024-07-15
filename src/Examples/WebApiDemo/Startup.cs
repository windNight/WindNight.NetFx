using System.Reflection;
using Microsoft.AspNetCore.Hosting.WnExtensions;
using WindNight.Config.Abstractions;
using WindNight.Config.Extensions;

namespace WebApiDemo
{
    public partial class Startup : WebStartupBase
    {
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
