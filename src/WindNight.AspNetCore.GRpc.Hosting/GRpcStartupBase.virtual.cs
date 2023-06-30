using System;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.WnExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Extensions;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Microsoft.AspNetCore.GRpc.WnExtensions
{
    public partial class GRpcStartupBase
    {
        public virtual IServiceProvider BuildServices(IServiceCollection services)
        {
            var serviceProvider = CreateServiceProvider(services);
            Ioc.Instance.InitServiceProvider(serviceProvider);

            return serviceProvider;
        }

        protected virtual IServiceCollection ConfigSysServices(IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton(Configuration);
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
            services.AddMvcBuilderWithDefaultFilters();

            services.AddSwaggerConfig(NamespaceName, configuration, swaggerGenOptionsAction: option =>
            {

            });

            services.AddGrpcSwagger();

            return services;
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
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("zh-CN"),
                // Formatting numbers, dates, etc.
                SupportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("zh-CN") }
            });
            // app.UseHttpsRedirection();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSwaggerConfig(NamespaceName, swaggerUIOptionsAction: options =>
             {
                 options.DisplayOperationId();
                 options.DisplayRequestDuration();
                 options.DocExpansion(DocExpansion.Full);
                 options.EnableDeepLinking();
                 options.EnableFilter();
                 options.ShowExtensions();
                 options.EnableValidator();
             });

            //app.UseSwagger();
            //app.UseSwaggerUI(options =>
            //{
            //    options.SwaggerEndpoint("/swagger/v1/swagger.json", "gRPC HTTP API");
            //    options.DisplayOperationId();
            //    options.DisplayRequestDuration();
            //    options.DocExpansion(DocExpansion.Full);
            //    options.EnableDeepLinking();
            //    options.EnableFilter();
            //    options.ShowExtensions();
            //    options.EnableValidator();  
            //});

            app.UseRouting();
            app.UseCors(options =>
                options.SetIsOriginAllowed(x => _ = true).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            app.UseEndpoints(endpoints =>
            {
                MapGRpcServices(endpoints);
                endpoints.MapControllers();
                endpoints.MapGet("/",
                    async context =>
                    {
                        await context.Response.WriteAsync(
                            "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                    });
            });
        }
    }
}