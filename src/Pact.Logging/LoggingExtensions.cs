using Serilog.AspNetCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Pact.Logging
{
    public static class LoggingExtensions
    {
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

        private static void EnrichFromContext(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            diagnosticContext.Set("IdentityName", httpContext?.User?.Identity?.Name);
            diagnosticContext.Set("RemoteIp", httpContext?.Connection?.RemoteIpAddress);

            if (httpContext?.Request?.Headers != null && httpContext.Request.Headers.ContainsKey(HeaderNames.UserAgent))
            {
                diagnosticContext.Set("Agent", httpContext.Request.Headers[HeaderNames.UserAgent]);
            }
        }

        public static void EnrichFromFilterContext(IDiagnosticContext diagnosticContext, FilterContext context)
        {
            diagnosticContext.Set("RouteData", context?.ActionDescriptor?.RouteValues);
            diagnosticContext.Set("ActionName", context?.ActionDescriptor?.DisplayName);
            diagnosticContext.Set("ActionId", context?.ActionDescriptor?.Id);
            diagnosticContext.Set("ValidationState", context?.ModelState?.IsValid);
        }

        private static bool IsNoisyEndpoint(HttpContext ctx, string[] patterns)
        {
            if (patterns == null || !patterns.Any())
                return false;

            return patterns.Any(x => MatchesEndpointPattern(ctx, x));
        }

        private static bool MatchesEndpointPattern(HttpContext ctx, string name)
        {
            var endpoint = ctx.GetEndpoint();

            if (!(endpoint is RouteEndpoint re)) return false;

            return string.Equals(
                re.RoutePattern?.RawText,
                name,
                StringComparison.Ordinal);
        }

        private const string OriginalValuePrefix = "__";

        public static Dictionary<string, object> GetLogPropertyDictionary(this object obj, bool filtered = true)
        {
            if (obj == null) return new Dictionary<string, object>();

            var type = obj.GetType().GetTypeInfo();
            
            return type.DeclaredProperties.Where(x => x.CanRead && !x.IsSpecialName && x.CanWrite && x.CustomAttributes.All(y => y.AttributeType != typeof(NotMappedAttribute)) &&
                (!filtered || !(x.Name.ToLowerInvariant().Contains("password") || x.Name.ToLowerInvariant().Contains("token"))) &&
                (x.PropertyType.IsPrimitive || x.PropertyType == typeof(string) || x.PropertyType == typeof(DateTime) || x.PropertyType == typeof(DateTime?) || x.PropertyType == typeof(int?) || x.PropertyType == typeof(bool?)))
                .ToDictionary(x => x.Name, x => x.GetValue(obj));
        }

        public static Dictionary<string, object> GetDifference<T>(this T amended, T original, bool filtered = true)
        {
            var originalProps = GetLogPropertyDictionary(original, filtered);

            return GetDifference(amended, originalProps);
        }

        public static Dictionary<string, object> GetDifference<T>(this T amended, Dictionary<string, object> originalProps, bool filtered = true)
        {
            var amendedProps = GetLogPropertyDictionary(amended, filtered);
            
            return amendedProps.GetDifference(originalProps);
        }

        public static Dictionary<string, object> GetDifference(this Dictionary<string, object> amendedProps, Dictionary<string, object> originalProps, bool filtered = true)
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

        public static string MethodName(this object obj, [CallerMemberName] string memberName = "") => memberName;

        public static string FullMethodName(this object obj, [CallerMemberName] string memberName = "") => obj.GetType().FullName + "." + memberName;
    }
}
