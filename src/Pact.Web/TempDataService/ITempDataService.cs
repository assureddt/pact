using System.Text.Json;

namespace Pact.Web.TempDataService;

/// <summary>
/// A wrapper service around ITempDataDictionaryFactory to make it easier to store objects
/// </summary>
public interface ITempDataService
{
    /// <summary>
    /// Gets a item from the Temp Data Dictionary
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="jsonOptions"></param>
    /// <returns></returns>
    T Get<T>(string key, JsonSerializerOptions jsonOptions = null) where T : class;

    /// <summary>
    /// Sets item in Temp Data Dictionary
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="jsonOptions"></param>
    void Set<T>(string key, T value, JsonSerializerOptions jsonOptions = null) where T : class;

    /// <summary>
    /// Store a value which can be retrieved with a token
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">Separator key</param>
    /// <param name="value">Item to be cached</param>
    /// <param name="jsonOptions"></param>
    /// <returns></returns>
    string StoreOnKeyToken<T>(string key, T value, JsonSerializerOptions jsonOptions = null) where T : class;

    /// <summary>
    /// Gets a value cached from a token
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">Separator key</param>
    /// <param name="token">Token to retrieve the value on</param>
    /// <param name="jsonOptions"></param>
    /// <returns></returns>
    T GetFromKeyToken<T>(string key, string token, JsonSerializerOptions jsonOptions = null) where T : class;

    /// <summary>
    /// Clears the Temp Data Dictionary
    /// </summary>
    void Clear();
}