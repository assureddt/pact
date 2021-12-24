using System.Text.Json;

namespace Pact.Core.Extensions;

public static class HttpContentExtensions
{
    /// <summary>
    /// Read HTTP Content as Json object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <param name="jsonOptions"></param>
    /// <returns></returns>
    public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content, JsonSerializerOptions jsonOptions = null) =>
        (await content.ReadAsStringAsync().ConfigureAwait(false)).FromJson<T>(jsonOptions);
}