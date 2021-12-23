using Serilog.AspNetCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Pact.Logging;

public static class LoggingExtensions
{
    /// <summary>
    /// Adds Serilog Request logging with some defaults and noise reduction for specified endpoints
    /// </summary>
    /// <param name="app"></param>
    /// <param name="noisyEndpointPatterns">Any specific endpoint addresses to log at verbose level only</param>
    public static IApplicationBuilder UseSerilogRequestLoggingWithPactDefaults(this IApplicationBuilder app, params string[] noisyEndpointPatterns)
    {
        app.UseSerilogRequestLogging(opts => opts.WithPactDefaults(noisyEndpointPatterns));

        return app;
    }

    /// <summary>
    /// Configures log enrichment with noise reduction for specified endpoints
    /// </summary>
    /// <param name="opts"></param>
    /// <param name="noisyEndpointPatterns"></param>
    public static void WithPactDefaults(this RequestLoggingOptions opts, string[] noisyEndpointPatterns = null)
    {
        opts.EnrichDiagnosticContext = EnrichFromContext;
        opts.GetLevel = (ctx, _, ex) =>
            ex != null
                ? LogEventLevel.Error
                : ctx.Response.StatusCode > 499
                    ? LogEventLevel.Error
                    : IsNoisyEndpoint(ctx, noisyEndpointPatterns)
                        ? LogEventLevel.Verbose
                        : LogEventLevel.Information;
    }

    /// <summary>
    /// HttpContext log enrichments used in most cases 
    /// </summary>
    /// <param name="diagnosticContext"></param>
    /// <param name="httpContext"></param>
    public static void EnrichFromContext(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        diagnosticContext.Set("IdentityName", httpContext?.User?.Identity?.Name);
        diagnosticContext.Set("RemoteIp", httpContext?.Connection?.RemoteIpAddress);

        if (httpContext?.Request?.Headers != null && httpContext.Request.Headers.ContainsKey(HeaderNames.UserAgent))
        {
            diagnosticContext.Set("Agent", httpContext.Request.Headers[HeaderNames.UserAgent]);
        }
    }

    /// <summary>
    /// FilterContext log enrichments used on action filters etc. 
    /// </summary>
    /// <param name="diagnosticContext"></param>
    /// <param name="context"></param>
    public static void EnrichFromFilterContext(IDiagnosticContext diagnosticContext, FilterContext context)
    {
        diagnosticContext.Set("RouteData", context?.ActionDescriptor?.RouteValues);
        diagnosticContext.Set("ActionName", context?.ActionDescriptor?.DisplayName);
        diagnosticContext.Set("ActionId", context?.ActionDescriptor?.Id);
        diagnosticContext.Set("ValidationState", context?.ModelState?.IsValid);
    }

    /// <summary>
    /// Checks if the endpoint matches any of the provided patterns
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="patterns"></param>
    /// <returns></returns>
    private static bool IsNoisyEndpoint(HttpContext ctx, string[] patterns)
    {
        if (patterns == null || !patterns.Any())
            return false;

        return patterns.Any(x => MatchesEndpointPattern(ctx, x));
    }

    /// <summary>
    /// Checks if the endpoint matches this pattern
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    private static bool MatchesEndpointPattern(HttpContext ctx, string name)
    {
        var endpoint = ctx.GetEndpoint();

        if (endpoint is not RouteEndpoint re) return false;

        return string.Equals(
            re.RoutePattern?.RawText,
            name,
            StringComparison.Ordinal);
    }

    private const string OriginalValuePrefix = "__";

    /// <summary>
    /// Retrieves a dictionary of the values of all public properties we may want to log from an object
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="filtered">By default, this is true, and removes any properties including "password" or "token" in their names</param>
    /// <returns></returns>
    public static Dictionary<string, object> GetLogPropertyDictionary(this object obj, bool filtered = true)
    {
        if (obj == null) return new Dictionary<string, object>();

        var type = obj.GetType().GetTypeInfo();
            
        return type.DeclaredProperties.Where(x => x.CanRead && !x.IsSpecialName && x.CanWrite && x.CustomAttributes.All(y => y.AttributeType != typeof(NotMappedAttribute)) &&
                                                  (!filtered || !(x.Name.ToLowerInvariant().Contains("password") || x.Name.ToLowerInvariant().Contains("token"))) &&
                                                  (x.PropertyType.IsPrimitive || x.PropertyType == typeof(string) || x.PropertyType == typeof(DateTime) || x.PropertyType == typeof(DateTime?) || x.PropertyType == typeof(int?) || x.PropertyType == typeof(bool?)))
            .ToDictionary(x => x.Name, x => x.GetValue(obj));
    }

