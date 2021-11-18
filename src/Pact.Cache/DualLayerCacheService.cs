using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pact.Core.Extensions;

namespace Pact.Cache
{
    /// <inheritdoc/>
    public class DualLayerCacheService : IDistributedCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly IMemoryCache _memory;
        private readonly ILogger<DualLayerCacheService> _logger;
        private readonly TimeSpan _memoryRetention;

        public DualLayerCacheService(IDistributedCache cache, IMemoryCache memory, ILogger<DualLayerCacheService> logger, IOptions<CacheSettings> settings)
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
        public T Get<T>(string key) where T : class => Get<T>(key, null);

        /// <inheritdoc/>
        public Task<T> GetAsync<T>(string key) where T : class => GetAsync<T>(key, null);

        /// <inheritdoc/>
        public T? GetValue<T>(string key) where T : struct => GetValue<T>(key, null);

        /// <inheritdoc/>
        public Task<T?> GetValueAsync<T>(string key) where T : struct => GetValueAsync<T>(key, null);

        private T Get<T>(string key, Func<DistributedCacheEntryOptions, T> factory) where T : class
        {
            return CacheLogContext(() =>
            {
                var inMem = _memory.Get<T>(key);
                if (inMem != null)
                    return inMem;

                if (factory == null)
                    return _cache.GetString(key)?.FromJson<T>();
                
                var result = _cache.GetString(key)?.FromJson<T>();

                return result != null ? _memory.Set(key, result) : Set(key, factory);
            });
        }

        private async Task<T> GetAsync<T>(string key, Func<DistributedCacheEntryOptions, Task<T>> factory) where T : class
        {
            return await CacheLogContext(async () =>
            {
                var inMem = _memory.Get<T>(key);
                if (inMem != null)
                    return inMem;

                if (factory == null)
                    return (await _cache.GetStringAsync(key))?.FromJson<T>();

                var result = (await _cache.GetStringAsync(key))?.FromJson<T>();

                return result != null ? _memory.Set(key, result) : await SetAsync(key, factory);
            });
        }

        private T? GetValue<T>(string key, Func<DistributedCacheEntryOptions, T> factory) where T : struct
        {
            return CacheLogContext(() =>
            {
                var inMem = _memory.Get<T?>(key);
                if (inMem != null)
                    return inMem;

                if (factory == null)
                    return _cache.GetString(key)?.ToNullable<T>();

                var result = _cache.GetString(key)?.ToNullable<T>();

                return result != null ? _memory.Set(key, result) : SetValue(key, factory);
            });
        }

        private async Task<T?> GetValueAsync<T>(string key, Func<DistributedCacheEntryOptions, Task<T>> factory) where T : struct
        {
            return await CacheLogContext(async () =>
            {
                var inMem = _memory.Get<T?>(key);
                if (inMem != null)
                    return inMem;

                if (factory == null)
                    return (await _cache.GetStringAsync(key))?.ToNullable<T>();

                var result = (await _cache.GetStringAsync(key))?.ToNullable<T>();

                return result != null ? _memory.Set(key, result) : await SetValueAsync(key, factory);
            });
        }

        /// <inheritdoc/>
        public T Set<T>(string key, T value, DistributedCacheEntryOptions options) where T : class
        {
            if (value == null)
                return null;

            CacheLogContext(() =>
            {
                _cache.SetString(key, value.ToJson(), options ?? new DistributedCacheEntryOptions());
                _memory.Set(key, value, GetMemoryRetention(options));
            });

            return value;
        }

        /// <inheritdoc/>
        public T Set<T>(string key, Func<DistributedCacheEntryOptions, T> factory) where T : class
        {
            var opts = new DistributedCacheEntryOptions();

            var result = factory(opts);

            Set(key, result, opts);

            return result;
        }

        /// <inheritdoc/>
        public async Task<T> SetAsync<T>(string key, T value, DistributedCacheEntryOptions options) where T : class
        {
            await CacheLogContext(async () =>
            {
                await _cache.SetStringAsync(key, value.ToJson(), options ?? new DistributedCacheEntryOptions()).ConfigureAwait(false);
                _memory.Set(key, value, GetMemoryRetention(options));
            }).ConfigureAwait(false);

            return value;
        }

        /// <inheritdoc/>
        public async Task<T> SetAsync<T>(string key, Func<DistributedCacheEntryOptions, Task<T>> factory) where T : class
        {
            var opts = new DistributedCacheEntryOptions();

            var result = await factory(opts).ConfigureAwait(false);
            await SetAsync(key, result, opts).ConfigureAwait(false);

            return result;
        }

        /// <inheritdoc/>
        public T? SetValue<T>(string key, T value, DistributedCacheEntryOptions options) where T : struct
        {
            CacheLogContext(() =>
            {
                _cache.SetString(key, value.ToString(), options ?? new DistributedCacheEntryOptions());
                _memory.Set(key, value, GetMemoryRetention(options));
            });

            return value;
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
        public async Task<T?> SetValueAsync<T>(string key, T value, DistributedCacheEntryOptions options) where T : struct
        {
            await CacheLogContext(async () =>
            {
                await _cache.SetStringAsync(key, value.ToString(), options ?? new DistributedCacheEntryOptions()).ConfigureAwait(false);
                _memory.Set(key, value, GetMemoryRetention(options));
            }).ConfigureAwait(false);

            return value;
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
        public T GetOrCreate<T>(string key, Func<DistributedCacheEntryOptions, T> factory) where T : class
        {
            var result = Get(key, factory);

            return result ?? Set(key, factory);
        }

        /// <inheritdoc/>
        public async Task<T> GetOrCreateAsync<T>(string key, Func<DistributedCacheEntryOptions, Task<T>> factory) where T : class
        {
            var result = await GetAsync(key, factory).ConfigureAwait(false);

            return result ?? await SetAsync(key, factory).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public T? GetOrCreateValue<T>(string key, Func<DistributedCacheEntryOptions, T> factory) where T : struct
        {
            var result = GetValue(key, factory);

            return result ?? SetValue(key, factory);
        }

        /// <inheritdoc/>
        public async Task<T?> GetOrCreateValueAsync<T>(string key, Func<DistributedCacheEntryOptions, Task<T>> factory) where T : struct
        {
            var result = await GetValueAsync(key, factory).ConfigureAwait(false);

            return result ?? await SetValueAsync(key, factory).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public void Remove(params string[] keys)
        {
            CacheLogContext(() =>
            {
                foreach (var key in keys)
                {
                    _cache.Remove(key);
                    _memory.Remove(key);
                }
            });
        }

        /// <inheritdoc/>
        public Task RemoveAsync(params string[] keys)
        {
            return CacheLogContext(async () =>
            {
                foreach (var key in keys)
                {
                    await _cache.RemoveAsync(key).ConfigureAwait(false);
                    _memory.Remove(key);
                }
            });
        }

        private void CacheLogContext(Action action)
        {
            CacheLogContext(() =>
            {
                action();
                return 0;
            });
        }

        private T CacheLogContext<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Dual Layer Distributed cache request failed");
            }

            return default;
        }
    }
}
