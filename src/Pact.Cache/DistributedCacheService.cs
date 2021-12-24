using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pact.Core.Extensions;

namespace Pact.Cache;

/// <inheritdoc/>
public class DistributedCacheService : DistributedCacheBase
{
    private readonly IDistributedCache _cache;

    public DistributedCacheService(IDistributedCache cache, ILogger<DistributedCacheService> logger)
        : base(logger)
    {
        _cache = cache;
    }

    /// <inheritdoc/>
    public override T Get<T>(string key, JsonSerializerOptions jsonOptions = null) where T : class
        => CacheLogContext(key, () => _cache.GetString(key)?.FromJson<T>(jsonOptions));

    /// <inheritdoc/>
    public override async Task<T> GetAsync<T>(string key, JsonSerializerOptions jsonOptions = null) where T : class
        => await CacheLogContext(key, async () => (await _cache.GetStringAsync(key))?.FromJson<T>(jsonOptions));

    /// <inheritdoc/>
    public override T? GetValue<T>(string key) where T : struct
        => CacheLogContext(key, () => _cache.GetString(key).ToNullable<T>());

    /// <inheritdoc/>
    public override Task<T?> GetValueAsync<T>(string key) where T : struct
        => CacheLogContext(key, async () => (await _cache.GetStringAsync(key).ConfigureAwait(false)).ToNullable<T>());

    /// <inheritdoc/>
    public override T Set<T>(string key, T value, DistributedCacheEntryOptions options, JsonSerializerOptions jsonOptions = null) where T : class
    {
        if (value == null)
            return null;

        CacheLogContext(key, () =>
        {
            _cache.SetString(key, value.ToJson(jsonOptions), options ?? new DistributedCacheEntryOptions());
        });

        return value;
    }

    /// <inheritdoc/>
    public override async Task<T> SetAsync<T>(string key, T value, DistributedCacheEntryOptions options, JsonSerializerOptions jsonOptions = null) where T : class
    {
        await CacheLogContext(key, async () =>
        {
            await _cache.SetStringAsync(key, value.ToJson(jsonOptions), options ?? new DistributedCacheEntryOptions()).ConfigureAwait(false);
        }).ConfigureAwait(false);

        return value;
    }

    /// <inheritdoc/>
    public override T? SetValue<T>(string key, T value, DistributedCacheEntryOptions options) where T : struct
    {
        CacheLogContext(key, () =>
        {
            _cache.SetString(key, value.ToString(), options ?? new DistributedCacheEntryOptions());
        });

        return value;
    }

    /// <inheritdoc/>
    public override async Task<T?> SetValueAsync<T>(string key, T value, DistributedCacheEntryOptions options) where T : struct
    {
        await CacheLogContext(key, async () =>
        {
            await _cache.SetStringAsync(key, value.ToString(), options ?? new DistributedCacheEntryOptions()).ConfigureAwait(false);
        }).ConfigureAwait(false);

        return value;
    }

    /// <inheritdoc/>
    public override void Remove(params string[] keys)
    {
        CacheLogContext(string.Join("; ", keys), () =>
        {
            foreach (var key in keys)
            {
                _cache.Remove(key);
            }
        });
    }

    /// <inheritdoc/>
    public override Task RemoveAsync(params string[] keys)
    {
        return CacheLogContext(string.Join("; ", keys),async () =>
        {
            foreach (var key in keys)
            {
                await _cache.RemoveAsync(key).ConfigureAwait(false);
            }
        });
    }
}