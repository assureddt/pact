using System;
using System.Collections.Generic;

namespace Pact.Core.Comparers
{
    /// <summary>
    /// Provides equality comparison for a type based on the resolved value of a provided projection
    /// Saves the need for an explicit IEqualityComparer implementation where the comparison is based on a single contained primitive
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _projection;

        public static GenericEqualityComparer<T> Create<TValue>(Func<T, TValue> projection)
        {
            return new((t1, t2) => EqualityComparer<TValue>.Default.Equals( projection(t1), projection(t2)));
        }

        public GenericEqualityComparer(Func<T, T, bool> projection)
        {
            _projection = projection;
        }

        public bool Equals(T x, T y)
        {
            return _projection(x, y);
        }

        public int GetHashCode(T obj)
        {
            return 0;
        }
    }
}
