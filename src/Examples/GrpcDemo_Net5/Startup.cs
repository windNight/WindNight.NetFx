using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.GRpc.WnExtensions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GrpcDemo_Net5
{
    public class Startup : GRpcStartupBase
    {
        public Startup(IConfiguration configuration) : base(configuration)
        {

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        protected override void UseBizConfigure(IApplicationBuilder app)
        {

        }

        protected override void ConfigBizServices(IServiceCollection services)
        {

        }

        protected override void MapGRpcServices(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGrpcService<GreeterService>();
        }




    }
}
