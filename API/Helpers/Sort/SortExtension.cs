using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ToDoAPI.API.Helpers.Sort
{
    public static class SortExtension
    {
        public static IQueryable<TEntity> Sort<TEntity>(this IQueryable<TEntity> query, ISort sort)
        {
            //https://www.tabsoverspaces.com/229310-sorting-in-iqueryable-using-string-as-column-name?utm_source=blog.cincura.net
            if (!string.IsNullOrEmpty(sort.SortBy))
            {
                Dictionary<string, PropertyInfo> properties = typeof(TEntity).GetProperties().ToDictionary(pi => pi.Name.ToLower());
                string[] sorts = MakeSortList(sort.SortBy);

                foreach (string sortBy in sorts)
                {
                    PropertyInfo property = properties.GetValueOrDefault(sortBy.Substring(1).ToLower());
                    if (property != null)
                    {
                        ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity));
                        Expression expression = Expression.MakeMemberAccess(parameterExpression, property);
                        LambdaExpression orderFunc = Expression.Lambda(expression, parameterExpression);

                        MethodCallExpression call = Expression.Call(
                            typeof(Queryable),
                            (sortBy.Equals(sorts.First()) ? "OrderBy" : "ThenBy") + (sortBy.First() == '-' ? "Descending" : string.Empty),
                            new[] { typeof(TEntity), property.PropertyType },
                            query.Expression,
                            Expression.Quote(orderFunc)
                        );
                        query = query.Provider.CreateQuery<TEntity>(call);
                    }
                }
            }
            return query;
        }

        private static string[] MakeSortList(string sort)
        {
            string pattern = @"^(\w)";
            Regex regex = new Regex(pattern);
            string[] sortList = sort.Split('.');
            for (int i = 0; i < sortList.Length; i++)
            {
                if (regex.IsMatch(sortList[i])) sortList[i] = "+" + sortList[i];
            }
            return sortList;
        }
    }
}