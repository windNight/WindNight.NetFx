#if !NET45
using System;
using WindNight.Core.NetCore.@internal;

namespace Microsoft.Extensions.Configuration.Json
{
    public static class JsonConfigExtensions
    {
        /// <summary>
        ///     Adds a JSON configuration source to <paramref name="builder" />.
        /// </summary>
        /// <param name="builder">The <see cref="T:Microsoft.Extensions.Configuration.IConfigurationBuilder" /> to add to.</param>
        /// <param name="obj">The <see cref="T:System.Object" /> to read the json configuration data from.</param>
        /// <returns>The <see cref="T:Microsoft.Extensions.Configuration.IConfigurationBuilder" />.</returns>
        public static IConfigurationBuilder AddJsonObject(
            this IConfigurationBuilder builder,
            object obj)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            return builder.Add<JsonObjectConfigurationSource>(s => s.Object = obj);
        }
    }
}
#endif
