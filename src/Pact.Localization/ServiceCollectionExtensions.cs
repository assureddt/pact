using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace Pact.Localization
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="DynamicLocalizationMiddleware"/> to automatically set culture information for
        /// requests based on information provided by the client.
        /// NOTE: probably want this placed towards the bottom of the middleware stack to ensure the HttpContext is furnished with an Identity by the time it arrives
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseDynamicRequestLocalization(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<DynamicLocalizationMiddleware>();
        }

        /// <summary>
        /// Adds the <see cref="DynamicLocalizationMiddleware"/> to automatically set culture information for
        /// requests based on information provided by the client.
        /// NOTE: probably want this placed towards the bottom of the middleware stack to ensure the HttpContext is furnished with an Identity by the time it arrives
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="options">The <see cref="RequestLocalizationOptions"/> to configure the middleware with.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseDynamicRequestLocalization(
            this IApplicationBuilder app,
            RequestLocalizationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return app.UseMiddleware<DynamicLocalizationMiddleware>(Options.Create(options));
        }

        /// <summary>
        /// Adds the <see cref="DynamicLocalizationMiddleware"/> to automatically set culture information for
        /// requests based on information provided by the client.
        /// NOTE: probably want this placed towards the bottom of the middleware stack to ensure the HttpContext is furnished with an Identity by the time it arrives
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="optionsAction"></param>
        /// <remarks>
        /// This will going to instantiate a new <see cref="RequestLocalizationOptions"/> that doesn't come from the services.
        /// </remarks>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseDynamicRequestLocalization(
            this IApplicationBuilder app,
            Action<RequestLocalizationOptions> optionsAction)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (optionsAction == null)
            {
                throw new ArgumentNullException(nameof(optionsAction));
            }

            var options = new RequestLocalizationOptions();
            optionsAction.Invoke(options);

            return app.UseMiddleware<DynamicLocalizationMiddleware>(Options.Create(options));
        }
    }
}
