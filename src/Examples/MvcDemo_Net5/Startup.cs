using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.WnExtensions;

namespace MvcDemo_Net5
{
    public class Startup : WebStartupBase
    {
        public Startup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override string NamespaceName => Assembly.GetEntryAssembly()?.FullName;

        protected override void ConfigBizServices(IServiceCollection services)
        {
            

        }

        protected override void UseBizConfigure(IApplicationBuilder app)
        {
            // TODO add your code here

        }


     

   
    }
}
