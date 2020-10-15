using System;
using System.Collections.Generic;
using System.Linq;
using Core.Helpers;

namespace Pact.Core.Extensions
{
    public static class CollectionHelpers
    {
        public static void ForEach<T>(this IEnumerable<T> @this, Action<T> action)
        {
            foreach (var item in @this)
            {
                action(item);
            }
        }

        public static void Times(this int count, Action action)
        {
            for (var i = 0; i < count; i++)
            {
                action();
            }
        }

        public static void NormalizeOrder(this IEnumerable<IOrderAttributes> order)
        {
            var index = 1;
            var normalizeOrder = order as IOrderAttributes[] ?? order.ToArray();
            foreach (var o in normalizeOrder.OrderBy(x => x.Order))
            {
                o.Order = index++;
            }
        }

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

        public static string PropertyCsv<T>(this IEnumerable<T> @this, Func<T, object> propSelector, string separator = ";")
        {
            var enumerable = @this as IList<T> ?? @this.ToList();
            return !enumerable.Any() ? "" : string.Join(separator, enumerable.Select(propSelector).ToString());
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
        /// <param name="sequences"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> result = new[] { Enumerable.Empty<T>() };
            return sequences.Aggregate(result, (current, s) => (from seq in current from item in s select seq.Concat(new[] {item})));
        }

        public static void ApplyChanges<T>(this ICollection<T> list, ICollection<T> newSet, Func<T, int> innerIdSelector)
        {
            var toRemove = list.Where(existing => newSet.All(x => innerIdSelector(x) != innerIdSelector(existing))).ToList();

            foreach (var rem in toRemove)
                list.Remove(rem);

            var toAdd = newSet.Where(toInclude => list.All(x => innerIdSelector(x) != innerIdSelector(toInclude))).ToList();

            foreach (var add in toAdd)
                list.Add(add);
        }

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

        public static IEnumerable<T> Distinct<T, TValue>(this IEnumerable<T> source, Func<T, TValue> projection)
        {
            return source.Distinct(GenericEqualityComparer<T>.Create(projection));
        }


        public static IEnumerable<T> Except<T, TValue>(this IEnumerable<T> source, IEnumerable<T> second, Func<T, TValue> projection)
        {
            return source.Except(second, GenericEqualityComparer<T>.Create(projection));
        }

        private static readonly Random Rnd = new Random();  

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
