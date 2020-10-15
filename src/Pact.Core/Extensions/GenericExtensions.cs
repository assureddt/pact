using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Pact.Core.Extensions
{
    public static class GenericExtensions
    {
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
