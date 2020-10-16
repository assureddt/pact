using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Pact.Core.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value, null if not present</returns>
        /// <example>myEnumVariable.GetAttributeOfType<DescriptionAttribute>().Description;</example>
        public static T GetAttributeOfType<T>(this Enum enumVal) where T : Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo.FirstOrDefault()?.GetCustomAttributes(typeof(T), false);

            return (T)attributes?.FirstOrDefault();
        }

        /// <summary>
        /// Gets Name attribute metadata for an enum value, falling back to some alternatives if absent
        /// </summary>
        /// <param name="enumVal"></param>
        /// <returns></returns>
        public static string GetName(this Enum enumVal)
        {
            return enumVal.GetAttributeOfType<DisplayAttribute>()?.Name ??
                   enumVal.GetAttributeOfType<DisplayAttribute>()?.Description ??
                   enumVal.GetAttributeOfType<DescriptionAttribute>()?.Description;
        }

        /// <summary>
        /// Gets Description attribute metadata for an enum value, falling back to some alternatives if absent
        /// </summary>
        /// <param name="enumVal"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum enumVal)
        {
            return enumVal.GetAttributeOfType<DisplayAttribute>()?.Description ??
                   enumVal.GetAttributeOfType<DescriptionAttribute>()?.Description;
        }

        /// <summary>
        /// Gets Order attribute metadata for an enum value
        /// </summary>
        /// <param name="enumVal"></param>
        /// <returns></returns>
        public static int? GetOrder(this Enum enumVal)
        {
            return enumVal.GetAttributeOfType<DisplayAttribute>()?.Order;
        }

        /// <summary>
        /// Returns all the values in an enum as a collection, respecting the order attribute
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TEnum> Values<TEnum>()
            where TEnum : struct,  IComparable, IFormattable, IConvertible
        {
            var enumType = typeof(TEnum);

            if (!enumType.IsEnum) throw new ArgumentException();

            return GetWithOrder<TEnum>(enumType);
        }

        /// <summary>
        /// Returns all the values in an enum as a collection, respecting the order attribute
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<TEnum> GetWithOrder<TEnum>(this Type type)
            where TEnum : struct,  IComparable, IFormattable, IConvertible
        {
            return type.GetFields()
                .Where(field => field.IsStatic)
                .Select(field => new
                {
                    field,
                    attribute = field.GetCustomAttribute<DisplayAttribute>()
                })
                .Select(fieldInfo => new
                {
                    name = (TEnum)fieldInfo.field.GetValue(null),
                    order = fieldInfo.attribute?.GetOrder() ?? (int)fieldInfo.field.GetValue(null)
                })
                .OrderBy(field => field.order)
                .Select(field => field.name);
        }
    }
}
