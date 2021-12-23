using Xunit;
using System;

namespace Pact.Cache.Tests.Extensions;

public class DistributedCacheExtensionsTests
{
    [Fact]
    public void AbsoluteExpirationOption_OK()
    {
        // act
        var result = Cache.Extensions.DistributedCacheExtensions.AbsoluteExpirationOption(null, TimeSpan.FromMinutes(5));

        // assert
        Assert.Null(result.SlidingExpiration);
        Assert.True(result.AbsoluteExpirationRelativeToNow.HasValue && result.AbsoluteExpirationRelativeToNow.Value == TimeSpan.FromMinutes(5));
    }
}