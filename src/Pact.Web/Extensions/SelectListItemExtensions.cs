using Microsoft.AspNetCore.Mvc.Rendering;
using Pact.Core.Extensions;

namespace Pact.Web.Extensions;

public static class SelectListItemExtensions
{
    /// <summary>
    /// Insert a default option with empty value at the start of the list
    /// </summary>
    /// <param name="this"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    public static ICollection<SelectListItem> WithDefault(this ICollection<SelectListItem> @this, string description)
    {
        var list = @this.ToList();
        list.Insert(0, new SelectListItem { Text = description, Value = "" });

        return list;
    }

    /// <summary>
    /// Returns the first option value if the one passed is not present in the collection
    /// </summary>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string GetDefaultIfNotValid(this string value, ICollection<SelectListItem> options)
    {
        var selectListItems = options as IList<SelectListItem> ?? options.ToList();

        return value == null || selectListItems.All(x => x.Value != value)
            ? selectListItems.FirstOrDefault()?.Value
            : value;
    }

    /// <summary>
    /// Returns the first option value if the one passed is not present in the collection
    /// </summary>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static T? GetDefaultIfNotValid<T>(this T? value, ICollection<SelectListItem> options) where T: struct
    {
        var selectListItems = options as IList<SelectListItem> ?? options.ToList();

        return value == null || selectListItems.All(x => x.Value != value.ToString())
            ? selectListItems.FirstOrDefault()?.Value.ToNullable<T>()
            : value;
    }

    /// <summary>
    /// Returns the first option value if the one passed is not present in the collection
    /// </summary>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static T GetDefaultIfNotValid<T>(this T value, ICollection<SelectListItem> options) where T : struct
    {
        var selectListItems = options as IList<SelectListItem> ?? options.ToList();

        return (T) (selectListItems.All(x => x.Value != value.ToString())
            ? typeof(T).IsEnum ? Enum.Parse(typeof(T), selectListItems.First().Value): Convert.ChangeType(selectListItems.First().Value, typeof(T))
            : value);
    }
}