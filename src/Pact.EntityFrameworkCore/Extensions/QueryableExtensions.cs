using Microsoft.EntityFrameworkCore;
using Pact.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;

namespace Pact.EntityFrameworkCore.Extensions
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Filters enumerable using the search term.
        /// If <see cref="FilterAttribute"/> & <see cref="IgnoreFilterAttribute"/> are present on the class these are used to determine what properties to filter on.
        /// If no filter arbitrates are present it checks all string fields.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public static IQueryable<T> TextFilter<T>(this IQueryable<T> source, string search) where T : class
        {
            if (string.IsNullOrWhiteSpace(search))
                return source;

            search = search.Trim().ToLower();

            //Look for specifically set properties to filter on
            var propertyInfos = typeof(T).GetProperties().Where(w => w.PropertyType == typeof(string) && w.CustomAttributes.Any(c => c.AttributeType == typeof(FilterAttribute)) && w.CustomAttributes.All(c => c.AttributeType != typeof(NotMappedAttribute))).ToList();

            //If none found use all strings which are not filtered out.
            if (propertyInfos.Count < 1)
                propertyInfos = typeof(T).GetProperties().Where(w => w.PropertyType == typeof(string) && !w.CustomAttributes.Any(c => c.AttributeType == typeof(NotMappedAttribute) || c.AttributeType == typeof(IgnoreFilterAttribute))).ToList();

            var item = Expression.Parameter(typeof(T), "x");
            var searchConstant = Expression.Constant("%" + search + "%");
            var likeMethod = typeof(DbFunctionsExtensions).GetMethod("Like", new[] { typeof(DbFunctions), typeof(string), typeof(string) });

            if (likeMethod == null)
                throw new Exception("Contains method not found on string in the .net framework");

            var methodCallExpressions = new List<MethodCallExpression>();
            foreach (var propertyInfo in propertyInfos)
            {
                var property = Expression.Property(item, propertyInfo);
                var searchExpression = Expression.Call(null, likeMethod, Expression.Constant(EF.Functions), property, searchConstant);
                methodCallExpressions.Add(searchExpression);
            }

            if (methodCallExpressions.Count < 0)
                return source;
            if (methodCallExpressions.Count == 1)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(methodCallExpressions.First(), item);
                return source.Where(lambda);
            }

            Expression expressionOr = null;
            foreach (var methodCallExpression in methodCallExpressions)
            {
                if (methodCallExpressions.First() == methodCallExpression)
                    continue;
                if (methodCallExpressions.IndexOf(methodCallExpression) == 1)
                    expressionOr = Expression.OrElse(methodCallExpressions.First(), methodCallExpression);
                else if (expressionOr != null)
                    expressionOr = Expression.OrElse(expressionOr, methodCallExpression);
            }

            if (expressionOr != null)
            {
                var lambdaOr = Expression.Lambda<Func<T, bool>>(expressionOr, item);
                return source.Where(lambdaOr);
            }

            return source;
        }
    }
}
