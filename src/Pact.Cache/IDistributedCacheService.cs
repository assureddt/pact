using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Pact.Cache
{
    /// <summary>
    /// Provides a wrapper around the default DistributedCache to add common functionality
    /// </summary>
    public interface IDistributedCacheService
    {
        /// <summary>
        /// Remove specified keys
        /// </summary>
        /// <param name="keys"></param>
        void Remove(params string[] keys);

        /// <summary>
        /// Get the value of a specified key and convert it to an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key) where T : class;
        /// <summary>
        /// Get the string value of a specified key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string Get(string key);

        /// <summary>
        /// Convert an object to a json string and store it against the specified key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        T Set<T>(string key, T value, DistributedCacheEntryOptions options) where T : class;
        /// <summary>
        /// Store a string value against the specified key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        string Set(string key, string value, DistributedCacheEntryOptions options);

        /// <summary>
        /// Convert an object to a json string and store it against the specified key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        T Set<T>(string key, Func<DistributedCacheEntryOptions, T> factory) where T : class;
        /// <summary>
        /// Store a string value against the specified key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        string Set(string key, Func<DistributedCacheEntryOptions, string> factory);

        /// <summary>
        /// Retrieves or creates an object in json format against the specified key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        T GetOrCreate<T>(string key, Func<DistributedCacheEntryOptions, T> factory) where T : class;
        /// <summary>
        /// Retrieves or creates a string value against the specified key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        string GetOrCreate(string key, Func<DistributedCacheEntryOptions, string> factory);

        /// <summary>
        /// Remove specified keys
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        Task RemoveAsync(params string[] keys);

        /// <summary>
        /// Retrieves an object from json format against the specified key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string key) where T : class;
        /// <summary>
        /// Retrieves a string value against the specified key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<string> GetAsync(string key);

        /// <summary>
        /// Convert an object to a json string and store it against the specified key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<T> SetAsync<T>(string key, T value, DistributedCacheEntryOptions options) where T : class;
        /// <summary>
        /// Store a string value against the specified key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<string> SetAsync(string key, string value, DistributedCacheEntryOptions options);

        /// <summary>
        /// Convert an object to a json string and store it against the specified key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        Task<T> SetAsync<T>(string key, Func<DistributedCacheEntryOptions, Task<T>> factory) where T : class;
        /// <summary>
        /// Store a string value against the specified key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        Task<string> SetAsync(string key, Func<DistributedCacheEntryOptions, Task<string>> factory);

        /// <summary>
        /// Retrieves or creates an object in json format against the specified key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        Task<T> GetOrCreateAsync<T>(string key, Func<DistributedCacheEntryOptions, Task<T>> factory) where T : class;
        /// <summary>
        /// Retrieves or creates a string value against the specified key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        Task<string> GetOrCreateAsync(string key, Func<DistributedCacheEntryOptions, Task<string>> factory);
    }
}