    /// <summary>
    /// Retrieves a dictionary of all properties between the two objects and, where values differ, introduces a prefixed key for the original value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="amended"></param>
    /// <param name="original"></param>
    /// <param name="filtered">By default, this is true, and removes any properties including "password" or "token" in their names</param>
    /// <returns></returns>
    public static Dictionary<string, object> GetDifference<T>(this T amended, T original, bool filtered = true)
    {
        var originalProps = GetLogPropertyDictionary(original, filtered);

        return GetDifference(amended, originalProps);
    }

    /// <summary>
    /// Retrieves a dictionary of all properties between the object &amp; original property dictionary and, where values differ, introduces a prefixed key for the original value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="amended"></param>
    /// <param name="originalProps"></param>
    /// <param name="filtered">By default, this is true, and removes any properties including "password" or "token" in their names</param>
    /// <returns></returns>
    public static Dictionary<string, object> GetDifference<T>(this T amended, Dictionary<string, object> originalProps, bool filtered = true)
    {
        var amendedProps = GetLogPropertyDictionary(amended, filtered);
            
        return amendedProps.GetDifference(originalProps);
    }

    /// <summary>
    /// Retrieves a dictionary of all properties between the two dictionaries and, where values differ, introduces a prefixed key for the original value
    /// </summary>
    /// <param name="amendedProps"></param>
    /// <param name="originalProps"></param>
    /// <returns></returns>
    public static Dictionary<string, object> GetDifference(this Dictionary<string, object> amendedProps, Dictionary<string, object> originalProps)
    {
        var allKeys = originalProps.Keys.Union(amendedProps.Keys);

        var differences = new Dictionary<string, object>();
        foreach (var key in allKeys)
        {
            if (amendedProps.ContainsKey(key) && originalProps.ContainsKey(key))
            {
                var originalValue = originalProps[key];
                var amendedValue = amendedProps[key];

                if (originalValue?.ToString() == amendedValue?.ToString())
                {
                    if (key.Equals("id", StringComparison.OrdinalIgnoreCase))
                        differences.Add(key, originalValue);

                    continue;
                }

                differences.Add(key, amendedValue);
                differences.Add($"{OriginalValuePrefix}{key}", originalValue);
            }
            else if (amendedProps.ContainsKey(key))
            {
                differences.Add(key, amendedProps[key]);
            }
            else differences.Add(key, originalProps[key]);
        }

        return differences;
    }

    /// <summary>
    /// Logs all properties on the objects and, where differences exist, denotes the original value with a prefixed key
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="logger"></param>
    /// <param name="original"></param>
    /// <param name="updated"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void LogDifference<T>(this ILogger logger, Dictionary<string, object> original, T updated, string message, params object[] args)
    {
        try
        {
            var difference = updated.GetDifference(original);

            if (difference.Keys.All(x => !x.StartsWith(OriginalValuePrefix)))
            {
                logger.LogDebug(message, args);

                return;
            }

            using (logger.BeginScope(difference))
            {
                logger.LogInformation(message, args);
            }
        }
        catch (Exception exc)
        {
            try
            {
                logger.LogWarning(exc, $"Failed to compile Log Difference for: {message}", args);
            }
            catch
            {
                // ignored
            }
        }
    }

    /// <summary>
    /// Helper to get the basic calling method name
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="memberName"></param>
    /// <returns></returns>
    public static string MethodName(this object _, [CallerMemberName] string memberName = "") => memberName;

    /// <summary>
    /// Helper to get the fully qualified calling method name
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="memberName"></param>
    /// <returns></returns>
    public static string FullMethodName(this object obj, [CallerMemberName] string memberName = "") => obj.GetType().FullName + "." + memberName;
}