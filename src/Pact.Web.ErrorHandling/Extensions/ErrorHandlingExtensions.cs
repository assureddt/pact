using Microsoft.AspNetCore.Builder;

namespace Pact.Web.ErrorHandling.Extensions;

public static class ErrorHandlingExtensions
{
    /// <summary>
    /// Adds opinionated Razor Pages automated error handling functionality
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UsePactErrorHandling(this IApplicationBuilder builder)
    {
        builder.UseStatusCodePagesWithReExecute("/Error/{0}");
        builder.UseExceptionHandler("/Error");

        return builder;
    }
}