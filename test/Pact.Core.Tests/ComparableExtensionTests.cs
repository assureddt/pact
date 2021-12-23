using System;
using Pact.Core.Extensions;
using Shouldly;
using Xunit;

namespace Pact.Core.Tests;

public class ComparableExtensionTests
{
    [Fact]
    public void InRange_Range_Invalid_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => 8.IsInRange(12, 10));
    }

    [Fact]
    public void InRange_Integer_Exclusive_OK()
    {
        8.IsInRange(4, 10, ComparableExtensions.RangeExtremityInclusion.Exclusive).ShouldBeTrue();
    }

    [Fact]
    public void InRange_Integer_Exclusive_Lower_False()
    {
        4.IsInRange(4, 10, ComparableExtensions.RangeExtremityInclusion.Exclusive).ShouldBeFalse();
    }

    [Fact]
    public void InRange_Integer_Exclusive_Upper_False()
    {
        10.IsInRange(4, 10, ComparableExtensions.RangeExtremityInclusion.Exclusive).ShouldBeFalse();
    }

    [Fact]
    public void InRange_Integer_Inclusive_OK()
    {
        8.IsInRange(4, 10).ShouldBeTrue();
    }

    [Fact]
    public void InRange_Integer_Inclusive_Lower_OK()
    {
        4.IsInRange(4, 10).ShouldBeTrue();
    }

    [Fact]
    public void InRange_Integer_Inclusive_Upper_OK()
    {
        10.IsInRange(4, 10).ShouldBeTrue();
    }

    [Fact]
    public void InRange_Integer_Inclusive_LowOutside_False()
    {
        3.IsInRange(4, 10).ShouldBeFalse();
    }

    [Fact]
    public void InRange_Integer_Inclusive_HighOutside_False()
    {
        11.IsInRange(4, 10).ShouldBeFalse();
    }

    [Fact]
    public void InRange_Integer_ExclusiveLower_OK()
    {
        8.IsInRange(4, 10, ComparableExtensions.RangeExtremityInclusion.LowerExclusiveUpperInclusive).ShouldBeTrue();
    }

    [Fact]
    public void InRange_Integer_ExclusiveLower_Lower_False()
    {
        4.IsInRange(4, 10, ComparableExtensions.RangeExtremityInclusion.LowerExclusiveUpperInclusive).ShouldBeFalse();
    }

    [Fact]
    public void InRange_Integer_ExclusiveLower_Upper_OK()
    {
        10.IsInRange(4, 10, ComparableExtensions.RangeExtremityInclusion.LowerExclusiveUpperInclusive).ShouldBeTrue();
    }

    [Fact]
    public void InRange_Integer_ExclusiveUpper_OK()
    {
        8.IsInRange(4, 10, ComparableExtensions.RangeExtremityInclusion.LowerInclusiveUpperExclusive).ShouldBeTrue();
    }

    [Fact]
    public void InRange_Integer_ExclusiveUpper_Lower_OK()
    {
        4.IsInRange(4, 10, ComparableExtensions.RangeExtremityInclusion.LowerInclusiveUpperExclusive).ShouldBeTrue();
    }

    [Fact]
    public void InRange_Integer_ExclusiveUpper_Upper_False()
    {
        10.IsInRange(4, 10, ComparableExtensions.RangeExtremityInclusion.LowerInclusiveUpperExclusive).ShouldBeFalse();
    }

    [Fact]
    public void InRange_DateTime_Inclusive_OK()
    {
        DateTime.Now.IsInRange(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1)).ShouldBeTrue();
    }

    [Fact]
    public void InRange_DateTime_Inclusive_Lower_OK()
    {
        DateTime.Today.AddDays(-1).IsInRange(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1)).ShouldBeTrue();
    }

    [Fact]
    public void InRange_DateTime_Inclusive_Upper_OK()
    {
        DateTime.Today.AddDays(1).IsInRange(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1)).ShouldBeTrue();
    }

    [Fact]
    public void InRange_DateTime_Inclusive_LowOutside_False()
    {
        DateTime.Today.AddDays(-1.1).IsInRange(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1)).ShouldBeFalse();
    }

    [Fact]
    public void InRange_DateTime_Inclusive_HighOutside_False()
    {
        DateTime.Today.AddDays(1.1).IsInRange(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1)).ShouldBeFalse();
    }
}