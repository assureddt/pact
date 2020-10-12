using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Pact.Cache
{
    public interface IDistributedCacheService
    {
        void Remove(params string[] keys);

        T Get<T>(string key) where T : class;
        string Get(string key);

        T Set<T>(string key, T value, DistributedCacheEntryOptions options) where T : class;
        string Set(string key, string value, DistributedCacheEntryOptions options);

        T Set<T>(string key, Func<DistributedCacheEntryOptions, T> factory) where T : class;
        string Set(string key, Func<DistributedCacheEntryOptions, string> factory);

        T GetOrCreate<T>(string key, Func<DistributedCacheEntryOptions, T> factory) where T : class;
        string GetOrCreate(string key, Func<DistributedCacheEntryOptions, string> factory);

        Task RemoveAsync(params string[] keys);

        Task<T> GetAsync<T>(string key) where T : class;
        Task<string> GetAsync(string key);

        Task<T> SetAsync<T>(string key, T value, DistributedCacheEntryOptions options) where T : class;
        Task<string> SetAsync(string key, string value, DistributedCacheEntryOptions options);

        Task<T> SetAsync<T>(string key, Func<DistributedCacheEntryOptions, Task<T>> factory) where T : class;
        Task<string> SetAsync(string key, Func<DistributedCacheEntryOptions, Task<string>> factory);

        Task<T> GetOrCreateAsync<T>(string key, Func<DistributedCacheEntryOptions, Task<T>> factory) where T : class;
        Task<string> GetOrCreateAsync(string key, Func<DistributedCacheEntryOptions, Task<string>> factory);
    }
}