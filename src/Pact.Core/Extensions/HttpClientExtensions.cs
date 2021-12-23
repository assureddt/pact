using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

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
    /// <returns></returns>
    public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
    {
        var content = new StringContent(data.ToJson());
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
    /// <returns></returns>
    public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
    {
        var content = new StringContent(data.ToJson());
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        return httpClient.PutAsync(url, content);
    }
}