using Microsoft.EntityFrameworkCore;
using Pact.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Pact.Web.Extensions
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Removes soft delete items if the "SoftDelete" property is present and true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IQueryable<T> SoftDelete<T>(this IQueryable<T> source) where T : class
        {
            var softDeleteProp = typeof(T).GetProperty("SoftDelete", typeof(bool));
            if (softDeleteProp != null)
            {
                var item = Expression.Parameter(typeof(T), "x");
                var prop = Expression.Property(item, "SoftDelete");
                var falseConstant = Expression.Constant(false);
                var equal = Expression.Equal(prop, falseConstant);
                var lambda = Expression.Lambda<Func<T, bool>>(equal, item);
                source = source.Where(lambda);
            }

            return source;
        }

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
            var propertyInfos = typeof(T).GetProperties().Where(w => w.PropertyType == typeof(string) && w.CustomAttributes.Count(c => c.AttributeType == typeof(FilterAttribute)) > 0 && w.CustomAttributes.Count(c => c.AttributeType == typeof(NotMappedAttribute)) < 1).ToList();

            //If none found use all strings which are not filtered out.
            if (propertyInfos == null || propertyInfos.Count < 1)
                propertyInfos = typeof(T).GetProperties().Where(w => w.PropertyType == typeof(string) && w.CustomAttributes.Count(c => c.AttributeType == typeof(NotMappedAttribute) || c.AttributeType == typeof(IgnoreFilterAttribute)) < 1).ToList();

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

        /// <summary>
        /// Extends method which allow to sort by string field name.
        /// Allow to use a relative object definition for sorting (ex:LinkedObject.FieldsName1)
        /// </summary>
        /// <typeparam name="TEntity">Current Object type for query</typeparam>
        /// <param name="source">list of defined object</param>
        /// <param name="sortExpression">string name of the field we want to sort by</param>
        /// <returns>Query sorted by sortExpression</returns>
        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string sortExpression) where TEntity : class
        {
            var type = typeof(TEntity);
            // Remember that for ascending order GridView just returns the column name and
            // for descending it returns column name followed by DESC keyword
            // Therefore we need to examine the sortExpression and separate out Column Name and
            // order (ASC/DESC)
            var expressionParts = sortExpression.Split(' '); // Assuming sortExpression is like [ColumnName DESC] or [ColumnName]
            var orderByProperty = expressionParts[0];
            var methodName = "OrderBy";
            //if sortDirection is descending
            if (expressionParts.Length > 1 && expressionParts[1] == "DESC")
            {
                const string sortDirection = "Descending";
                methodName += sortDirection; // Add sort direction at the end of Method name
            }

            MethodCallExpression resultExp;
            if (!orderByProperty.Contains("."))
            {
                var property = type.GetProperty(orderByProperty, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var parameter = Expression.Parameter(type, "p");
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                resultExp = Expression.Call(typeof(Queryable), methodName,
                new[] { type, property.PropertyType },
                source.Expression, Expression.Quote(orderByExp));
            }
            else
            {
                var relationType = type.GetProperty(orderByProperty.Split('.')[0], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).PropertyType;
                var relationProperty = type.GetProperty(orderByProperty.Split('.')[0], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var relationProperty2 = relationType.GetProperty(orderByProperty.Split('.')[1], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var parameter = Expression.Parameter(type, "p");
                var propertyAccess = Expression.MakeMemberAccess(parameter, relationProperty);
                var propertyAccess2 = Expression.MakeMemberAccess(propertyAccess, relationProperty2);
                var orderByExp = Expression.Lambda(propertyAccess2, parameter);
                resultExp = Expression.Call(typeof(Queryable), methodName,
                new[] { type, relationProperty2.PropertyType },
                source.Expression, Expression.Quote(orderByExp));
            }

            return source.Provider.CreateQuery<TEntity>(resultExp);
        }

        /// <summary>
        /// Allow to add another sorting on a query with a string representation of the field to sort by.
        /// </summary>
        /// <typeparam name="TEntity">Current Object type for query</typeparam>
        /// <param name="source">list of defined object</param>
        /// <param name="sortExpression">string name of the field we want to sort by</param>
        /// <returns>Query sorted by sortExpression</returns>
        public static IQueryable<TEntity> ThenBy<TEntity>(this IQueryable<TEntity> source, string sortExpression) where TEntity : class
        {
            var type = typeof(TEntity);
            // Remember that for ascending order GridView just returns the column name and
            // for descending it returns column name followed by DESC keyword
            // Therefore we need to examine the sortExpression and separate out Column Name and
            // order (ASC/DESC)
            var expressionParts = sortExpression.Split(' '); // Assuming sortExpression is like [ColumnName DESC] or [ColumnName]
            var orderByProperty = expressionParts[0];
            var methodName = "ThenBy";
            //if sortDirection is descending
            if (expressionParts.Length > 1 && expressionParts[1] == "DESC")
            {
                const string sortDirection = "Descending";
                methodName += sortDirection; // Add sort direction at the end of Method name
            }

            MethodCallExpression resultExp;
            if (!orderByProperty.Contains("."))
            {
                var property = type.GetProperty(orderByProperty, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var parameter = Expression.Parameter(type, "p");
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                resultExp = Expression.Call(typeof(Queryable), methodName,
                new[] { type, property.PropertyType },
                source.Expression, Expression.Quote(orderByExp));
            }
            else
            {
                var relationType = type.GetProperty(orderByProperty.Split('.')[0], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).PropertyType;
                var relationProperty = type.GetProperty(orderByProperty.Split('.')[0], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var relationProperty2 = relationType.GetProperty(orderByProperty.Split('.')[1], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var parameter = Expression.Parameter(type, "p");
                var propertyAccess = Expression.MakeMemberAccess(parameter, relationProperty);
                var propertyAccess2 = Expression.MakeMemberAccess(propertyAccess, relationProperty2);
                var orderByExp = Expression.Lambda(propertyAccess2, parameter);
                resultExp = Expression.Call(typeof(Queryable), methodName,
                new[] { type, relationProperty2.PropertyType },
                source.Expression, Expression.Quote(orderByExp));
            }

            return (IOrderedQueryable<TEntity>)source.Provider.CreateQuery<TEntity>(resultExp);
        }
    }
}
