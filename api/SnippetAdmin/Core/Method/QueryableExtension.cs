using SnippetAdmin.Core.Method;
using System.Linq.Expressions;

namespace SnippetAdmin.Core.Method
{
    public static class QueryableExtension
    {
        /// <summary>
        /// 拼接条件(直接拼接）
        /// </summary>
        public static IQueryable<T> AndIf<T>(this IQueryable<T> queryable, Expression<Func<T, bool>> predicate) where T : class
            => queryable.Where(predicate);

        /// <summary>
        /// 拼接条件(直接拼接）
        /// </summary>
        public static IQueryable<T> AndIf<T>(this IQueryable<T> queryable, bool condition, Expression<Func<T, bool>> predicate) where T : class
            => condition ? queryable.Where(predicate) : queryable;

        /// <summary>
        /// 拼接条件（字符串不为空）
        /// </summary>
        public static IQueryable<T> AndIfExist<T>(this IQueryable<T> queryable, string value,
            Expression<Func<T, bool>> predicate) where T : class
            => string.IsNullOrEmpty(value) ? queryable : queryable.Where(predicate);

        /// <summary>
        /// 拼接条件（对象不为空）
        /// </summary>
        public static IQueryable<T> AndIfExist<T, TData>(this IQueryable<T> queryable, TData value,
            Expression<Func<T, bool>> predicate) where T : class
            => value is null ? queryable : queryable.Where(predicate);

        /// <summary>
        /// 拼接多个排序条件
        /// </summary>
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> queryable, params (Expression<Func<T, object>>, bool)[] keySelectors) where T : class
        {
            foreach (var keySelector in keySelectors)
            {
                if (queryable is IOrderedQueryable<T>)
                {
                    queryable = keySelector.Item2 ?
                           (queryable as IOrderedQueryable<T>).ThenBy(keySelector.Item1) :
                           (queryable as IOrderedQueryable<T>).ThenByDescending(keySelector.Item1);
                }
                else
                {
                    queryable = keySelector.Item2 ?
                        queryable.OrderBy(keySelector.Item1) :
                        queryable.OrderByDescending(keySelector.Item1);

                }
            }
            return queryable;
        }

        /// <summary>
        /// 取得分页
        /// </summary>
        public static IQueryable<T> QueryPage<T>(this IQueryable<T> queryable, int page, int size)
        {
            return queryable.Skip((page - 1) * size).Take(size);
        }
    }
}