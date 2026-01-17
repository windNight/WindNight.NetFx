using System;
using System.Attributes;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Extensions.Abstractions;
using Swashbuckle.AspNetCore.Extensions.@internal;
using Swashbuckle.AspNetCore.HideApi;
using Swashbuckle.AspNetCore.HideApi.@internal;
using Swashbuckle.AspNetCore.HideApi.Middleware;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using WindNight.Core.Attributes.Abstractions;
using WindNight.Linq.Extensions.Expressions;
using JsonNamingPolicy = System.Text.Json.Extension.JsonNamingPolicy;

namespace Swashbuckle.AspNetCore.Extensions
{
    public static class SwaggerExtension
    {
        /// <summary>
        ///     Init Config
        ///     AddSwaggerGen
        ///     Add HiddenApiAttribute And HiddenSwaggerFilter
        /// </summary>
        /// <param name="services"></param>
        /// <param name="title"></param>
        /// <param name="configuration"></param>
        /// <param name="swaggerConfig"></param>
        /// <param name="apiVersion"></param>
        /// <param name="swaggerGenOptionsAction"></param>
        /// <param name="paramUpperCamelCase"></param>
        /// <remarks>
        ///     add AddSwaggerGen with DocumentFilter
        ///     1、 <see cref="HiddenApiAttribute" />
        ///     2、 <see cref="System.Attributes.HiddenSwaggerFilter" />
        ///     add all files like *.xml in AppContext.BaseDirectory
        /// </remarks>
        /// <returns></returns>
        public static IServiceCollection AddSwaggerConfig(this IServiceCollection services,
            string title,
            IConfiguration configuration,
            ISwaggerConfig swaggerConfig = null,
            string apiVersion = "v1",
            Action<SwaggerGenOptions> swaggerGenOptionsAction = null,
            bool paramUpperCamelCase = true,
            Dictionary<string, string> signKeyDict = null,
            Dictionary<string, string> resDict = null,
            bool useDefaultRes = false, string apiDes = "")
        {
            if (swaggerConfig != null)
            {
                ConfigItems.InitSwaggerConfig(swaggerConfig);
            }

            Ioc.Instance.InitServiceProvider(services);

            services.AddSwaggerGen(c =>
            {
                //c.DocInclusionPredicate((docName, apiDesc) =>
                //{
                //    if (ConfigItems.ShowHiddenApi) return false;

                //    // 根据apiDesc来决定是否包含某个API到Swagger文档
                //    return true; // 或者根据条件返回false
                //});

            
                if (!paramUpperCamelCase)
                {
                    c.DescribeAllParametersInCamelCase();
                }


                //c.SchemaFilter<PascalCaseSchemaFilter>();

                //c.SchemaFilter<HiddenSchemasResolver>();

                //c.DocumentFilter<HiddenApiAttribute>();
                c.DocumentFilter<HiddenApiFilter>();
                c.DocumentFilter<HiddenSwaggerFilter>();
                //c.OperationFilter<HiddenSwaggerFilter>();

                var openApiInfo = new OpenApiInfo
                {
                    Title = title,
                    Version = apiVersion,

                };


                if (apiDes.IsNotNullOrEmpty())
                {
                    openApiInfo.Description = @$"<details>
<summary>
<span style='color: #3498db; cursor: pointer;'>🔍<strong>展开</strong></span></summary>
{apiDes}
</details>";   /*
                *
                *
                *
<h3 style='color: #27ae60;'>欢迎使用本 API</h3>
                  <h3 style='color: red;'>欢迎使用本 API</h3>
                  <span style='color: red;font-weight: bold;'>红色</span>
                  <p>这是一个<strong>示例文档</strong>，支持以下功能：</p>
                  
                  <ul>
                    <li>🔧 参数验证</li>
                    <li>📡 JWT 认证</li>
                    <li><span style='color: #9b59b6;'>动态数据响应</span></li>
                  </ul>
                  
                  <pre><code>// 示例代码
                  public ActionResult Get()
                  {
                      return Ok();
                  }</code></pre>
                     * “支持 **Markdown** 和 <span style='color: #e74c3c;'>HTML 样式</span>！

                      -换行使用 `< br >` 或段落
                      - 代码高亮：`var example = Hello;`
                      -颜色：< span style = 'background: #f1c40f; padding: 2px;' > 高亮文本 </ span >
                     *
                     */
                }
                c.SwaggerDoc(apiVersion, openApiInfo);

                // 允许 Description 字段渲染 HTML
                c.UseInlineDefinitionsForEnums();
                // c.SchemaFilter<HtmlDescriptionFilter>();

                //if (XmlHelper.Instance.DocumentFiles.IsNullOrEmpty())
                //{
                var path = AppContext.BaseDirectory;
                foreach (var file in Directory.GetFiles(path))
                {
                    if (".xml".Equals(Path.GetExtension(file)))
                    {
                        c.IncludeXmlComments(Path.GetFullPath(file));
                    }
                }
                //}
                //else
                //{
                //    XmlHelper.Instance.DocumentFiles.ForEach(m => c.IncludeXmlComments(m));
                //}

                swaggerGenOptionsAction?.Invoke(c);

                ProcessSecurityMode(c, configuration, signKeyDict);
                //ProcessResponseMode(c, configuration, resDict);
            });

            return services;
        }

