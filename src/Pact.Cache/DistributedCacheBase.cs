using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pact.Cache.Extensions;

namespace Pact.Cache;

/// <inheritdoc/>
public abstract class DistributedCacheBase : IDistributedCacheService
{
    private readonly ILogger<DistributedCacheBase> _logger;

    protected DistributedCacheBase(ILogger<DistributedCacheBase> logger)
    {
        _logger = logger;
    }

    protected void CacheLogContext(string key, Action action)
    {
        CacheLogContext(key, () =>
        {
            action();
            return 0;
        });
    }

    protected T CacheLogContext<T>(string key, Func<T> action)
    {
        try
        {
            return action();
        }
        catch (Exception e)
        {
            _logger.CacheOperationFailed(key, e);
        }
        finally
        {
            _logger.CacheOperationRequested(key);
        }

        return default;
    }

    /// <inheritdoc/>
    public abstract T Get<T>(string key, JsonSerializerOptions jsonOptions = null) where T : class;

    /// <inheritdoc/>
    public abstract Task<T> GetAsync<T>(string key, JsonSerializerOptions jsonOptions = null) where T : class;

    /// <inheritdoc/>
    public abstract T? GetValue<T>(string key) where T : struct;

    /// <inheritdoc/>
    public abstract Task<T?> GetValueAsync<T>(string key) where T : struct;

    /// <inheritdoc/>
    public T Set<T>(string key, Func<DistributedCacheEntryOptions, T> factory, JsonSerializerOptions jsonOptions = null) where T : class
    {
        var opts = new DistributedCacheEntryOptions();

        var result = factory(opts);

        Set(key, result, opts, jsonOptions);

        return result;
    }

    /// <inheritdoc/>
    public async Task<T> SetAsync<T>(string key, Func<DistributedCacheEntryOptions, Task<T>> factory, JsonSerializerOptions jsonOptions = null) where T : class
    {
        var opts = new DistributedCacheEntryOptions();

        var result = await factory(opts).ConfigureAwait(false);
        await SetAsync(key, result, opts, jsonOptions).ConfigureAwait(false);

        return result;
    }

    /// <inheritdoc/>
    public T? SetValue<T>(string key, Func<DistributedCacheEntryOptions, T> factory) where T : struct
    {
        var opts = new DistributedCacheEntryOptions();

        var result = factory(opts);

        SetValue(key, result, opts);

        return result;
    }

    /// <inheritdoc/>
    public async Task<T?> SetValueAsync<T>(string key, Func<DistributedCacheEntryOptions, Task<T>> factory) where T : struct
    {
        var opts = new DistributedCacheEntryOptions();

        var result = await factory(opts).ConfigureAwait(false);
        await SetValueAsync(key, result, opts).ConfigureAwait(false);

        return result;
    }

    /// <inheritdoc/>
    public T GetOrCreate<T>(string key, Func<DistributedCacheEntryOptions, T> factory, JsonSerializerOptions jsonOptions = null) where T : class
    {
        var result = Get<T>(key, jsonOptions);

        return result ?? Set(key, factory, jsonOptions);
    }

    /// <inheritdoc/>
    public async Task<T> GetOrCreateAsync<T>(string key, Func<DistributedCacheEntryOptions, Task<T>> factory, JsonSerializerOptions jsonOptions = null) where T : class
    {
        var result = await GetAsync<T>(key, jsonOptions).ConfigureAwait(false);

        return result ?? await SetAsync(key, factory, jsonOptions).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public T? GetOrCreateValue<T>(string key, Func<DistributedCacheEntryOptions, T> factory) where T : struct
    {
        var result = GetValue<T>(key);

        return result ?? SetValue(key, factory);
    }

    /// <inheritdoc/>
    public async Task<T?> GetOrCreateValueAsync<T>(string key, Func<DistributedCacheEntryOptions, Task<T>> factory) where T : struct
    {
        var result = await GetValueAsync<T>(key).ConfigureAwait(false);

        return result ?? await SetValueAsync(key, factory).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public abstract void Remove(params string[] keys);
    /// <inheritdoc/>
    public abstract T Set<T>(string key, T value, DistributedCacheEntryOptions options, JsonSerializerOptions jsonOptions = null) where T : class;
    /// <inheritdoc/>
    public abstract T? SetValue<T>(string key, T value, DistributedCacheEntryOptions options) where T : struct;
    /// <inheritdoc/>
    public abstract Task RemoveAsync(params string[] keys);
    /// <inheritdoc/>
    public abstract Task<T> SetAsync<T>(string key, T value, DistributedCacheEntryOptions options, JsonSerializerOptions jsonOptions = null) where T : class;
    /// <inheritdoc/>
    public abstract Task<T?> SetValueAsync<T>(string key, T value, DistributedCacheEntryOptions options) where T : struct;
}