using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Pact.Web.Extensions;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Convenience extension for Static Files with the Max Age Cache Control header set
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <param name="days">Number of days to set the Max Age with</param>
    /// <returns>The application builder for fluent usage</returns>
    public static IApplicationBuilder UseStaticFilesWithMaxAge(this IApplicationBuilder app, int days = 365)
        => app.UseStaticFilesWithMaxAge(TimeSpan.FromDays(days));

    /// <summary>
    /// Convenience extension for Static Files with the Max Age Cache Control header set
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <param name="timeSpan">TimeSpan to set the Max Age with</param>
    /// <returns>The application builder for fluent usage</returns>
    public static IApplicationBuilder UseStaticFilesWithMaxAge(this IApplicationBuilder app, TimeSpan timeSpan)
    {
        return app.UseStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = context =>
            {
                var headers = context.Context.Response.GetTypedHeaders();
                headers.CacheControl = new CacheControlHeaderValue
                {
                    MaxAge = timeSpan
                };
            }
        });
    }
}