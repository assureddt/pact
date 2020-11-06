using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Pact.Core.Extensions
{
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Adds "sharedsettings(.*).json" as the initial source of settings (intended to be a single origin in the project to reduce need for common settings in appsettings.json)
        /// </summary>
        /// <param name="builder">The host builder</param>
        /// <param name="filename">The name of the settings file (defaults to "sharedsettings.json" & "sharedsettings.{Env}.json")</param>
        /// <returns></returns>
        public static IHostBuilder ConfigureSharedSettings(this IHostBuilder builder, string filename = @"sharedsettings")
        {
            return builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                var env = hostingContext.HostingEnvironment;

                // as the files are "linked", not explicit, if running in docker, we won't be able to see them in the project directory
                // so we'll always use the copy that's in the bin folder instead (if we can). a null provider will just use the appcontext.basedirectory by default
                var assemblyLocation = Assembly.GetEntryAssembly()?.Location;
                var provider = assemblyLocation != null ? new PhysicalFileProvider(Path.GetDirectoryName(assemblyLocation)) : null;

                config.InsertJsonFileSource(provider, $"{filename}.{env.EnvironmentName}.json");
                config.InsertJsonFileSource(provider, $"{filename}.json");
            });
        }
    }
}
