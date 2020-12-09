using System;
using Microsoft.AspNetCore.Mvc;
using Pact.Web.Filters;

namespace Pact.Web.Extensions
{
    public static class MvcOptionsExtensions
    {
        /// <summary>
        /// Adds log-enrichment filters for razor pages & controller actions (used in conjunction with Pact.Logging extensions
        /// </summary>
        /// <param name="options">The <see cref="MvcOptions"/>.</param>
        /// <returns>The <see cref="MvcOptions"/>.</returns>
        public static MvcOptions AddLogEnrichmentFilters(this MvcOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.Filters.Add<LoggingActionFilter>();
            options.Filters.Add<LoggingPageFilter>();

            return options;
        }
    }
}
