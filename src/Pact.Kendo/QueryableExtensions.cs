using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Pact.Kendo
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Kendo data source support for soft delete, text filtering and sorting
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="kendoDataRequest"></param>
        /// <returns></returns>
        public static IQueryable<T> Kendo<T>(this IQueryable<T> source, KendoDataRequest kendoDataRequest) where T : class
        {
            //Remove soft delete
            source = Core.Extensions.CollectionExtensions.SoftDelete(source);

            //Kendo text filter
            source = EntityFrameworkCore.Extensions.QueryableExtensions.TextFilter(source, kendoDataRequest.TextFilter);

            //Sort
            if (kendoDataRequest.Sort != null)
            {
                if (kendoDataRequest.Sort.Count > 0)
                    source = Core.Extensions.CollectionExtensions.OrderBy(source, kendoDataRequest.Sort.First().ToString());
                if (kendoDataRequest.Sort.Count > 1)
                    source = Core.Extensions.CollectionExtensions.ThenBy(source, kendoDataRequest.Sort[0].ToString());
            }

            return source;
        }

        /// <summary>
        /// Kendo data source support for soft delete, text filtering and sorting
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="kendoDataRequest"></param>
        /// <returns></returns>
        public static async Task<JsonResult> KendoResultAsync<T>(this IQueryable<T> source, KendoDataRequest kendoDataRequest) where T : class
        {
            //Execute
            var items = await source.ToListAsync();

            //Get the count for kendo ui grids
            var count = items.Count;

            //Skip/Take
            items = items.Skip(kendoDataRequest.Skip).Take(kendoDataRequest.Take).ToList();

            return new JsonResult(new KendoResult<T> { Result = "OK", Records = items, Count = count });
        }
    }
}
