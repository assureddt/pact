using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pact.Core.Extensions;

namespace Pact.Web.Extensions
{
    public static class CollectionHelpers
    {
        
        public static IEnumerable<SelectListItem> WithDefault(this IEnumerable<SelectListItem> @this, string description)
        {
            var list = @this.ToList();
            list.Insert(0, new SelectListItem { Text = description, Value = "" });

            return list;
        }

        public static string GetDefaultIfNotValid(this string value, IEnumerable<SelectListItem> options)
        {
            var selectListItems = options as IList<SelectListItem> ?? options.ToList();

            return value == null || selectListItems.All(x => x.Value != value)
                ? selectListItems.FirstOrDefault()?.Value
                : value;
        }

        public static T? GetDefaultIfNotValid<T>(this T? value, IEnumerable<SelectListItem> options) where T: struct
        {
            var selectListItems = options as IList<SelectListItem> ?? options.ToList();

            return value == null || selectListItems.All(x => x.Value != value.ToString())
                ? selectListItems.FirstOrDefault()?.Value.ToNullable<T>()
                : value;
        }

        public static T GetDefaultIfNotValid<T>(this T value, IEnumerable<SelectListItem> options) where T : struct
        {
            var selectListItems = options as IList<SelectListItem> ?? options.ToList();

            return (T) (selectListItems.All(x => x.Value != value.ToString())
                ? (typeof(T).IsEnum ? Enum.Parse(typeof(T), selectListItems.First().Value): Convert.ChangeType(selectListItems.First().Value, typeof(T)))
                : value);
        }
    }
}
