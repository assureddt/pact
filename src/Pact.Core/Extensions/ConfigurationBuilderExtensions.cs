using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;

namespace Pact.Core.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Inserts a named json file source to the configuration at the designated index
        /// </summary>
        /// <param name="config">The configuration builder</param>
        /// <param name="provider">The resolved file provider</param>
        /// <param name="filename">The filename</param>
        /// <param name="index">Where to insert (default 1)</param>
        /// <returns></returns>
        public static IConfigurationBuilder InsertJsonFileSource(this IConfigurationBuilder config, IFileProvider provider, string filename, int index = 1)
        {
            config.Sources.Insert(index, new JsonConfigurationSource { Path = filename, Optional = true, ReloadOnChange = false, FileProvider = provider });

            return config;
        }
    }
}
