using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Pact.Core.Extensions
{
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Adds "sharedsettings(.*).json" as the initial source of settings (intended to be a single origin in the project to reduce need for common settings in appsettings.json)
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder ConfigureSharedSettings(this IHostBuilder builder, string [] args)
        {
            return builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                var env = hostingContext.HostingEnvironment;

                // NOTE: docs used to suggest using clear, but there's a chained configuration provider that gets added first earlier on
                // we can't re-add that and it breaks everything, so we'll just insert these shared sources directly above the appsettings.json already added
                foreach (var provider in GetSharedFileProviders(env))
                {
                    config.Sources.Insert(1, new JsonConfigurationSource { Path = $"sharedsettings.{env.EnvironmentName}.json", Optional = true, ReloadOnChange = false, FileProvider = provider });
                    config.Sources.Insert(1, new JsonConfigurationSource { Path = "sharedsettings.json", Optional = true, ReloadOnChange = false, FileProvider = provider });
                }
            });
        }

        private static IEnumerable<IFileProvider> GetSharedFileProviders(IHostEnvironment env)
        {
            // as only one of the variations needs to work (and some will fail) dependent on the runtime (IISExpress, Docker, Published) environment, we add with failover
            // the default (content root: bin folder for published scenarios)
            var fps = new List<IFileProvider> {null};

            try
            {
                // repo root where the raw files are (for local debug)
                fps.Add(new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "..", "..")));
            }
            catch
            {
                // ignored
            }

            try
            { 
                // mapped volume source for docker debug
                fps.Add(new PhysicalFileProvider("/src"));
            }
            catch
            {
                // ignored
            }

            return fps;
        }
    }
}
