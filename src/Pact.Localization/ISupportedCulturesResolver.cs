using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Pact.Localization;

/// <summary>
/// Used by <see cref="DynamicLocalizationMiddleware"/> to allow the application to define how the set of supported cultures are resolved for an HttpContext
/// </summary>
public interface ISupportedCulturesResolver
{
    /// <summary>
    /// Determines the list of cultures available for selection for a context (likely based on the authenticated user)
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Task<IList<CultureInfo>> GetSupportedCulturesAsync(HttpContext context);
}