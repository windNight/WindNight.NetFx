#if CORE31LATER
using System.Text.Encodings.Web;
using System.Text.Unicode;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Filters.Extensions;
using Microsoft.AspNetCore.Mvc.WnExtensions.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.HideApi.Abstractions;
using WindNight.Linq.Extensions.Expressions;
using WindNight.AspNetCore.Mvc.Extensions;

namespace Microsoft.AspNetCore.Mvc.WnExtensions
{
    public static class MvcExtension
    {

        /// <summary>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="actionFilters"></param>
        /// <param name="addDefaultFilters"></param>
        /// <param name="mvcJsonOption">
        ///  used in  <see cref="Microsoft.Extensions.DependencyInjection.MvcJsonMvcBuilderExtensions.AddJsonOptions(IMvcBuilder, Action{MvcJsonOptions})"/>
        ///   The <see cref="T:Microsoft.AspNetCore.Mvc.MvcJsonOptions" /> which need to be configured.
        /// </param>
        /// <param name="jsonOptions">
        /// used in <see cref="Microsoft.Extensions.DependencyInjection.MvcCoreMvcBuilderExtensions.AddJsonOptions(IMvcBuilder, Action{Microsoft.AspNetCore.Mvc.JsonOptions})"/>
        /// An <see cref="T:System.Action" /> to configure the <see cref="T:Microsoft.AspNetCore.Mvc.JsonOptions" />
        /// </param>
        /// <param name="mvcJsonOptions">
        /// used in <see cref="Microsoft.Extensions.DependencyInjection.NewtonsoftJsonMvcBuilderExtensions.AddNewtonsoftJson(IMvcBuilder, Action{MvcNewtonsoftJsonOptions})"/>
        /// Callback to configure <see cref="T:Microsoft.AspNetCore.Mvc.MvcNewtonsoftJsonOptions" />
        /// </param>
        /// <returns></returns>
        public static IMvcBuilder AddMvcBuilderWithSelfFilters(this IServiceCollection services, IConfiguration configuration, IEnumerable<Type> actionFilters = null, bool addDefaultFilters = true//, Action<MvcOptions> mvcOption = null
#if !CORE31LATER
            , Action<MvcJsonOptions>? mvcJsonOption = null
#else
            , Action<JsonOptions>? jsonOptions = null, Action<MvcNewtonsoftJsonOptions>? mvcJsonOptions = null
#endif
        )
        {
            Type[] defaultFilters = {
                typeof (LogProcessAttribute),
                typeof (ApiResultFilterAttribute),
                typeof (ApiExceptionFilterAttribute),
                typeof (ValidateInputAttribute),
            };

            return services.AddMvcBuilder(configuration, options =>
                {
                    //if (mvcOption != null)
                    //{
                    //    mvcOption.Invoke(options);
                    //}
                    if (!actionFilters.IsNullOrEmpty())
                    {
                        foreach (var actionFilter in actionFilters.Distinct())
                        {
                            if (actionFilter.GetInterfaces().Contains(typeof(IFilterMetadata)) &&
                                !defaultFilters.Contains(actionFilter))
                            {
                                AddFilter(actionFilter);
                            }
                        }
                    }

                    if (addDefaultFilters)
                    {
                        foreach (var actionFilter in defaultFilters)
                        {
                            AddFilter(actionFilter);
                        }
                    }

                    void AddFilter(Type actionFilter)
                    {
                        if (!typeof(IFilterMetadata).IsAssignableFrom(actionFilter))
                        {
                            return;
                        }

                        var typeFilterAttribute = new TypeFilterAttribute(actionFilter);

                        if (!options.Filters.Contains(typeFilterAttribute))
                        {
                            options.Filters.Add(typeFilterAttribute);
                        }

                    }


                }

#if !CORE31LATER
                ,  mvcJsonOption  
#else
                , jsonOptions, mvcJsonOptions
#endif

            );
        }


