using System.ComponentModel;
using Serilog.AspNetCore;
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
        diagnosticContext.Set("IdentityName", httpContext?.User.Identity?.Name);
        diagnosticContext.Set("RemoteIp", httpContext?.Connection.RemoteIpAddress);

        if (httpContext?.Request.Headers != null && httpContext.Request.Headers.ContainsKey(HeaderNames.UserAgent))
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
        diagnosticContext.Set("RouteData", context?.ActionDescriptor.RouteValues);
        diagnosticContext.Set("ActionName", context?.ActionDescriptor.DisplayName);
        diagnosticContext.Set("ActionId", context?.ActionDescriptor.Id);
        diagnosticContext.Set("ValidationState", context?.ModelState.IsValid);
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
            re.RoutePattern.RawText,
            name,
            StringComparison.Ordinal);
    }

    private static readonly string[] DefaultFilterTerms = {"password", "token"};

    public static Dictionary<string, object> GetLogPropertyDictionary(this object obj) => GetLogPropertyDictionary(obj, DefaultFilterTerms);

    /// <summary>
    /// Retrieves a dictionary of the values of all public properties we may want to log from an object
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="filterTerms">By default, this filters any props containing the terms "password" or "token" if no override is provided</param>
    /// <returns></returns>
    public static Dictionary<string, object> GetLogPropertyDictionary(this object obj, params string[] filterTerms)
    {
        if (obj == null) return new Dictionary<string, object>();

        var props = TypeDescriptor.GetProperties(obj);
        var dict = new Dictionary<string, object>();
        
        foreach (var x in props.Cast<PropertyDescriptor>().Where(x => IsSupportedLogProperty(x.PropertyType)))
        {
            dict.TryAdd(x.Name,
                filterTerms.Any(y => x.Name.Contains(y, StringComparison.InvariantCultureIgnoreCase))
                    ? "[Redacted]"
                    : x.GetValue(obj));
        }

        return dict;
    }

    internal static bool IsNullable<T>(this T obj, out Type underlying)
    {
        underlying = null;

        if (obj == null)
            return true;

        var type = typeof(T);
        if (!type.IsValueType)
            return true;

        underlying = Nullable.GetUnderlyingType(type);
        return underlying != null;
    }

    internal static bool IsSupportedLogProperty<T>(this T _) => typeof(T).IsSupportedLogProperty();

    internal static bool IsSupportedLogProperty(this Type type)
    {
        if (type == typeof(string) || type.IsValueType) 
            return true;

        return type.IsNullable(out var underlying) && underlying?.IsSupportedLogProperty() == true;
    }

    /// <summary>
    /// Retrieves a dictionary of all properties between the two objects and, where values differ
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="amended"></param>
    /// <param name="original"></param>
    /// <returns></returns>
    public static Dictionary<string, ObjectChange> GetDifference<T>(this T amended, T original)
    {
        var originalProps = GetLogPropertyDictionary(original);

        return GetDifference(amended, originalProps);
    }

    /// <summary>
    /// Retrieves a dictionary of all properties between the two objects and, where values differ
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="amended"></param>
    /// <param name="original"></param>
    /// <param name="filterTerms">By default, this filters any props containing the terms "password" or "token" if no override is provided</param>
    /// <returns></returns>
    public static Dictionary<string, ObjectChange> GetDifference<T>(this T amended, T original, params string[] filterTerms)
    {
        var originalProps = GetLogPropertyDictionary(original, filterTerms);

        return GetDifference(amended, originalProps, filterTerms);
    }

    /// <summary>
    /// Retrieves a dictionary of all properties in the object (with no original state passed, this will present as all new values)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="amended"></param>
    /// <returns></returns>
    public static Dictionary<string, ObjectChange> GetDifference<T>(this T amended)
    {
        return GetDifference(amended, null, DefaultFilterTerms);
    }

    /// <summary>
    /// Retrieves a dictionary of all properties in the object (with no original state passed, this will present as all new values)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="amended"></param>
    /// <param name="filterTerms">By default, this filters any props containing the terms "password" or "token" if no override is provided</param>
    /// <returns></returns>
    public static Dictionary<string, ObjectChange> GetDifference<T>(this T amended, params string[] filterTerms)
    {
        return GetDifference(amended, null, filterTerms);
    }

    /// <summary>
    /// Retrieves a dictionary of all properties between the object &amp; original property dictionary and, where values differ
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="amended"></param>
    /// <param name="originalProps"></param>
    /// <returns></returns>
    public static Dictionary<string, ObjectChange> GetDifference<T>(this T amended, Dictionary<string, object> originalProps)
    {
        var amendedProps = GetLogPropertyDictionary(amended);
            
        return amendedProps.GetDifference(originalProps);
    }

    /// <summary>
    /// Retrieves a dictionary of all properties between the object &amp; original property dictionary and, where values differ
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="amended"></param>
    /// <param name="originalProps"></param>
    /// <param name="filterTerms">By default, this filters any props containing the terms "password" or "token" if no override is provided</param>
    /// <returns></returns>
    public static Dictionary<string, ObjectChange> GetDifference<T>(this T amended, Dictionary<string, object> originalProps, params string[] filterTerms)
    {
        var amendedProps = GetLogPropertyDictionary(amended, filterTerms);
            
        return amendedProps.GetDifference(originalProps);
    }

    /// <summary>
    /// Retrieves a dictionary of all properties between the two dictionaries and, where values differ
    /// </summary>
    /// <param name="amendedProps"></param>
    /// <param name="originalProps">Accepts null if no original state exists</param>
    /// <returns></returns>
    public static Dictionary<string, ObjectChange> GetDifference(this Dictionary<string, object> amendedProps, Dictionary<string, object> originalProps)
    {
        var allKeys = originalProps?.Keys.Union(amendedProps.Keys) ?? amendedProps.Keys;

        var differences = new Dictionary<string, ObjectChange>();
        foreach (var key in allKeys)
        {
            if (amendedProps.ContainsKey(key) && originalProps?.ContainsKey(key) == true)
            {
                var originalValue = originalProps[key];
                var amendedValue = amendedProps[key];

                if (originalValue?.ToString() == amendedValue?.ToString())
                {
                    if (key.Equals("id", StringComparison.OrdinalIgnoreCase))
                        differences.Add(key, new ObjectChange(originalValue));

                    continue;
                }

                differences.Add(key, new ObjectChange(originalValue, amendedValue, ObjectChange.ChangeType.Edit));
            }
            else if (amendedProps.ContainsKey(key))
            {
                differences.Add(key, new ObjectChange(null, amendedProps[key], ObjectChange.ChangeType.New));
            }
            else if (originalProps?.ContainsKey(key) == true)
            {
                differences.Add(key, new ObjectChange(originalProps[key], null, ObjectChange.ChangeType.Removed));
            }
        }

        return differences;
    }

    /// <summary>
    /// Retrieves a dictionary of all changes (but with no origin, these will present as all new values)
    /// </summary>
    /// <param name="amendedProps"></param>
    /// <returns></returns>
    public static Dictionary<string, ObjectChange> GetDifference(this Dictionary<string, object> amendedProps)
    {
        return amendedProps.GetDifference((Dictionary<string, object>)null);
    }

    /// <summary>
    /// Shorthand for adding an object to a log scope
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IDisposable BeginScope(this ILogger logger, string key, object value)
    {
        return logger.BeginScope(new Dictionary<string, object> { { key, value } });
    }

    /// <summary>
    /// Logs all properties on the objects and, where differences exist, denotes the original value with a prefixed key
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="logger"></param>
    /// <param name="level"></param>
    /// <param name="original"></param>
    /// <param name="updated"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void LogDifference<T>(
        this ILogger logger,
        LogLevel level,
        Dictionary<string, object> original,
        T updated,
        string message,
        params object[] args)
    {
        try
        {
            var difference = updated.GetDifference(original);
            if (difference.Values.All(x => x.Change == ObjectChange.ChangeType.None))
            {
                logger.LogDebug(message, args);
            }
            else
            {
                using (logger.BeginScope("@ObjectChanges", difference))
                    logger.Log(level, message, args);
            }
        }
        catch (Exception ex)
        {
            try
            {
                logger.LogWarning(ex, "Failed to compile Log Difference for: " + message, args);
            }
            catch
            {
                // ignored
            }
        }
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
    public static void LogDifference<T>(
        this ILogger logger,
        Dictionary<string, object> original,
        T updated,
        string message,
        params object[] args)
    {
        logger.LogDifference(LogLevel.Information, original, updated, message, args);
    }

    /// <summary>
    /// Logs all properties on the objects, with no original state provided, to indicate a new object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="logger"></param>
    /// <param name="level"></param>
    /// <param name="updated"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void LogDifference<T>(
        this ILogger logger,
        LogLevel level,
        T updated,
        string message,
        params object[] args)
    {
        logger.LogDifference(level, null, updated, message, args);
    }

    /// <summary>
    /// Logs all properties on the objects, with no original state provided, to indicate a new object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="logger"></param>
    /// <param name="updated"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void LogDifference<T>(
        this ILogger logger,
        T updated,
        string message,
        params object[] args)
    {
        logger.LogDifference(null, updated, message, args);
    }

    /// <summary>
    /// Helper to get the basic calling method name
    /// </summary>
    /// <param name="_"></param>
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