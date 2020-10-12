using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;

namespace Pact.Kendo
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Kendo data source support for soft delete, text filtering and sorting
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static IEnumerable<T> Kendo<T>(this IEnumerable<T> source, KendoDataRequest r) where T : class
        {
            //Remove soft delete
            source = SoftDelete(source);

            //Kendo text filter
            source = TextFilter(source, r.TextFilter);

            //Sort
            if (r.Sort == null) return source;
            if (r.Sort.Count > 0)
                source = source.OrderBy(r.Sort.First().ToString());

            return source;
        }

        /// <summary>
        /// Removes soft delete item if the "SoftDelete" property is present and false
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
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static JsonResult KendoResult<T>(this IEnumerable<T> source, KendoDataRequest r) where T : class
        {
            //Execute
            var items = source.ToList();

            //Get the count for kendo ui grids
            var count = items.Count;

            //Skip/Take
            items = items.Skip(r.Skip).Take(r.Take).ToList();

            return new JsonResult(new KendoResult<T> { Result = "OK", Records = items, TotalRecordCount = count });
        }

        /// <summary>
        /// Checks if any of the string properties on the object contain the search term
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
        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> source, string sortExpression) where T : class
        {
            var expressionParts = sortExpression.Split(' ');
            var orderByProperty = expressionParts[0];

            var propertyInfo = typeof(T).GetProperties().FirstOrDefault(x=>x.Name.ToLowerInvariant() == orderByProperty.ToLowerInvariant());

            if(propertyInfo == null)
                throw new Exception("Cant find property '" + orderByProperty +"' on type '" + typeof(T).Name + "'");

            if (expressionParts.Length > 1 && expressionParts[1] == "DESC")
                return source.OrderByDescending(x => propertyInfo.GetValue(x, null));
            return source.OrderBy(x => propertyInfo.GetValue(x, null));
        }
    }
}
