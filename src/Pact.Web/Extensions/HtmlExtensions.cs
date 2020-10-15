using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Pact.Web.Extensions
{
    public static class HtmlExtensions
    {
        public static string DescriptionFor<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            DisplayAttribute descriptionAttribute = null;
            if (expression.Body is MemberExpression memberExpression)
            {
                descriptionAttribute = memberExpression.Member
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .Cast<DisplayAttribute>()
                    .SingleOrDefault();
            }

            return descriptionAttribute?.Description;
        }
    }
}
