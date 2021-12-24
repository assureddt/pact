using System.Net.Http.Headers;
using System.Text.Json;

namespace Pact.Core.Extensions;

public static class HttpClientExtensions
{
    /// <summary>
    /// Basic POST object to url as Json
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="httpClient"></param>
    /// <param name="url"></param>
    /// <param name="data"></param>
    /// <param name="jsonOptions"></param>
    /// <returns></returns>
    public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient, string url, T data, JsonSerializerOptions jsonOptions = null)
    {
        var content = new StringContent(data.ToJson(jsonOptions));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        return httpClient.PostAsync(url, content);
    }

    /// <summary>
    /// Basic PUT object to url as Json
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="httpClient"></param>
    /// <param name="url"></param>
    /// <param name="data"></param>
    /// <param name="jsonOptions"></param>
    /// <returns></returns>
    public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient httpClient, string url, T data, JsonSerializerOptions jsonOptions = null)
    {
        var content = new StringContent(data.ToJson(jsonOptions));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        return httpClient.PutAsync(url, content);
    }
}