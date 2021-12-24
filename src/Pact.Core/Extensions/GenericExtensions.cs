using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace Pact.Core.Extensions;

public static class GenericExtensions
{
    /// <summary>
    /// Returns a Json formatted string representation of the object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="options">Optional options to override the default STJ ones</param>
    /// <returns></returns>
    /// <remarks>NOTE: we've moved away from overriding global defaults as it's probably a net-bad thing - the caller is always now in control of the serialization options</remarks>
    public static string ToJson<T>(this T obj, JsonSerializerOptions options = null) => JsonSerializer.Serialize(obj, options);

    /// <summary>
    /// Set a property value via an expression (involves reflection)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="target"></param>
    /// <param name="propSelector"></param>
    /// <param name="value"></param>
    public static void SetPropertyValue<T, TValue>(this T target, Expression<Func<T, TValue>> propSelector, TValue value)
    {
        if (propSelector.Body is not MemberExpression memberSelectorExpression) return;

        var property = memberSelectorExpression.Member as PropertyInfo;
        if (property != null)
        {
            if (property.CanWrite)
            {
                property.SetValue(target, value, null);
                return;
            }
        }

        var field = memberSelectorExpression.Member as FieldInfo;
        if (field != null)
        {
            field.SetValue(target, value);
        }
    }

    /// <summary>
    /// Get a property value via an expression (involves reflection)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="target"></param>
    /// <param name="propSelector"></param>
    /// <returns></returns>
    public static TValue GetPropertyValue<T, TValue>(this T target, Expression<Func<T, TValue>> propSelector)
    {
        if (propSelector.Body is not MemberExpression memberSelectorExpression) return default;

        var property = memberSelectorExpression.Member as PropertyInfo;
        if (property != null)
        {
            if (property.CanRead)
            {
                return (TValue)property.GetValue(target, null);
            }
        }

        var field = memberSelectorExpression.Member as FieldInfo;
        if (field != null)
        {
            return (TValue)field.GetValue(target);
        }

        return default;
    }
}