using Convience.Util.Extension;
using SnippetAdmin.Data.Entity.Enums;
using SnippetAdmin.Models.Common;
using System.Linq.Expressions;
using System.Reflection;

namespace SnippetAdmin.Models.Dynamic
{
    public class DynamicSearchInputModel : PagedInputModel
    {
        public List<DynamicFilter> Filters { get; set; }

        public IQueryable<T> GetFilterExpression<T>(IQueryable<T> query)
        {
            Expression<Func<T, bool>> filterExpression = t => true;
            foreach (var filter in Filters)
            {
                if (filter.CompareType == null)
                {
                    continue;
                }

                //if (filter.ValueType == ValueTypeEnum.Number)
                //{
                //    filterExpression = HandleNumberFilter(filterExpression, filter);
                //}

                filterExpression = filter.ValueType switch
                {
                    ValueTypeEnum.Number => HandleNumberFilter(filterExpression, filter),
                    ValueTypeEnum.String => HandleStringFilter(filterExpression, filter),
                    ValueTypeEnum.DateTime => HandleDateFilter(filterExpression, filter),
                    ValueTypeEnum.Bool => HandleBoolFilter(filterExpression, filter),
                    ValueTypeEnum.Enum => HandleEnumFilter(filterExpression, filter),
                    _ => throw new Exception("Unable to identify valueType")
                };
            }

            return query.Where(filterExpression);
        }

        private Expression<Func<T, bool>> HandleNumberFilter<T>(Expression<Func<T, bool>> expression, DynamicFilter filter)
        {
            var propertyInfo = typeof(T)
                .GetProperty(filter.PropertyName.Substring(0, 1).ToUpper() +
                            filter.PropertyName.Substring(1));

            // 如果是可空类型的话，需要获取它的根类型
            var propertyType = propertyInfo.PropertyType;
            if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                propertyType = propertyInfo.PropertyType.GetGenericArguments()[0];
            }

            var parseMethod = propertyType.GetMethod("Parse", new Type[] { typeof(string) });
            var value = parseMethod.Invoke(null, new object[] { filter.NumberValue?.ToString() });

            var param = Expression.Parameter(typeof(T), "s");
            var propertyExpression = Expression.Property(param, propertyInfo);
            var valueExpression = Expression.Constant(value, propertyInfo.PropertyType);

            // 1:大于,2:大于等于,3:小于,4:小于等于,5:等于,6:不等于
            var compareExpression = filter.CompareType switch
            {
                1 => Expression.GreaterThan(propertyExpression, valueExpression),
                2 => Expression.GreaterThanOrEqual(propertyExpression, valueExpression),
                3 => Expression.LessThan(propertyExpression, valueExpression),
                4 => Expression.LessThanOrEqual(propertyExpression, valueExpression),
                5 => Expression.Equal(propertyExpression, valueExpression),
                6 => Expression.NotEqual(propertyExpression, valueExpression),
                _ => throw new NotSupportedException()
            };
            var filterExpression = Expression.Lambda<Func<T, bool>>(compareExpression, param);

            return expression.AndAll(filterExpression);
        }

