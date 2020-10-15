using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Pact.Core.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Normalizes a specified order property value on a collection to be 1-based
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection containing the property to be normalized</param>
        /// <param name="propSelector">A selection expression for the integer property being normalized</param>
        /// <param name="baseIndex">The base number to normalize from</param>
        public static void NormalizeOrder<T>(this ICollection<T> collection, Expression<Func<T, int>> propSelector, int baseIndex = 1)
        {
            var index = baseIndex;

            var reordered = collection.OrderBy(x => x.GetPropertyValue(propSelector));
            foreach (var o in reordered)
            {
                o.SetPropertyValue(propSelector, index++);
            }
        }

        /// <summary>
        /// Formats an unordered HTML list for the designated property in a collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this">The collection to extract the list from</param>
        /// <param name="propSelector">A selector function for the property to display</param>
        /// <param name="preventWrap">Introduces non-breaking spaces to the items</param>
        /// <param name="listClass">An optional CSS class to apply to the unordered list element</param>
        /// <returns>The raw HTML string</returns>
        public static string FormatListForTooltip<T>(this IEnumerable<T> @this, Func<T, string> propSelector, bool preventWrap = true, string listClass = null)
        {
            const int maxItems = 10;
            var enumerable = @this as IList<T> ?? @this.ToList();
            if (!enumerable.Any()) return "";

            var classText = listClass == null ? "" : $" class='{listClass}'";

            var needsElipsis = enumerable.Count > maxItems;

            if (needsElipsis) enumerable = enumerable.Take(maxItems).ToList();

            var output = $"<ul{classText}>{string.Join("", enumerable.Select(x => $"<li>{propSelector(x)}</li>"))}</ul>";
            if (preventWrap) output = output.Replace(" ", "&nbsp;");
            if (needsElipsis) output += "...";

            return output;
        }

        /// <summary>
        /// Abbreviation for a quick CSV from a specified property in a collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this">The collection to use</param>
        /// <param name="propSelector">A selector function for the property to include, returning a string</param>
        /// <param name="separator">The separator string to use</param>
        /// <returns></returns>
        public static string PropertyCsv<T>(this ICollection<T> @this, Func<T, string> propSelector, string separator = ";")
        {
            return !@this.Any() ? "" : string.Join(separator, @this.Select(propSelector));
        }

        /// <summary>
        /// Fancy means of getting a set of all permutations from a set of values passed in, from here: https://blogs.msdn.microsoft.com/ericlippert/2010/06/28/computing-a-cartesian-product-with-linq/
        /// e.g.
        /// [
        ///     [1, 2],
        ///     [1, 2]
        /// ]
        /// =>
        /// [[1, 1], [1, 2], [2, 1], [2, 2]]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequences">The seed set</param>
        /// <returns>The cartesian product set</returns>
        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> result = new[] { Enumerable.Empty<T>() };
            return sequences.Aggregate(result, (current, s) => (from seq in current from item in s select seq.Concat(new[] {item})));
        }

        /// <summary>
        /// Flattens a hierarchical tree of objects containing specified child collections
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The top level nodes</param>
        /// <param name="selector">A selector function for the child collection in each node</param>
        /// <returns>A flat collection of all objects in the tree (not necessarily distinct)</returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            foreach (var element in source)
            {
                yield return element;

                var children = selector(element);
                if (children == null) continue;

                foreach (var nested in Flatten(children, selector))
                {
                    yield return nested;
                }
            }
        }

        /// <summary>
        /// Order a collection to follow the index order defined in another collection
        /// </summary>
        /// <typeparam name="T">The collection type</typeparam>
        /// <typeparam name="T2">The index type</typeparam>
        /// <param name="source">The collection to reorder</param>
        /// <param name="innerIdSelector">The selector function for the index field in the collection</param>
        /// <param name="index">The index defining the desired order</param>
        /// <returns>The re-ordered collection</returns>
        public static IEnumerable<T> OrderByIndex<T, T2>(this IEnumerable<T> source, Func<T, T2> innerIdSelector, T2[] index)
        {
            return source.OrderBy(x =>
            {
                for (var i = 0; i < index.Length; i++)
                {
                    if (index[i].Equals(innerIdSelector(x)))
                        return i;
                }

                return -1;
            });
        }

        /// <summary>
        /// Determines object distinction purely based on a specified property (i.e. not object equality comparison)
        /// </summary>
        /// <typeparam name="T">The collection type</typeparam>
        /// <typeparam name="TValue">The type of the property to compare</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="projection">The selector function for the distinction property</param>
        /// <returns>The distinct collection</returns>
        public static IEnumerable<T> Distinct<T, TValue>(this IEnumerable<T> source, Func<T, TValue> projection)
        {
            return source.Distinct(GenericEqualityComparer<T>.Create(projection));
        }


        /// <summary>
        /// Removes a set of objects from a collection based on a specific property value
        /// </summary>
        /// <typeparam name="T">The collection type</typeparam>
        /// <typeparam name="TValue">The type of the property to compare</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="second">The collection of objects to be excluded</param>
        /// <param name="projection">The selector function for the distinction property</param>
        /// <returns>The collection without the specified elements</returns>
        public static IEnumerable<T> Except<T, TValue>(this IEnumerable<T> source, IEnumerable<T> second, Func<T, TValue> projection)
        {
            return source.Except(second, GenericEqualityComparer<T>.Create(projection));
        }

        private static readonly Random Rnd = new Random();  

        /// <summary>
        /// Random re-ordering of a collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">Th collection</param>
        public static void Shuffle<T>(this IList<T> list)  
        {  
            var n = list.Count;  
            while (n > 1) {  
                n--;  
                var k = Rnd.Next(n + 1);  
                var value = list[k];  
                list[k] = list[n];  
                list[n] = value;  
            }  
        }
    }
}
