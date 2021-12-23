using Microsoft.Extensions.Caching.Distributed;

namespace Pact.Cache.Extensions;

public static class DistributedCacheExtensions
{
    /// <summary>
    /// Creates a DistributedCacheEntryOptions set to expire on a given timespan from now
    /// </summary>
    /// <param name="distributedCacheService">Not used only present to bring the extension method in an easy place</param>
    /// <param name="cacheTime">The time from now the cache time will expire</param>
    /// <returns></returns>
    public static DistributedCacheEntryOptions AbsoluteExpirationOption(this IDistributedCacheService _, TimeSpan cacheTime)
    {
        return new DistributedCacheEntryOptions().SetAbsoluteExpiration(cacheTime);
    }
}