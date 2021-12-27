using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pact.Cache.Extensions;
using Pact.Core.Extensions;

namespace Pact.Cache;

/// <inheritdoc/>
public class DualLayerCacheService : DistributedCacheBase
{
    private readonly IDistributedCache _cache;
    private readonly IMemoryCache _memory;
    private readonly ILogger<DualLayerCacheService> _logger;
    private readonly TimeSpan _memoryRetention;

    public DualLayerCacheService(IDistributedCache cache, IMemoryCache memory, ILogger<DualLayerCacheService> logger, IOptions<CacheSettings> settings)
        : base(logger)
    {
        _cache = cache;
        _memory = memory;
        _logger = logger;
        _memoryRetention = TimeSpan.FromSeconds(settings?.Value.DefaultMemoryExpirySeconds ?? 60);
    }

    /// <summary>
    /// This is a global setting to change the expiry of the memory layer cache - it will be deliberately very short as we're just looking to speed up
    /// successive reads in the same request, rather than expecting it to be there between requests and we don't want to use more memory than necessary
    /// </summary>

    /// <summary>
    /// Ensures the in-memory retention is not beyond the provided distributed options
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    private TimeSpan GetMemoryRetention(DistributedCacheEntryOptions options)
    {
        if (options == null) return _memoryRetention;

        TimeSpan? relative = null;
        if (options.AbsoluteExpiration != null)
        {
            relative = options.AbsoluteExpiration.Value.Subtract(DateTime.Now);
        }

        relative ??= options.AbsoluteExpirationRelativeToNow ?? options.SlidingExpiration;
        if (relative == null) return _memoryRetention;
            
        return _memoryRetention < relative ? _memoryRetention : new TimeSpan(relative.Value.Ticks / 2);
    }

    /// <inheritdoc/>
    public override T Get<T>(string key, JsonSerializerOptions jsonOptions = null) where T : class => Get<T>(key, null, jsonOptions);

    /// <inheritdoc/>
    public override Task<T> GetAsync<T>(string key, JsonSerializerOptions jsonOptions = null) where T : class => GetAsync<T>(key, null, jsonOptions);

    /// <inheritdoc/>
    public override T? GetValue<T>(string key) where T : struct => GetValue<T>(key, null);

    /// <inheritdoc/>
    public override Task<T?> GetValueAsync<T>(string key) where T : struct => GetValueAsync<T>(key, null);

    private T Get<T>(string key, Func<DistributedCacheEntryOptions, T> factory, JsonSerializerOptions jsonOptions = null) where T : class
    {
        return CacheLogContext(CacheOperation.Get, key, () =>
        {
            var inMem = _memory.Get<T>(key);
            if (inMem != null)
            {
                _logger.MemoryCacheHit(key);
                return inMem;
            }

            var result = _cache.GetString(key)?.FromJson<T>(jsonOptions);

            if (result == null)
                return factory?.Invoke(new DistributedCacheEntryOptions());

            _memory.Set(key, result, GetMemoryRetention(null));

            return result;
        });
    }

    private async Task<T> GetAsync<T>(string key, Func<DistributedCacheEntryOptions, Task<T>> factory, JsonSerializerOptions jsonOptions = null) where T : class
    {
        return await CacheLogContext(CacheOperation.Get, key, async () =>
        {
            var inMem = _memory.Get<T>(key);
            if (inMem != null)
            {
                _logger.MemoryCacheHit(key);
                return inMem;
            }

            var result = (await _cache.GetStringAsync(key))?.FromJson<T>(jsonOptions);

            if (result == null)
            {
                if (factory == null)
                    return null;

                return await factory(new DistributedCacheEntryOptions());
            }

            _memory.Set(key, result, GetMemoryRetention(null));

            return result;
        });
    }

    private T? GetValue<T>(string key, Func<DistributedCacheEntryOptions, T> factory) where T : struct
    {
        return CacheLogContext(CacheOperation.Get, key, () =>
        {
            var inMem = _memory.Get<T?>(key);
            if (inMem != null)
            {
                _logger.MemoryCacheHit(key);
                return inMem;
            }

            var result = _cache.GetString(key)?.ToNullable<T>();

            if (result == null)
                return factory?.Invoke(new DistributedCacheEntryOptions());

            _memory.Set(key, result, GetMemoryRetention(null));

            return result;
        });
    }

    private async Task<T?> GetValueAsync<T>(string key, Func<DistributedCacheEntryOptions, Task<T>> factory) where T : struct
    {
        return await CacheLogContext(CacheOperation.Get, key, async () =>
        {
            var inMem = _memory.Get<T?>(key);
            if (inMem != null)
            {
                _logger.MemoryCacheHit(key);
                return inMem;
            }

            var result = (await _cache.GetStringAsync(key))?.ToNullable<T>();

            if (result == null)
            {
                if (factory == null)
                    return null;

                return await factory(new DistributedCacheEntryOptions());
            }

            _memory.Set(key, result, GetMemoryRetention(null));

            return result;
        });
    }

    /// <inheritdoc/>
    public override T Set<T>(string key, T value, DistributedCacheEntryOptions options, JsonSerializerOptions jsonOptions = null) where T : class
    {
        if (value == null)
            return null;

        CacheLogContext(CacheOperation.Set, key, () =>
        {
            _cache.SetString(key, value.ToJson(jsonOptions), options ?? new DistributedCacheEntryOptions());
            _memory.Set(key, value, GetMemoryRetention(options));
        });

        return value;
    }

    /// <inheritdoc/>
    public override async Task<T> SetAsync<T>(string key, T value, DistributedCacheEntryOptions options, JsonSerializerOptions jsonOptions = null) where T : class
    {
        await CacheLogContext(CacheOperation.Set, key, async () =>
        {
            await _cache.SetStringAsync(key, value.ToJson(jsonOptions), options ?? new DistributedCacheEntryOptions()).ConfigureAwait(false);
            _memory.Set(key, value, GetMemoryRetention(options));
        }).ConfigureAwait(false);

        return value;
    }

    /// <inheritdoc/>
    public override T? SetValue<T>(string key, T value, DistributedCacheEntryOptions options) where T : struct
    {
        CacheLogContext(CacheOperation.Set, key, () =>
        {
            _cache.SetString(key, value.ToString(), options ?? new DistributedCacheEntryOptions());
            _memory.Set(key, value, GetMemoryRetention(options));
        });

        return value;
    }

    /// <inheritdoc/>
    public override async Task<T?> SetValueAsync<T>(string key, T value, DistributedCacheEntryOptions options) where T : struct
    {
        await CacheLogContext(CacheOperation.Set, key, async () =>
        {
            await _cache.SetStringAsync(key, value.ToString(), options ?? new DistributedCacheEntryOptions()).ConfigureAwait(false);
            _memory.Set(key, value, GetMemoryRetention(options));
        }).ConfigureAwait(false);

        return value;
    }

    /// <inheritdoc/>
    public override void Remove(params string[] keys)
    {
        CacheLogContext(CacheOperation.Remove, string.Join("; ", keys), () =>
        {
            foreach (var key in keys)
            {
                _cache.Remove(key);
                _memory.Remove(key);
            }
        });
    }

    /// <inheritdoc/>
    public override Task RemoveAsync(params string[] keys)
    {
        return CacheLogContext(CacheOperation.Remove, string.Join("; ", keys), async () =>
        {
            foreach (var key in keys)
            {
                await _cache.RemoveAsync(key).ConfigureAwait(false);
                _memory.Remove(key);
            }
        });
    }
}