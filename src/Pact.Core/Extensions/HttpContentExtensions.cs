using System.Net.Http;
using System.Threading.Tasks;

namespace Pact.Core.Extensions;

public static class HttpContentExtensions
{

    /// <summary>
    /// Read HTTP Content as Json object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <returns></returns>
    public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content) =>
        (await content.ReadAsStringAsync().ConfigureAwait(false)).FromJson<T>();
}