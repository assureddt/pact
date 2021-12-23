using Microsoft.AspNetCore.Http;
using Pact.Core.Extensions;

namespace Pact.Web.Extensions;

public static class SessionExtensions
{
    /// <summary>
    /// Serialize a json value to the session
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="session"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, value.ToJson());
    }

    /// <summary>
    /// Deserialize a json object from the session
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="session"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static T Get<T>(this ISession session, string key)
    {
        var value = session.GetString(key);

        return value == null ? default : value.FromJson<T>();
    }
}