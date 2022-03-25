using FluentValidation;
using SnippetAdmin.Constants;
using SnippetAdmin.Core;
using SnippetAdmin.Core.Exceptions;
using System.Linq.Expressions;

namespace SnippetAdmin.Models.Common
{
    public class PagedInputModel
    {
        public int Page { get; set; }

        public int Size { get; set; }

        public int TakeCount { get => Size; }

        public int SkipCount { get => Size * (Page - 1); }

        public Sort[] Sorts { get; set; }

        public IQueryable<T> GetSortExpression<T>(IQueryable<T> query)
        {
            // 创建表达式变量参数
            var parameter = Expression.Parameter(typeof(T), "o");

            if (Sorts != null)
            {
                for (int i = 0; i < Sorts.Length; i++)
                {
                    // 根据属性名获取属性
                    var sort = Sorts[i];

                    if (typeof(T).GetProperties().All(p => p.Name != sort.PropertyName))
                    {
                        throw new ErrorSortPropertyException();
                    }

                    var property = typeof(T).GetProperty(sort.PropertyName);

                    // 创建一个访问属性的表达式
                    var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                    var orderByExp = Expression.Lambda(propertyAccess, parameter);

                    string OrderName = "";
                    if (i > 0)
                        OrderName = sort.IsAsc ? "ThenBy" : "ThenByDescending";
                    else
                        OrderName = sort.IsAsc ? "OrderBy" : "OrderByDescending";


                    var resultExp = Expression.Call(typeof(Queryable), OrderName, new Type[] { typeof(T), property.PropertyType }, query.Expression, Expression.Quote(orderByExp));
                    query = query.Provider.CreateQuery<T>(resultExp);
                }
            }
            return query;
        }
    }

    public class PagedInputModelValidator : AbstractValidator<PagedInputModel>
    {
        public PagedInputModelValidator()
        {
            RuleFor(x => x.Page).GreaterThan(0).ConfirmMessage(MessageConstant.SYSTEM_ERROR_002);
            RuleFor(x => x.Size).GreaterThan(0).ConfirmMessage(MessageConstant.SYSTEM_ERROR_003);
        }
    }

    public class Sort
    {
        public string PropertyName { get; set; }

        public bool IsAsc { get; set; }
    }
}