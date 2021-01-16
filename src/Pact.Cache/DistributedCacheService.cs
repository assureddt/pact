using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pact.Core.Extensions;

namespace Pact.Cache
{
    /// <inheritdoc/>
    public class DistributedCacheService : IDistributedCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<DistributedCacheService> _logger;

        public DistributedCacheService(IDistributedCache cache, ILogger<DistributedCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        /// <inheritdoc/>
        public void Remove(params string[] keys)
        {
            CacheLogContext(() =>
            {
                foreach (var key in keys)
                {
                    _cache.Remove(key);
                }
            });
        }

        /// <inheritdoc/>
        public T Get<T>(string key) where T : class
        {
            return Get(key)?.FromJson<T>();
        }

        /// <inheritdoc/>
        public string Get(string key)
        {
            return CacheLogContext(() => _cache.GetString(key));
        }

        /// <inheritdoc/>
        public T Set<T>(string key, T value, DistributedCacheEntryOptions options) where T : class
        {
            if (value == null)
                return null;

            CacheLogContext(() =>
            {
                _cache.SetString(key, value.ToJson(), options ?? new DistributedCacheEntryOptions());
            });

            return value;
        }

        /// <inheritdoc/>
        public string Set(string key, string value, DistributedCacheEntryOptions options)
        {
            if (value == null)
                return null;

            CacheLogContext(() =>
            {
                _cache.SetString(key, value, options ?? new DistributedCacheEntryOptions());
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
        public string Set(string key, Func<DistributedCacheEntryOptions, string> factory)
        {
            var opts = new DistributedCacheEntryOptions();

            var result = factory(opts);

            Set(key, result, opts);

            return result;
        }

        /// <inheritdoc/>
        public T GetOrCreate<T>(string key, Func<DistributedCacheEntryOptions, T> factory) where T : class
        {
            var result = Get<T>(key);

            return result ?? Set(key, factory);
        }

        /// <inheritdoc/>
        public string GetOrCreate(string key, Func<DistributedCacheEntryOptions, string> factory)
        {
            var result = Get(key);

            return result ?? Set(key, factory);
        }

        /// <inheritdoc/>
        public Task RemoveAsync(params string[] keys)
        {
            return CacheLogContext(async () =>
            {
                foreach (var key in keys)
                {
                    await _cache.RemoveAsync(key).ConfigureAwait(false);
                }
            });
        }

        /// <inheritdoc/>
        public async Task<T> GetAsync<T>(string key) where T : class
        {
            return (await GetAsync(key))?.FromJson<T>();
        }

        /// <inheritdoc/>
        public Task<string> GetAsync(string key)
        {
            return CacheLogContext(async () => (await _cache.GetStringAsync(key).ConfigureAwait(false)));
        }

        /// <inheritdoc/>
        public async Task<T> SetAsync<T>(string key, T value, DistributedCacheEntryOptions options) where T : class
        {
            await CacheLogContext(async () =>
            {
                await _cache.SetStringAsync(key, value.ToJson(), options ?? new DistributedCacheEntryOptions()).ConfigureAwait(false);
            }).ConfigureAwait(false);

            return value;
        }

        /// <inheritdoc/>
        public async Task<string> SetAsync(string key, string value, DistributedCacheEntryOptions options)
        {
            await CacheLogContext(async () =>
            {
                await _cache.SetStringAsync(key, value, options ?? new DistributedCacheEntryOptions()).ConfigureAwait(false);
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
        public async Task<string> SetAsync(string key, Func<DistributedCacheEntryOptions, Task<string>> factory)
        {
            var opts = new DistributedCacheEntryOptions();

            var result = await factory(opts).ConfigureAwait(false);
            await SetAsync(key, result, opts).ConfigureAwait(false);

            return result;
        }

        /// <inheritdoc/>
        public async Task<T> GetOrCreateAsync<T>(string key, Func<DistributedCacheEntryOptions, Task<T>> factory) where T : class
        {
            var result = await GetAsync<T>(key).ConfigureAwait(false);

            return result ?? await SetAsync(key, factory).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<string> GetOrCreateAsync(string key, Func<DistributedCacheEntryOptions, Task<string>> factory)
        {
            var result = await GetAsync(key).ConfigureAwait(false);

            return result ?? await SetAsync(key, factory).ConfigureAwait(false);
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
                _logger.LogWarning(e, "Distributed cache request failed");
            }

            return default;
        }
    }
}
