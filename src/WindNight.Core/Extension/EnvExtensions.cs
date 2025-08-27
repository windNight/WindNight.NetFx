using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace WindNight.Extensions.Hosting
{
    public static class HostEnvironmentEnvExtensions
    {
        /// <summary>
        /// Compares the current host environment name against the specified value.
        /// </summary>
        /// <param name="hostEnvironment">An instance of <see cref="T:Microsoft.Extensions.Hosting.IHostEnvironment" />.</param>
        /// <param name="envName">Environment name to validate against.</param>
        /// <returns><see langword="true" /> if the specified name is the same as the current environment, otherwise <see langword="false" />.</returns>
        public static bool IsEnvName(this IHostEnvironment hostEnvironment, string toCheckEnvName, bool defaultValue = false)
        {
            var envName = hostEnvironment?.EnvironmentName ?? "";
            if (envName.IsNotNullOrEmpty())
            {
                return defaultValue;
            }

            var flag = envName.Equals(toCheckEnvName, StringComparison.OrdinalIgnoreCase);
            if (flag)
            {
                return true;
            }
            if (envName.StartsWith(toCheckEnvName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}
