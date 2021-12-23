using System;
using System.Collections.Generic;

namespace Pact.Core.Comparers;

/// <summary>
/// Provides sort order comparison for a type based on the resolved value of a provided projection
/// Saves the need for an explicit IComparer implementation where the comparison is based on a single contained primitive
/// </summary>
/// <typeparam name="T"></typeparam>
public class GenericComparer<T> : IComparer<T>
{
    private readonly Func<T, T, int> _projection;

    public static GenericComparer<T> Create<TValue>(Func<T, TValue> projection)
    {
        return new GenericComparer<T>((t1, t2) => Comparer<TValue>.Default.Compare(projection(t1), projection(t2)));
    }

    public GenericComparer(Func<T, T, int> projection)
    {
        _projection = projection;
    }

    public int Compare(T x, T y)
    {
        return _projection(x, y);
    }
}