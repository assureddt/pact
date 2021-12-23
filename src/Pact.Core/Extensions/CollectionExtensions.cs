using System.Linq.Expressions;
using System.Reflection;
using Pact.Core.Comparers;
using Pact.Core.Models;

namespace Pact.Core.Extensions;

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
    /// Takes a source list (presuming distinct and in the existing order) and shifts the item matching an identifier either up or down in the order
    /// then (optionally) normalizes all order values
    /// </summary>
    /// <typeparam name="T">The type of the list items</typeparam>
    /// <typeparam name="TIdentifier">The type of the identifier</typeparam>
    /// <param name="items">The source list to adapt</param>
    /// <param name="id">The identifier value we're shifting</param>
    /// <param name="shift">Determines direction of shift (decrement/increment) </param>
    /// <param name="idSelector">An expression to determine the Identifier property</param>
    /// <param name="orderSelector">An expression to determine the Order property</param>
    /// <param name="normalize">An expression to determine the Order property</param>
    public static void UpdateOrderAndNormalize<T, TIdentifier>(this IList<T> items,
        TIdentifier id,
        OrderShift shift, 
        Expression<Func<T, TIdentifier>> idSelector,
        Expression<Func<T, int>> orderSelector,
        bool normalize = true)
        where TIdentifier : struct
    {
        if (normalize)
            items.NormalizeOrder(orderSelector);

        for (var i = 0; i < items.Count; i++)
        {
            if (!items[i].GetPropertyValue(idSelector).Equals(id)) continue;

            if (shift == OrderShift.Decrement)
            {
                if (i >= 1)
                {
                    var newOrder = items[i - 1].GetPropertyValue(orderSelector);
                    items[i - 1].SetPropertyValue(orderSelector, items[i].GetPropertyValue(orderSelector));
                    items[i].SetPropertyValue(orderSelector, newOrder);
                }
                else
                    items[i].SetPropertyValue(orderSelector, 0);
            }
            else
            {
                if (i < items.Count - 1)
                {
                    var newOrder = items[i + 1].GetPropertyValue(orderSelector);
                    items[i + 1].SetPropertyValue(orderSelector, items[i].GetPropertyValue(orderSelector));
                    items[i].SetPropertyValue(orderSelector, newOrder);
                }
                else
                    items[i].SetPropertyValue(orderSelector, items.Max(x => x.GetPropertyValue(orderSelector)) + 1);
            }
        }

        if (!normalize) return;

        items.NormalizeOrder(orderSelector);
    }
    public enum OrderShift
    {
        Decrement,
        Increment
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

    private static readonly Random Rnd = new();  

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
            (list[k], list[n]) = (list[n], list[k]);
        }  
    }

    /// <summary>
    /// Removes soft delete items if the "SoftDelete" property is present and true
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IEnumerable<T> SoftDelete<T>(this IEnumerable<T> source) where T : class
    {
        var softDeleteProp = typeof(T).GetProperty("SoftDelete", typeof(bool));
        if (softDeleteProp == null) return source;

        var item = Expression.Parameter(typeof(T), "x");
        var prop = Expression.Property(item, "SoftDelete");
        var falseConstant = Expression.Constant(false);
        var equal = Expression.Equal(prop, falseConstant);
        var lambda = Expression.Lambda<Func<T, bool>>(equal, item);
        source = source.Where(lambda.Compile());

        return source;
    }

    /// <summary>
    /// Filters enumerable using the search term.
    /// If <see cref="FilterAttribute"/> & <see cref="IgnoreFilterAttribute"/> are present on the class these are used to determine what properties to filter on.
    /// If no filter arbitrates are present it checks all string fields.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="search"></param>
    /// <returns></returns>
    public static IEnumerable<T> TextFilter<T>(this IEnumerable<T> source, string search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return source;

        search = search.Trim().ToLower();

        //Look for specifically set properties to filter on
        var propertyInfos = typeof(T).GetProperties().Where(w => (w.PropertyType == typeof(string) || w.PropertyType == typeof(List<string>)) && w.CustomAttributes.Count(c => c.AttributeType == typeof(FilterAttribute)) > 0).ToList();

        //If none found use all strings which are not filtered out.
        if (propertyInfos.Count < 1)
            propertyInfos = typeof(T).GetProperties().Where(w => (w.PropertyType == typeof(string) || w.PropertyType == typeof(List<string>)) && w.CustomAttributes.Count(c => c.AttributeType == typeof(IgnoreFilterAttribute)) < 1).ToList();

        var output = new List<T>();
        if (!propertyInfos.Any()) return output;

        foreach (var item in source)
        {
            foreach (var prop in propertyInfos)
            {
                if (prop.PropertyType == typeof(string))
                {
                    var value = (string)prop.GetValue(item, null);
                    if (value == null || !value.ToLower().Contains(search)) continue;

                    output.Add(item);
                    break;
                }

                if (prop.PropertyType != typeof(List<string>)) continue;

                var added = false;
                var values = (List<string>)prop.GetValue(item, null);
                if (values != null && values.Any())
                {
                    if (values.Any(value => !string.IsNullOrWhiteSpace(value) && value.ToLower().Contains(search)))
                    {
                        output.Add(item);
                        added = true;
                    }
                }

                if (added)
                    break;
            }
        }
        return output;
    }

    /// <summary>
    /// Extends method which allow to sort by string field name.
    /// Allow to use a relative object definition for sorting (ex:LinkedObject.FieldsName1)
    /// </summary>
    /// <typeparam name="T">Current Object type for query</typeparam>
    /// <param name="source">list of defined object</param>
    /// <param name="sortExpression">string name of the field we want to sort by</param>
    /// <returns>Query sorted by sortExpression</returns>
    public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> source, string sortExpression) where T : class
    {
        var expressionParts = sortExpression.Split(' ');
        var orderByProperty = expressionParts[0];

        var propertyInfo = typeof(T).GetProperties().FirstOrDefault(x => x.Name.ToLowerInvariant() == orderByProperty.ToLowerInvariant());

        if (propertyInfo == null)
            throw new Exception("Cant find property '" + orderByProperty + "' on type '" + typeof(T).Name + "'");

        if (expressionParts.Length > 1 && expressionParts[1] == "DESC")
            return source.OrderByDescending(x => propertyInfo.GetValue(x, null));
        return source.OrderBy(x => propertyInfo.GetValue(x, null));
    }

    /// <summary>
    /// Extends method which allow to sort further by additional string field name.
    /// Allow to use a relative object definition for sorting (ex:LinkedObject.FieldsName1)
    /// </summary>
    /// <typeparam name="T">Current Object type for query</typeparam>
    /// <param name="source">list of defined object</param>
    /// <param name="sortExpression">string name of the field we want to sort by</param>
    /// <returns>Query sorted by sortExpression</returns>
    public static IOrderedEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> source, string sortExpression) where T : class
    {
        var expressionParts = sortExpression.Split(' ');
        var orderByProperty = expressionParts[0];

        var propertyInfo = typeof(T).GetProperties().FirstOrDefault(x => x.Name.ToLowerInvariant() == orderByProperty.ToLowerInvariant());

        if (propertyInfo == null)
            throw new Exception("Cant find property '" + orderByProperty + "' on type '" + typeof(T).Name + "'");

        if (expressionParts.Length > 1 && expressionParts[1] == "DESC")
            return source.ThenByDescending(x => propertyInfo.GetValue(x, null));
        return source.ThenBy(x => propertyInfo.GetValue(x, null));
    }

    /// <summary>
    /// Removes soft delete items if the "SoftDelete" property is present and true
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IQueryable<T> SoftDelete<T>(this IQueryable<T> source) where T : class
    {
        var softDeleteProp = typeof(T).GetProperty("SoftDelete", typeof(bool));
        if (softDeleteProp != null)
        {
            var item = Expression.Parameter(typeof(T), "x");
            var prop = Expression.Property(item, "SoftDelete");
            var falseConstant = Expression.Constant(false);
            var equal = Expression.Equal(prop, falseConstant);
            var lambda = Expression.Lambda<Func<T, bool>>(equal, item);
            source = source.Where(lambda);
        }

        return source;
    }

    /// <summary>
    /// Extends method which allow to sort by string field name.
    /// Allow to use a relative object definition for sorting (ex:LinkedObject.FieldsName1)
    /// </summary>
    /// <typeparam name="TEntity">Current Object type for query</typeparam>
    /// <param name="source">list of defined object</param>
    /// <param name="sortExpression">string name of the field we want to sort by</param>
    /// <returns>Query sorted by sortExpression</returns>
    public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string sortExpression) where TEntity : class
    {
        var type = typeof(TEntity);
        // Remember that for ascending order GridView just returns the column name and
        // for descending it returns column name followed by DESC keyword
        // Therefore we need to examine the sortExpression and separate out Column Name and
        // order (ASC/DESC)
        var expressionParts = sortExpression.Split(' '); // Assuming sortExpression is like [ColumnName DESC] or [ColumnName]
        var orderByProperty = expressionParts[0];
        var methodName = "OrderBy";
        //if sortDirection is descending
        if (expressionParts.Length > 1 && expressionParts[1] == "DESC")
        {
            const string sortDirection = "Descending";
            methodName += sortDirection; // Add sort direction at the end of Method name
        }

        MethodCallExpression resultExp;
        if (!orderByProperty.Contains("."))
        {
            var property = type.GetProperty(orderByProperty, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            resultExp = Expression.Call(typeof(Queryable), methodName,
                new[] { type, property.PropertyType },
                source.Expression, Expression.Quote(orderByExp));
        }
        else
        {
            var relationType = type.GetProperty(orderByProperty.Split('.')[0], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).PropertyType;
            var relationProperty = type.GetProperty(orderByProperty.Split('.')[0], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            var relationProperty2 = relationType.GetProperty(orderByProperty.Split('.')[1], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, relationProperty);
            var propertyAccess2 = Expression.MakeMemberAccess(propertyAccess, relationProperty2);
            var orderByExp = Expression.Lambda(propertyAccess2, parameter);
            resultExp = Expression.Call(typeof(Queryable), methodName,
                new[] { type, relationProperty2.PropertyType },
                source.Expression, Expression.Quote(orderByExp));
        }

        return source.Provider.CreateQuery<TEntity>(resultExp);
    }

    /// <summary>
    /// Allow to add another sorting on a query with a string representation of the field to sort by.
    /// </summary>
    /// <typeparam name="TEntity">Current Object type for query</typeparam>
    /// <param name="source">list of defined object</param>
    /// <param name="sortExpression">string name of the field we want to sort by</param>
    /// <returns>Query sorted by sortExpression</returns>
    public static IQueryable<TEntity> ThenBy<TEntity>(this IQueryable<TEntity> source, string sortExpression) where TEntity : class
    {
        var type = typeof(TEntity);
        // Remember that for ascending order GridView just returns the column name and
        // for descending it returns column name followed by DESC keyword
        // Therefore we need to examine the sortExpression and separate out Column Name and
        // order (ASC/DESC)
        var expressionParts = sortExpression.Split(' '); // Assuming sortExpression is like [ColumnName DESC] or [ColumnName]
        var orderByProperty = expressionParts[0];
        var methodName = "ThenBy";
        //if sortDirection is descending
        if (expressionParts.Length > 1 && expressionParts[1] == "DESC")
        {
            const string sortDirection = "Descending";
            methodName += sortDirection; // Add sort direction at the end of Method name
        }

        MethodCallExpression resultExp;
        if (!orderByProperty.Contains("."))
        {
            var property = type.GetProperty(orderByProperty, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            resultExp = Expression.Call(typeof(Queryable), methodName,
                new[] { type, property.PropertyType },
                source.Expression, Expression.Quote(orderByExp));
        }
        else
        {
            var relationType = type.GetProperty(orderByProperty.Split('.')[0], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).PropertyType;
            var relationProperty = type.GetProperty(orderByProperty.Split('.')[0], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            var relationProperty2 = relationType.GetProperty(orderByProperty.Split('.')[1], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, relationProperty);
            var propertyAccess2 = Expression.MakeMemberAccess(propertyAccess, relationProperty2);
            var orderByExp = Expression.Lambda(propertyAccess2, parameter);
            resultExp = Expression.Call(typeof(Queryable), methodName,
                new[] { type, relationProperty2.PropertyType },
                source.Expression, Expression.Quote(orderByExp));
        }

        return (IOrderedQueryable<TEntity>)source.Provider.CreateQuery<TEntity>(resultExp);
    }
}