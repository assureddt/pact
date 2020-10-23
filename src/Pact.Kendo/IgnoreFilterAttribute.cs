using System;

namespace Pact.Kendo
{
    /// <summary>
    /// Apply to property to say we are not going to filter on it.
    /// Used with <see cref="EnumerableExtensions.TextFilter{T}(System.Collections.Generic.IEnumerable{T}, string)"/> and <see cref="QueryableExtensions.TextFilter{T}(System.Linq.IQueryable{T}, string)"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreFilterAttribute : Attribute
    {
    }
}