        private Expression<Func<T, bool>> HandleStringFilter<T>(Expression<Func<T, bool>> expression, DynamicFilter filter)
        {
            var propertyInfo = typeof(T)
                .GetProperty(filter.PropertyName.Substring(0, 1).ToUpper() +
                            filter.PropertyName.Substring(1));

            var containsMethod = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });

            var param = Expression.Parameter(typeof(T), "s");
            var propertyExpression = Expression.Property(param, propertyInfo);
            var valueExpression = Expression.Constant(filter.StringValue, propertyInfo.PropertyType);

            // 1:包含,2:不包含,3:等于,4:不等于
            Expression compareExpression = filter.CompareType switch
            {
                1 => Expression.Call(propertyExpression, containsMethod, valueExpression),
                2 => Expression.Not(Expression.Call(propertyExpression, containsMethod, valueExpression)),
                3 => Expression.Equal(propertyExpression, valueExpression),
                4 => Expression.NotEqual(propertyExpression, valueExpression),
                _ => throw new NotSupportedException()
            };
            var filterExpression = Expression.Lambda<Func<T, bool>>(compareExpression, param);

            return expression.AndAll(filterExpression);
        }

        private Expression<Func<T, bool>> HandleDateFilter<T>(Expression<Func<T, bool>> expression, DynamicFilter filter)
        {
            var propertyInfo = typeof(T)
                .GetProperty(filter.PropertyName.Substring(0, 1).ToUpper() +
                            filter.PropertyName.Substring(1));
            var yearProperty = typeof(DateTime).GetProperty("Year");
            var dateProperty = typeof(DateTime).GetProperty("Date");
            var valueProperty = typeof(DateTime?).GetProperty("Value");

            var propertyIsNullable = propertyInfo.PropertyType.IsGenericType &&
                propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);

            // 左侧属性表达式 类型为datetime，如果是datetime？转换为datetime
            // 可空类型表达式 s.属性.value.day/date   非可空类型表达式为s.属性.day/date
            var param = Expression.Parameter(typeof(T), "s");
            var datePropertyExpression = propertyIsNullable ?
                Expression.Property(Expression.Property(param, propertyInfo), valueProperty)
                : Expression.Property(param, propertyInfo);
            var yearPropertyExpression = Expression.Property(datePropertyExpression, yearProperty);
            var dayPropertyExpression = Expression.Property(datePropertyExpression, dateProperty);

            var propertyExpression = filter.DatePrecision switch
            {
                DatePrecisionEnum.Year => yearPropertyExpression,
                DatePrecisionEnum.YearMonthDay => dayPropertyExpression,
                DatePrecisionEnum.YearMonthDayHourMinuteSecond => datePropertyExpression,
                _ => datePropertyExpression
            };

            // 右侧值表达式 类型为datetime
            var compareValueExpression = Expression.Constant(filter.DateTimeValue, typeof(DateTime));
            Expression valueExpression = filter.DatePrecision switch
            {
                DatePrecisionEnum.Year => Expression.Property(compareValueExpression, yearProperty),
                DatePrecisionEnum.YearMonthDay => Expression.Property(compareValueExpression, dateProperty),
                DatePrecisionEnum.YearMonthDayHourMinuteSecond => compareValueExpression,
                _ => compareValueExpression
            };

            // 1:大于,2:大于等于,3:小于,4:小于等于,5:等于,6:不等于
            Expression compareExpression = filter.CompareType switch
            {
                1 => Expression.GreaterThan(propertyExpression, valueExpression),
                2 => Expression.GreaterThanOrEqual(propertyExpression, valueExpression),
                3 => Expression.LessThan(propertyExpression, valueExpression),
                4 => Expression.LessThanOrEqual(propertyExpression, valueExpression),
                5 => Expression.Equal(propertyExpression, valueExpression),
                6 => Expression.NotEqual(propertyExpression, valueExpression),
                _ => throw new NotSupportedException()
            };
            var filterExpression = Expression.Lambda<Func<T, bool>>(compareExpression, param);

            return expression.AndAll(filterExpression);
        }

        private Expression<Func<T, bool>> HandleBoolFilter<T>(Expression<Func<T, bool>> expression, DynamicFilter filter)
        {
            var propertyInfo = typeof(T)
                .GetProperty(filter.PropertyName.Substring(0, 1).ToUpper() +
                            filter.PropertyName.Substring(1));

            var param = Expression.Parameter(typeof(T), "s");
            var propertyExpression = Expression.Property(param, propertyInfo);

            // 1:是  2：否
            var compareExpression = filter.CompareType switch
            {
                1 => Expression.IsTrue(propertyExpression),
                2 => Expression.IsFalse(propertyExpression),
                _ => throw new NotSupportedException()
            };
            var filterExpression = Expression.Lambda<Func<T, bool>>(compareExpression, param);

            return expression.AndAll(filterExpression);
        }

        private Expression<Func<T, bool>> HandleEnumFilter<T>(Expression<Func<T, bool>> expression, DynamicFilter filter)
        {
            var propertyInfo = typeof(T)
                .GetProperty(filter.PropertyName.Substring(0, 1).ToUpper() +
                            filter.PropertyName.Substring(1));

            // 如果是可空类型的话，需要获取它的根类型
            var propertyType = propertyInfo.PropertyType;
            if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                propertyType = propertyInfo.PropertyType.GetGenericArguments()[0];
            }

            var parseMethod = typeof(Enum).GetMethod("Parse", new Type[] { typeof(Type), typeof(string) });

            var listType = typeof(List<>).MakeGenericType(propertyType);
            var listAddMethod = listType.GetMethod("Add", new Type[] { propertyType });
            var listContainsMethod = listType.GetMethod("Contains", new Type[] { propertyType });
            var enumList = Activator.CreateInstance(listType);

            foreach (var value in filter.IntArrayValue)
            {
                var enumvalue = parseMethod.Invoke(null, new object[] { propertyType, value.ToString() });
                listAddMethod.Invoke(enumList, new object[] { enumvalue });
            }

            var param = Expression.Parameter(typeof(T), "s");
            var propertyExpression = Expression.Property(param, propertyInfo);
            var valueExpression = Expression.Constant(enumList);

            // 1：包含 2：不包含
            Expression compareExpression = filter.CompareType switch
            {
                1 => Expression.Call(valueExpression, listContainsMethod, propertyExpression),
                2 => Expression.Not(Expression.Call(valueExpression, listContainsMethod, propertyExpression)),
                _ => throw new NotSupportedException()
            };
            var filterExpression = Expression.Lambda<Func<T, bool>>(compareExpression, param);

            return expression.AndAll(filterExpression);
        }

    }

    public class DynamicFilter
    {
        public string PropertyName { get; set; }

        public ValueTypeEnum ValueType { get; set; }

        public int? CompareType { get; set; }

        public string StringValue { get; set; }

        public double? NumberValue { get; set; }

        public DateTime? DateTimeValue { get; set; }

        public DatePrecisionEnum? DatePrecision { get; set; }

        public bool? BoolValue { get; set; }

        public List<int> IntArrayValue { get; set; }
    }

    public enum ValueTypeEnum
    {
        String = 1,
        Number = 2,
        DateTime = 3,
        Bool = 4,
        Enum = 5
    }

    public enum DatePrecisionEnum
    {
        Year = 1,
        YearMonthDay = 2,
        YearMonthDayHourMinuteSecond = 3
    }
}
