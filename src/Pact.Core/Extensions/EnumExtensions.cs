using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Pact.Core.Extensions;

public static class EnumExtensions
{
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