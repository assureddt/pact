using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Pact.Kendo;

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
        if (kendoDataRequest.Sort?.Any() != true) return source;

        source = Core.Extensions.CollectionExtensions.OrderBy(source, kendoDataRequest.Sort.First().ToString());
        source = kendoDataRequest.Sort.Skip(1).Aggregate(source, (current, sort) => Core.Extensions.CollectionExtensions.ThenBy(current, sort.ToString()));

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