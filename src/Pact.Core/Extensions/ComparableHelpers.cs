using System;

namespace Pact.Core.Extensions
{
    public static class ComparableHelpers
    {
        public static bool IsInRange<T>(this T value, T lower, T upper, RangeExtremityInclusion comparison) where T : IComparable
        {
            return comparison switch
            {
                RangeExtremityInclusion.Exclusive => value.CompareTo(lower) > 0 && value.CompareTo(upper) < 0,
                RangeExtremityInclusion.Inclusive => value.CompareTo(lower) >= 0 && value.CompareTo(upper) <= 0,
                RangeExtremityInclusion.LowerInclusiveUpperExclusive => value.CompareTo(lower) >= 0 && value.CompareTo(upper) < 0,
                RangeExtremityInclusion.LowerExclusiveUpperInclusive => value.CompareTo(lower) > 0 && value.CompareTo(upper) <= 0,
                _ => throw new ArgumentOutOfRangeException(nameof(comparison), comparison, null)
            };
        }

        public enum RangeExtremityInclusion
        {
            Exclusive,
            Inclusive,
            LowerInclusiveUpperExclusive,
            LowerExclusiveUpperInclusive
        }
    }
}
