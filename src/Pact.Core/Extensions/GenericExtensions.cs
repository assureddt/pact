using System;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;

namespace Pact.Core.Extensions
{
    public static class GenericExtensions
    {
        /// <summary>
        /// Returns a Json formatted string representation of the object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="indent">Prettify the output</param>
        /// <param name="ignoreNull">If true, properties with null values will be omitted from the output</param>
        /// <param name="stringEscape">If true, HTML content is escaped</param>
        /// <returns></returns>
        public static string ToJson<T>(this T obj, bool indent = false, bool ignoreNull = false, bool stringEscape = true) where T : class
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                Formatting = indent ? Formatting.Indented : Formatting.None,
                NullValueHandling = ignoreNull ? NullValueHandling.Ignore : NullValueHandling.Include,
                StringEscapeHandling = stringEscape ? StringEscapeHandling.EscapeHtml : StringEscapeHandling.Default
            });
        }

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
            if (!(propSelector.Body is MemberExpression memberSelectorExpression)) return;

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
            if (!(propSelector.Body is MemberExpression memberSelectorExpression)) return default;

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
}
