using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pact.Core.Extensions;

namespace Pact.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Similar to IdFor etc., just using display attribute for description
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="html"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string DescriptionFor<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression) =>
            (expression?.Body as MemberExpression)?.Member?.GetDescription();
    }
}
