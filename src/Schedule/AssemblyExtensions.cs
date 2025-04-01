using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WindNight.Core.Abstractions;
using WindNight.Core.Attributes.Abstractions;

namespace Schedule
{
    public static class AssemblyExtensions
    {

        public static IServiceCollection AutoAddJobs(this IServiceCollection services, IConfiguration configuration)
        {
            var typeList = AppDomain.CurrentDomain.GetAssemblies()
                   .SelectMany(x => x.GetTypes())
                   .Where(m => m is
                   {
                       IsClass: true,
                       IsAbstract: false,
                       IsInterface: false,
                       IsGenericType: false,
                   }).ToList();

            foreach (var currentType in typeList)
            {
                //var hasAlias = currentType.GetCustomAttribute<AliasAttribute>() is not null;
                // var hasAlias = Attribute.IsDefined(currentType, typeof(AliasAttribute));
                var hasAlias = currentType.IsDefined<AliasAttribute>();
                var hasAlias2 = currentType.HasAttribute<AliasAttribute>();
                if (hasAlias || hasAlias2)
                {
                    var isEnJob = typeof(IJobBase).IsAssignableFrom(currentType);
                    if (isEnJob)
                    {
                        services.AddTransient(typeof(IJobBase), currentType);
                    }
                }
                //var isEnJob = typeof(IJobBase).IsAssignableFrom(currentType);
                //if (hasAlias && isEnJob)
                //{
                //    services.AddTransient(typeof(IJobBase), currentType);
                //}
                //else
                //{
                //    continue;
                //}
            }

            return services;
        }

        //public static IServiceCollection AutoAddJobs2(IServiceCollection services)
        //{

        //    var currentAppDir = Environment.CurrentDirectory;
        //    var dllFiles = Directory.GetFiles(currentAppDir)
        //       .Where(m =>
        //           ".dll".Equals(Path.GetExtension(m), StringComparison.OrdinalIgnoreCase)
        //           );

        //    var jobDlls = dllFiles.Where(m =>
        //        Path.GetFileNameWithoutExtension(m).Contains("Jobs", StringComparison.OrdinalIgnoreCase)
        //        );

        //    foreach (var file in jobDlls)
        //    {
        //        var assembly = Assembly.LoadFile(file);
        //        var types = assembly.GetTypes()
        //            .Where(m => m is
        //            {
        //                IsInterface: false,
        //                IsGenericType: false,
        //                IsAbstract: false,
        //                IsClass: true,
        //            }
        //            )
        //            .ToList();

        //        //Type jobInterface;
        //        //var jobInterfaces = assembly.GetTypes()
        //        //    .Where(t => t.Name == "IEnJob" && t.IsInterface).ToList();
        //        //if (jobInterfaces.Count() > 1)
        //        //{
        //        //    jobInterface = jobInterfaces.FirstOrDefault();
        //        //}
        //        //else
        //        //{
        //        //    jobInterface = jobInterfaces.FirstOrDefault();
        //        //}
        //        //if (jobInterface == null)
        //        //{
        //        //    continue;
        //        //}
        //        //if (jobInterface == typeof(IEnJob))
        //        //{
        //        //}

        //        foreach (var currentType in types)
        //        {
        //            if (typeof(IEnJob).IsAssignableFrom(currentType))
        //            {
        //                services.AddTransient(typeof(IEnJob), currentType);
        //            }
        //            else
        //            {
        //                continue;
        //            }
        //            var interfaces = currentType.GetInterfaces().ToList();
        //            if (interfaces.IsNullOrEmpty())
        //            {
        //                continue;
        //            }

        //            foreach (var currentInterface in interfaces)
        //            {
        //                if (typeof(IEnJob).IsAssignableFrom(currentType))
        //                {
        //                    services.AddTransient(currentInterface, currentType);

        //                }
        //                else
        //                {
        //                    services.AddTransient(currentInterface, currentType);

        //                }
        //                //if (currentInterface.Name.Equals("IEnJob", StringComparison.OrdinalIgnoreCase))
        //                //{
        //                //    services.AddTransient(currentInterface, currentType);
        //                //}
        //                //var isEnJob = jobInterface.IsAssignableFrom(currentInterface) ||
        //                //              (currentInterface.IsGenericType &&
        //                //               currentInterface.GetGenericTypeDefinition() == jobInterface);
        //                //var enJobs = jobInterface.IsAssignableFrom(currentType);
        //                //var enJobs1 = jobInterface.IsAssignableFrom(currentInterface);
        //                //var type11s = AppDomain.CurrentDomain.GetAssemblies()
        //                //    .SelectMany(x => x.GetTypes())
        //                //    .Where(t => t.Name == "IEnJob" && t.IsInterface).ToList();
        //                //if (enJobs)
        //                //{
        //                //    services.AddTransient(jobInterface, currentType);

        //                //}
        //                //if (typeof(IEnJob) == currentInterface || currentInterface.IsGenericType && currentInterface.GetGenericTypeDefinition() == typeof(IEnJob))
        //                //{
        //                //    services.AddTransient(currentInterface, currentType);
        //                //}

        //            }


        //        }




        //    }



        //    return services;

        //}

    }
}