        private static void ProcessResponseMode(SwaggerGenOptions c, IConfiguration configuration,
            Dictionary<string, string> resDict = null, bool useDefaultRes = false)
        {
            if (resDict.IsNullOrEmpty())
            {
                var sectionKey = nameof(SwaggerConfigs);
                var config = configuration.GetSection(sectionKey).Get<SwaggerConfigs>();
                resDict = config.ResConfigs;
                if (resDict.IsNullOrEmpty() && useDefaultRes)
                {
                    resDict = new Dictionary<string, string>
                    {
                        { "0", "OK" },
                        { "100400", "BadRequest" },
                        { "100401", "Unauthorized" },
                        { "100404", "NOT FOUND" },
                        { "100500", "SystemError" },
                    };
                }
            }

            if (resDict.IsNullOrEmpty())
            {
                return;
            }

            c.OperationFilter<CustomResponseOperationFilter>(resDict);


        }

        private static void ProcessSecurityMode(SwaggerGenOptions c, IConfiguration configuration,
            Dictionary<string, string> signKeyDict = null)
        {
            if (signKeyDict.IsNullOrEmpty())
            {
                var sectionKey = nameof(SwaggerConfigs);
                var config = configuration.GetSection(sectionKey).Get<SwaggerConfigs>();
                signKeyDict = config.GetSignDict();
            }

            if (signKeyDict.IsNotNullOrEmpty())
            {
                var securityRequirements = new OpenApiSecurityRequirement();
                foreach (var item in signKeyDict)
                {
                    var name = item.Key;
                    var des = item.Value;
                    if (name.IsNotNullOrEmpty())
                    {
                        // 添加自定义请求头
                        c.AddSecurityDefinition(name, new OpenApiSecurityScheme
                        {
                            Name = name,
                            Description = des,
                            In = ParameterLocation.Header,
                            Type = SecuritySchemeType.ApiKey,
                        });
                    
                        securityRequirements.Add(
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = name,
                                },
                            }, new string[] { });
                    }
                }

                if (securityRequirements.IsNotNullOrEmpty())
                {
                    c.AddSecurityRequirement(securityRequirements);
                }
            }
        }


        /// <summary>
        ///     UseSwagger And UseSwaggerUI
        /// </summary>
        /// <param name="app"></param>
        /// <param name="assemblyName"></param>
        /// <param name="apiVersion"></param>
        /// <param name="swaggerOptionsAction"></param>
        /// <param name="swaggerUIOptionsAction"></param>
        public static void UseSwaggerConfig(this IApplicationBuilder app, string assemblyName, string apiVersion = "v1",
            Action<SwaggerOptions> swaggerOptionsAction = null, Action<SwaggerUIOptions> swaggerUIOptionsAction = null)
        {
            //  XmlHelper.Instance.Init();
            app.UseWhen(ctx => ctx.Request.Path.StartsWithSegments("/swagger"), appBuilder =>
            {
                appBuilder.UseMiddleware<InternalSwaggerMiddlewareBase>();
            });

            app.Use(async (context, next) =>
            {
                try
                {
                    if (context.Request.Path.StartsWithSegments("/swagger"))
                    {
                        // 获取客户端IP地址 
                        var remoteIp = context.Request.HttpContext.QueryDefaultClient();
                        if (!remoteIp.IpValid())
                        {
                            context.Response.StatusCode = 404;
                            return;
                        }

                        if (ConfigItems.IsOnline && !ConfigItems.SwaggerOnlineDebug)
                        {
                            context.Response.StatusCode = 404;
                            return;
                        }

                        if (ConfigItems.HiddenSwagger)
                        {
                            context.Response.StatusCode = 404;
                            return;
                        }
                    }

                    // 检查请求路径是否匹配指定路由
                    if (context.Request.Path.Equals("/api/internal/swaggerconfigs", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!ConfigItems.OpenSwaggerDebug)
                        {
                            context.Response.StatusCode = 404;
                            return;
                        }

                        var responseData = new
                        {
                            Data = new
                            {
                                SwaggerConfig = ConfigItems.SwaggerConfigs,
                            },
                            Message = "",
                            Code = 0,
                        };

                        var options = new JsonSerializerOptions
                        {
                            //  PropertyNamingPolicy = JsonNamingPolicy.PascalCase,
                        };

                        await context.Response.WriteAsJsonAsync(responseData, options);
                        //await context.Response.WriteAsJsonAsync(responseData);
                        return;
                    }
                    await next();

                }
                catch
                {

                }

            });


            app.UseSwagger(c => { swaggerOptionsAction?.Invoke(c); });


            app.UseSwaggerUI(c =>
            {
                c.DefaultModelExpandDepth(-1);

                c.SwaggerEndpoint($"/swagger/{apiVersion}/swagger.json", assemblyName);

                c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true"); // 保留授权信息

                c.InjectJavascript(@"function toggleDetails() {
            var details = document.querySelectorAll('details');
            details.forEach(function(detail) {
                detail.open = !detail.open;
            });
        }
        document.addEventListener('DOMContentLoaded', function() {
            var toggleButton = document.createElement('button');
            toggleButton.innerHTML = 'Toggle Details';
            toggleButton.onclick = toggleDetails;
            document.querySelector('.topbar').appendChild(toggleButton);
        });
    ");
                //c.RoutePrefix = string.Empty;
                swaggerUIOptionsAction?.Invoke(c);
            });
        }
    }


}
