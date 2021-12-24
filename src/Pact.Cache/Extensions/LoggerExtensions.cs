using Microsoft.Extensions.Logging;

namespace Pact.Cache.Extensions;

/// <summary>
/// High-performance logging as cache is often a hot path
/// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/loggermessage?view=aspnetcore-6.0
/// </summary>
internal static class LoggerExtensions
{
    private static readonly Action<ILogger, string, Exception> _cacheOperationRequested;
    private static readonly Action<ILogger, string, Exception> _cacheOperationFailed;

    static LoggerExtensions()
    {
        _cacheOperationRequested = LoggerMessage.Define<string>(
            LogLevel.Debug,
            new EventId(1, nameof(CacheOperationRequested)),
            "Distributed Cache operation requested => {Key}");

        _cacheOperationFailed = LoggerMessage.Define<string>(
            LogLevel.Warning,
            new EventId(2, nameof(CacheOperationFailed)),
            "Distributed Cache operation failed => {Key}");
    }

    internal static void CacheOperationRequested(this ILogger<IDistributedCacheService> logger, string key)
    {
        _cacheOperationRequested(logger, key, null);
    }

    internal static void CacheOperationFailed(this ILogger<IDistributedCacheService> logger, string key, Exception ex)
    {
        _cacheOperationFailed(logger, key, ex);
    }
}