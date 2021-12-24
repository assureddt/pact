using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Pact.Cache;

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
    /// Get the value of a specified key and convert it to a reference type
    /// </summary>
    /// <remarks>Internally deserializes from JSON</remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="jsonOptions">Optional options to override the default STJ ones</param>
    /// <returns></returns>
    T Get<T>(string key, JsonSerializerOptions jsonOptions = null) where T : class;
    /// <summary>
    /// Get the value type of a specified key
    /// </summary>
    /// <remarks>Does NOT internally deserialize from JSON</remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    T? GetValue<T>(string key) where T: struct;

    /// <summary>
    /// Convert a reference type to a json string and store it against the specified key
    /// </summary>
    /// <remarks>Internally serializes to JSON</remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <param name="jsonOptions">Optional options to override the default STJ ones</param>
    /// <returns></returns>
    T Set<T>(string key, T value, DistributedCacheEntryOptions options, JsonSerializerOptions jsonOptions = null) where T : class;
    /// <summary>
    /// Store a value type against the specified key
    /// </summary>
    /// <remarks>Does NOT internally serialize to JSON</remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    T? SetValue<T>(string key, T value, DistributedCacheEntryOptions options) where T: struct;

    /// <summary>
    /// Convert a reference type to a json string and store it against the specified key
    /// </summary>
    /// <remarks>Internally serializes to JSON</remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    /// <param name="jsonOptions">Optional options to override the default STJ ones</param>
    /// <returns></returns>
    T Set<T>(string key, Func<DistributedCacheEntryOptions, T> factory, JsonSerializerOptions jsonOptions = null) where T : class;
    /// <summary>
    /// Store a value type against the specified key
    /// </summary>
    /// <remarks>Does NOT internally serialize to JSON</remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    /// <returns></returns>
    T? SetValue<T>(string key, Func<DistributedCacheEntryOptions, T> factory) where T: struct;

    /// <summary>
    /// Retrieves or creates a reference type in json format against the specified key
    /// </summary>
    /// <remarks>Internally (de)serializes (from)/to JSON</remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    /// <param name="jsonOptions">Optional options to override the default STJ ones</param>
    /// <returns></returns>
    T GetOrCreate<T>(string key, Func<DistributedCacheEntryOptions, T> factory, JsonSerializerOptions jsonOptions = null) where T : class;
    /// <summary>
    /// Retrieves or creates a value type against the specified key
    /// </summary>
    /// <remarks>Does NOT internally (de)serialize (from)/to JSON</remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    /// <returns></returns>
    T? GetOrCreateValue<T>(string key, Func<DistributedCacheEntryOptions, T> factory) where T : struct;

    /// <summary>
    /// Remove specified keys
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    Task RemoveAsync(params string[] keys);

    /// <summary>
    /// Retrieves a reference type from json format against the specified key
    /// </summary>
    /// <remarks>Internally deserializes from JSON</remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="jsonOptions">Optional options to override the default STJ ones</param>
    /// <returns></returns>
    Task<T> GetAsync<T>(string key, JsonSerializerOptions jsonOptions = null) where T : class;
    /// <summary>
    /// Retrieves a value type against the specified key
    /// </summary>
    /// <remarks>Does NOT internally deserialize from JSON</remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<T?> GetValueAsync<T>(string key) where T: struct;

    /// <summary>
    /// Convert a reference type to a json string and store it against the specified key
    /// </summary>
    /// <remarks>Internally serializes to JSON</remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <param name="jsonOptions">Optional options to override the default STJ ones</param>
    /// <returns></returns>
    Task<T> SetAsync<T>(string key, T value, DistributedCacheEntryOptions options, JsonSerializerOptions jsonOptions = null) where T : class;
    /// <summary>
    /// Store a value type against the specified key
    /// </summary>
    /// <remarks>Does NOT internally serialize to JSON</remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    Task<T?> SetValueAsync<T>(string key, T value, DistributedCacheEntryOptions options) where T: struct;

    /// <summary>
    /// Convert a reference type to a json string and store it against the specified key
    /// </summary>
    /// <remarks>Internally serializes to JSON</remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    /// <param name="jsonOptions">Optional options to override the default STJ ones</param>
    /// <returns></returns>
    Task<T> SetAsync<T>(string key, Func<DistributedCacheEntryOptions, Task<T>> factory, JsonSerializerOptions jsonOptions = null) where T : class;
    /// <summary>
    /// Store a value type against the specified key
    /// </summary>
    /// <remarks>Does NOT internally serialize to JSON</remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    /// <returns></returns>
    Task<T?> SetValueAsync<T>(string key, Func<DistributedCacheEntryOptions, Task<T>> factory) where T: struct;

    /// <summary>
    /// Retrieves or creates a reference type in json format against the specified key
    /// </summary>
    /// <remarks>Internally (de)serializes (from)/to JSON</remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    /// <param name="jsonOptions">Optional options to override the default STJ ones</param>
    /// <returns></returns>
    Task<T> GetOrCreateAsync<T>(string key, Func<DistributedCacheEntryOptions, Task<T>> factory, JsonSerializerOptions jsonOptions = null) where T : class;
    /// <summary>
    /// Retrieves or creates a value type against the specified key
    /// </summary>
    /// <remarks>Does NOT internally (de)serialize (from)/to JSON</remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    /// <returns></returns>
    Task<T?> GetOrCreateValueAsync<T>(string key, Func<DistributedCacheEntryOptions, Task<T>> factory) where T: struct;
}