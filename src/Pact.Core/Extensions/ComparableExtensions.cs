using System;

namespace Pact.Core.Extensions
{
    public static class ComparableExtensions
    {
        /// <summary>
        /// Determines if a value falls in a defined range based on inclusivity criteria
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value to be checked</param>
        /// <param name="lower">The lower extent of the range</param>
        /// <param name="upper">The upper extent of the range</param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static bool IsInRange<T>(this T value, T lower, T upper, RangeExtremityInclusion comparison = RangeExtremityInclusion.Inclusive) where T : IComparable
        {
            if (lower.CompareTo(upper) > 0)
                throw new ArgumentOutOfRangeException(nameof(upper), upper, null);

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
