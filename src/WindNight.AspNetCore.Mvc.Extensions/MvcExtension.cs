#if NETCOREAPP3_1||NET5_0
using System.Text.Encodings.Web;
using System.Text.Unicode;

#endif
using System;
using Microsoft.AspNetCore.Mvc.Filters.Extensions;
using Microsoft.AspNetCore.Mvc.WnExtensions.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace Microsoft.AspNetCore.Mvc.WnExtensions
{
    public static class MvcExtension
    {
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
        public static IMvcBuilder AddMvcBuilder(this IServiceCollection services, Action<MvcOptions> mvcOption
#if !NETCOREAPP3_1&&!NET5_0
            , Action<MvcJsonOptions>? mvcJsonOption = null
#else
            , Action<JsonOptions>? jsonOptions = null, Action<MvcNewtonsoftJsonOptions>? mvcJsonOptions = null
#endif
            )
        {
            AddCommonMvc(services);

#if NETCOREAPP3_1||NET5_0
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
        public static IMvcBuilder AddMvcBuilderWithDefaultFilters(this IServiceCollection services
#if !NETCOREAPP3_1&&!NET5_0
            , Action<MvcJsonOptions>? mvcJsonOption = null
#else
            , Action<JsonOptions>? jsonOptions = null, Action<MvcNewtonsoftJsonOptions>? mvcJsonOptions = null
#endif
        )
        {
            return services.AddMvcBuilder(options =>
                 {
                     options.Filters.Add(new ApiResultFilterAttribute());
                     options.Filters.Add(new ApiExceptionFilterAttribute());
                     options.Filters.Add(new ValidateInputAttribute());
                     options.Filters.Add(new LogProcessAttribute());
                 }
#if !NETCOREAPP3_1&&!NET5_0
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
        private static IServiceCollection AddCommonMvc(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddInternalController();

#if NETCOREAPP3_1

#endif
            return services;
        }

        /// <summary>
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddInternalController(this IServiceCollection services)
        {
            services.AddTransient(typeof(InternalController));
            services.AddTransient(typeof(MonitorController));
            return services;
        }


        public static IMvcBuilder AppendJsonSettings(this IMvcBuilder mvcBuilder
#if !NETCOREAPP3_1&&!NET5_0
            , Action<MvcJsonOptions>? mvcJsonOption = null
#else
            , Action<JsonOptions>? jsonOptions = null, Action<MvcNewtonsoftJsonOptions>? mvcJsonOptions = null
#endif
        )
        {
#if NETCOREAPP3_1||NET5_0
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

    }
}