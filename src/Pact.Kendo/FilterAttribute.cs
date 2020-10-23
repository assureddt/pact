using System;

namespace Pact.Kendo
{
    /// <summary>
    /// Sets if we should filters on this property.
    /// Used with <see cref="EnumerableExtensions.TextFilter{T}(System.Collections.Generic.IEnumerable{T}, string)"/> and <see cref="QueryableExtensions.TextFilter{T}(System.Linq.IQueryable{T}, string)"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FilterAttribute : Attribute
    {
    }
}
