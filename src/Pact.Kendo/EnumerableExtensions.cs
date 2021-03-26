using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Pact.Kendo
{
    /// <summary>
    /// A set of enumerable extensions to help with common functions
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Kendo data source support for soft delete, text filtering and sorting
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="kendoDataRequest">filer and pagination options</param>
        /// <returns></returns>
        public static IEnumerable<T> Kendo<T>(this IEnumerable<T> source, KendoDataRequest kendoDataRequest) where T : class
        {
            //Remove soft delete
            source = Core.Extensions.CollectionExtensions.SoftDelete(source);

            //Kendo text filter
            source = Core.Extensions.CollectionExtensions.TextFilter(source, kendoDataRequest.TextFilter);

            //Sort
            if (kendoDataRequest.Sort == null) return source;
            if (kendoDataRequest.Sort.Count > 0)
                source = Core.Extensions.CollectionExtensions.OrderBy(source, kendoDataRequest.Sort.First().ToString());

            return source;
        }

        /// <summary>
        /// Formats data source into json result kendo ui expects
        /// Applies pagination
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="kendoDataRequest">filer and pagination options</param>
        /// <returns></returns>
        public static JsonResult KendoResult<T>(this IEnumerable<T> source, KendoDataRequest kendoDataRequest) where T : class
        {
            //Execute
            var items = source.ToList();

            //Get the count for kendo ui grids
            var count = items.Count;

            //Skip/Take
            items = items.Skip(kendoDataRequest.Skip).Take(kendoDataRequest.Take).ToList();

            return new JsonResult(new KendoResult<T> { Result = "OK", Records = items, Count = count });
        }
    }
}
