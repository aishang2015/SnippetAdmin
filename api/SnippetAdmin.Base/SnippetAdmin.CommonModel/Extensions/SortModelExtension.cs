using System.Linq.Expressions;

namespace SnippetAdmin.CommonModel.Extensions
{
    public static class SortModelExtension
    {
        public static IQueryable<T> Sort<T>(this IQueryable<T> query, SortModel[] sorts)
        {
            // 创建表达式变量参数
            var parameter = Expression.Parameter(typeof(T), "o");

            if (sorts != null)
            {
                for (int i = 0; i < sorts.Length; i++)
                {
                    // 根据属性名获取属性
                    var sort = sorts[i];

                    if (typeof(T).GetProperties().All(p => p.Name != sort.PropertyName))
                    {
                        throw new ErrorSortPropertyException();
                    }

                    var property = typeof(T).GetProperty(sort.PropertyName);

                    // 创建一个访问属性的表达式
                    var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                    var orderByExp = Expression.Lambda(propertyAccess, parameter);

                    string OrderName = i > 0 ?
                        sort.IsAsc ? "ThenBy" : "ThenByDescending" :
                        sort.IsAsc ? "OrderBy" : "OrderByDescending";

                    var resultExp = Expression.Call(typeof(Queryable), OrderName, new Type[] { typeof(T), property.PropertyType }, query.Expression, Expression.Quote(orderByExp));
                    query = query.Provider.CreateQuery<T>(resultExp);
                }
            }
            return query;
        }
    }
}
