using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Pact.Localization
{
    /// <summary>
    /// Enables automatic setting of the culture for <see cref="HttpRequest"/>s based on information
    /// sent by the client in headers and logic provided by the application.
    /// </summary>
    public class DynamicLocalizationMiddleware
    {
        private const int MaxCultureFallbackDepth = 5;

        private readonly RequestDelegate _next;
        private readonly RequestLocalizationOptions _options;
        private ILogger _logger;

        /// <summary>
        /// This implementation is derived from the built-in <see cref="RequestLocalizationMiddleware"/>.
        /// Rather than adopting a static "Supported Cultures" list from the options, we'll instead be picking up the available cultures based on the request
        /// You'll need to inject a service based on the ISupportedCulturesResolver interface that will be provide that logic
        /// NOTE: probably want this placed towards the bottom of the middleware stack to ensure the HttpContext is furnished with an Identity by the time it arrives
        /// </summary>
        /// <param name="next">The <see cref="RequestDelegate"/> representing the next middleware in the pipeline.</param>
        /// <param name="options">The <see cref="RequestLocalizationOptions"/> representing the options for the
        /// <see cref="RequestLocalizationOptions"/>.</param>
        public DynamicLocalizationMiddleware(RequestDelegate next, IOptions<RequestLocalizationOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options.Value;
        }

        /// <summary>
        /// Invokes the logic of the middleware.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <returns>A <see cref="Task"/> that completes when the middleware has completed processing.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var requestCulture = _options.DefaultRequestCulture;

            IRequestCultureProvider winningProvider = null;

            var supportedCultures = await context.RequestServices.GetService<ISupportedCulturesResolver>().GetSupportedCulturesAsync(context);

            if (_options.RequestCultureProviders != null)
            {
                foreach (var provider in _options.RequestCultureProviders)
                {
                    var providerResultCulture = await provider.DetermineProviderCultureResult(context);
                    if (providerResultCulture == null)
                    {
                        continue;
                    }
                    var cultures = providerResultCulture.Cultures;
                    var uiCultures = providerResultCulture.UICultures;

                    CultureInfo cultureInfo = null;
                    CultureInfo uiCultureInfo = null;
                    
                    if (supportedCultures != null)
                    {
                        cultureInfo = GetCultureInfo(
                            cultures,
                            supportedCultures,
                            _options.FallBackToParentCultures);

                        if (cultureInfo == null)
                        {
                            EnsureLogger(context);
                            _logger?.LogDebug("Unsupported Cultures: {Provider} => {Cultures}", provider.GetType().Name, cultures);
                        }
                    }

                    if (supportedCultures != null)
                    {
                        uiCultureInfo = GetCultureInfo(
                            uiCultures,
                            supportedCultures,
                            _options.FallBackToParentUICultures);

                        if (uiCultureInfo == null)
                        {
                            EnsureLogger(context);
                            _logger?.LogDebug("Unsupported UI Cultures: {Provider} => {Cultures}", provider.GetType().Name, uiCultures);
                        }
                    }

                    if (cultureInfo == null && uiCultureInfo == null)
                    {
                        continue;
                    }

                    if (cultureInfo == null)
                    {
                        cultureInfo = _options.DefaultRequestCulture.Culture;
                    }

                    if (cultureInfo != null && uiCultureInfo == null)
                    {
                        uiCultureInfo = _options.DefaultRequestCulture.UICulture;
                    }

                    var result = new RequestCulture(cultureInfo, uiCultureInfo);

                    requestCulture = result;
                    winningProvider = provider;
                    break;
                }
            }

            context.Features.Set<IRequestCultureFeature>(new RequestCultureFeature(requestCulture, winningProvider));

            SetCurrentThreadCulture(requestCulture);

            await _next(context);
        }

        private void EnsureLogger(HttpContext context)
        {
            _logger ??= context.RequestServices.GetService<ILogger<DynamicLocalizationMiddleware>>();
        }

        private static void SetCurrentThreadCulture(RequestCulture requestCulture)
        {
            CultureInfo.CurrentCulture = requestCulture.Culture;
            CultureInfo.CurrentUICulture = requestCulture.UICulture;
        }

        private static CultureInfo GetCultureInfo(
            IList<StringSegment> cultureNames,
            IList<CultureInfo> supportedCultures,
            bool fallbackToParentCultures)
        {
            foreach (var cultureName in cultureNames)
            {
                // Allow empty string values as they map to InvariantCulture, whereas null culture values will throw in
                // the CultureInfo ctor
                if (cultureName == null) continue;

                var cultureInfo = GetCultureInfo(cultureName, supportedCultures, fallbackToParentCultures, currentDepth: 0);
                if (cultureInfo != null)
                {
                    return cultureInfo;
                }
            }

            return null;
        }

        private static CultureInfo GetCultureInfo(StringSegment name, IList<CultureInfo> supportedCultures)
        {
            // Allow only known culture names as this API is called with input from users (HTTP requests) and
            // creating CultureInfo objects is expensive and we don't want it to throw either.
            if (supportedCultures == null)
                return null;

            var culture = supportedCultures.FirstOrDefault(
                supportedCulture => StringSegment.Equals(supportedCulture.Name, name, StringComparison.OrdinalIgnoreCase));

            return culture == null ? null : CultureInfo.ReadOnly(culture);
        }

        private static CultureInfo GetCultureInfo(
            StringSegment cultureName,
            IList<CultureInfo> supportedCultures,
            bool fallbackToParentCultures,
            int currentDepth)
        {
            var culture = GetCultureInfo(cultureName, supportedCultures);

            if (culture != null || !fallbackToParentCultures || currentDepth >= MaxCultureFallbackDepth) return culture;

            var lastIndexOfHyphen = cultureName.LastIndexOf('-');

            if (lastIndexOfHyphen <= 0) return null;

            // Trim the trailing section from the culture name, e.g. "fr-FR" becomes "fr"
            var parentCultureName = cultureName.Subsegment(0, lastIndexOfHyphen);

            culture = GetCultureInfo(parentCultureName, supportedCultures, true, currentDepth + 1);

            return culture;
        }
    }
}