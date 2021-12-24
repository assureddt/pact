using Microsoft.Extensions.Logging;

namespace Pact.Cache.Extensions;

/// <summary>
/// High-performance logging as cache is often a hot path
/// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/loggermessage?view=aspnetcore-6.0
/// </summary>
internal static class LoggerExtensions
{
    private static readonly Action<ILogger, CacheOperation, string, Exception> _cacheOperationRequested;
    private static readonly Action<ILogger, CacheOperation, string, Exception> _cacheOperationFailed;
    private static readonly Action<ILogger, string, Exception> _memoryCacheHit;

    static LoggerExtensions()
    {
        _cacheOperationRequested = LoggerMessage.Define<CacheOperation, string>(
            LogLevel.Trace,
            new EventId(1, nameof(CacheOperationRequested)),
            "Distributed Cache operation requested ({Operation}) => {Key}");

        _cacheOperationFailed = LoggerMessage.Define<CacheOperation, string>(
            LogLevel.Warning,
            new EventId(2, nameof(CacheOperationFailed)),
            "Distributed Cache operation failed ({Operation}) => {Key}");

        _memoryCacheHit = LoggerMessage.Define<string>(
            LogLevel.Trace,
            new EventId(3, nameof(MemoryCacheHit)),
            "Distributed Memory Cache Hit => {Key}");
    }

    internal static void CacheOperationRequested(this ILogger<IDistributedCacheService> logger, CacheOperation operation, string key)
    {
        _cacheOperationRequested(logger, operation, key, null);
    }

    internal static void CacheOperationFailed(this ILogger<IDistributedCacheService> logger, CacheOperation operation, string key, Exception ex)
    {
        _cacheOperationFailed(logger, operation, key, ex);
    }

    internal static void MemoryCacheHit(this ILogger<IDistributedCacheService> logger, string key)
    {
        _memoryCacheHit(logger, key, null);
    }
}