using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Pact.Kendo
{
    public static class SortExtender
    {
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