        /// <summary>
        /// </summary>
        /// <param name="services"></param> 
        /// <param name="mvcOption">An <see cref="T:System.Action`1" /> to configure the provided <see cref="T:Microsoft.AspNetCore.Mvc.MvcOptions" />.</param>
        /// <param name="mvcJsonOption">
        ///  used in  <see cref="Microsoft.Extensions.DependencyInjection.MvcJsonMvcBuilderExtensions.AddJsonOptions(IMvcBuilder, Action{MvcJsonOptions})"/>
        ///   The <see cref="T:Microsoft.AspNetCore.Mvc.MvcJsonOptions" /> which need to be configured.
        /// </param>
        /// <param name="jsonOptions">
        /// used in <see cref=" Microsoft.Extensions.DependencyInjection.MvcCoreMvcBuilderExtensions.AddJsonOptions(IMvcBuilder, Action{Microsoft.AspNetCore.Mvc.JsonOptions})"/>
        /// An <see cref="T:System.Action" /> to configure the <see cref="T:Microsoft.AspNetCore.Mvc.JsonOptions" />
        /// </param>
        /// <param name="mvcJsonOptions">
        /// used in <see cref=" Microsoft.Extensions.DependencyInjection.NewtonsoftJsonMvcBuilderExtensions.AddNewtonsoftJson(IMvcBuilder, Action{MvcNewtonsoftJsonOptions})"/>
        /// Callback to configure <see cref="T:Microsoft.AspNetCore.Mvc.MvcNewtonsoftJsonOptions" />
        /// </param>
        /// <returns></returns>
        public static IMvcBuilder AddMvcBuilder(this IServiceCollection services, IConfiguration configuration,
            Action<MvcOptions> mvcOption
#if !CORE31LATER
            , Action<MvcJsonOptions>? mvcJsonOption = null
#else
            , Action<JsonOptions>? jsonOptions = null, Action<MvcNewtonsoftJsonOptions>? mvcJsonOptions = null
#endif
            )
        {
            AddCommonMvc(services, configuration);

#if CORE31LATER
            return services.AddControllers(mvcOption)
                           .AppendJsonSettings(jsonOptions, mvcJsonOptions);

#else
            return services.AddMvc(mvcOption)
                           .AppendJsonSettings(mvcJsonOption);
#endif
        }

        /// <summary>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="mvcJsonOption">
        ///  used in  <see cref="Microsoft.Extensions.DependencyInjection.MvcJsonMvcBuilderExtensions.AddJsonOptions(IMvcBuilder, Action{MvcJsonOptions})"/>
        /// </param>
        /// <param name="jsonOptions">
        /// used in <see cref=" Microsoft.Extensions.DependencyInjection.MvcCoreMvcBuilderExtensions.AddJsonOptions(IMvcBuilder, Action{JsonOptions})"/>
        /// </param>
        /// <param name="mvcJsonOptions">
        /// used in <see cref=" Microsoft.Extensions.DependencyInjection.NewtonsoftJsonMvcBuilderExtensions.AddNewtonsoftJson(IMvcBuilder, Action{MvcNewtonsoftJsonOptions})"/>
        /// </param>
        /// <remarks>
        ///     while add filters with
        ///     1、 <see cref="Microsoft.AspNetCore.Mvc.Filters.Extensions.ApiResultFilterAttribute" />
        ///     2、 <see cref="Microsoft.AspNetCore.Mvc.Filters.Extensions.ApiExceptionFilterAttribute" />
        ///     3、 <see cref="Microsoft.AspNetCore.Mvc.Filters.Extensions.ValidateInputAttribute" />
        ///     4、 <see cref="Microsoft.AspNetCore.Mvc.Filters.Extensions.LogProcessAttribute" />
        /// </remarks>
        /// <returns></returns>
        public static IMvcBuilder AddMvcBuilderWithDefaultFilters(this IServiceCollection services,
            IConfiguration configuration//, Action<MvcOptions> mvcOption = null
#if !CORE31LATER
            , Action<MvcJsonOptions>? mvcJsonOption = null
#else
            , Action<JsonOptions>? jsonOptions = null, Action<MvcNewtonsoftJsonOptions>? mvcJsonOptions = null
#endif
        )
        {
            return services.AddMvcBuilder(configuration, options =>
                 {
                     //if (mvcOption != null)
                     //{
                     //    mvcOption.Invoke(options);
                     //}
                     options.Filters.Add(new LogProcessAttribute());
                     options.Filters.Add(new ApiResultFilterAttribute());
                     options.Filters.Add(new ApiExceptionFilterAttribute());
                     options.Filters.Add(new ValidateInputAttribute());
                 }
#if !CORE31LATER
                , mvcJsonOption
#else
            , jsonOptions, mvcJsonOptions
#endif
                );
        }


        /// <summary>
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private static IServiceCollection AddCommonMvc(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddInternalController(configuration);
            services.AddSwaggerHiddenCheck(configuration);


#if NETCOREAPP3_1

#endif
            return services;
        }

        /// <summary>
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddInternalController(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient(typeof(InternalController));
            services.AddTransient(typeof(MonitorController));
            return services;
        }


        public static IMvcBuilder AppendJsonSettings(this IMvcBuilder mvcBuilder
#if !CORE31LATER
            , Action<MvcJsonOptions>? mvcJsonOption = null
#else
            , Action<JsonOptions>? jsonOptions = null, Action<MvcNewtonsoftJsonOptions>? mvcJsonOptions = null
#endif
        )
        {
#if CORE31LATER
            return mvcBuilder.AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                jsonOptions?.Invoke(options);
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                mvcJsonOptions?.Invoke(options);
            });
#else
            return mvcBuilder.AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                mvcJsonOption?.Invoke(options);
            });
#endif


        }

        public static IServiceCollection AddSwaggerHiddenCheck(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ISwaggerHiddenCheck, SwaggerHiddenCheckImpl>();
            return services;
        }


    }
}
