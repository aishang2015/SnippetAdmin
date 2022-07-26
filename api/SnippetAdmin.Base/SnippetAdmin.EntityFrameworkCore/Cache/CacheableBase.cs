using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace SnippetAdmin.EntityFrameworkCore.Cache
{
    public class CacheableBase<TDbContext> where TDbContext : DbContext
    {
        private CacheableBase() { }

        /// <summary>
        /// 延迟初始化器
        /// </summary>
        private static readonly Lazy<CacheableBase<TDbContext>> lazy =
            new(() => new CacheableBase<TDbContext>());

        /// <summary>
        /// 可缓存类型
        /// </summary>
        public List<Type> CacheableTypeList { get; private set; }
            = new();

        /// <summary>
        /// 可缓存类型List的Add方法
        /// </summary>
        public Dictionary<string, MethodInfo> AddMethodInfoDic { get; private set; }
            = new();

        /// <summary>
        /// 可缓存类型List的删除方法
        /// </summary>
        public Dictionary<string, MethodInfo> RemoveAllMethodInfoDic { get; private set; }
            = new();

        /// <summary>
        /// 取得泛型实例
        /// </summary>
        public static CacheableBase<TDbContext> Instance
        {
            get
            {
                if (lazy.IsValueCreated)
                {
                    return lazy.Value;
                }
                else
                {
                    var result = lazy.Value;
                    SetCacheableTypeList(result);
                    SetMethodInfoDic(result);
                    return result;
                }
            }
        }


        /// <summary>
        /// 取得并设置可以实例化的类型
        /// </summary>
        private static void SetCacheableTypeList(CacheableBase<TDbContext> instance)
        {
            instance.CacheableTypeList = typeof(TDbContext).GetProperties()
                .Where(p => p.PropertyType.IsGenericType)
                .Where(p => typeof(DbSet<>).IsAssignableFrom(p.PropertyType.GetGenericTypeDefinition()) ||
                    p.PropertyType.GetInterface(typeof(DbSet<>).FullName) != null)
                .Select(p => p.PropertyType.GetGenericArguments()[0])
                .Where(t =>
                {
                    var cacheAttribute = t.GetCustomAttributes(typeof(CachableAttribute), false).FirstOrDefault();
                    return cacheAttribute != null && (cacheAttribute as CachableAttribute).CacheAble;
                }).ToList();
        }

        /// <summary>
        /// 取得并设置对应list的方法
        /// </summary>
        private static void SetMethodInfoDic(CacheableBase<TDbContext> instance)
        {
            instance.CacheableTypeList.ForEach(t =>
            {
                var listType = typeof(List<>).MakeGenericType(t);
                instance.AddMethodInfoDic.Add(t.FullName, typeof(List<>)
                    .MakeGenericType(t).GetMethod("Add"));
                instance.RemoveAllMethodInfoDic.Add(t.FullName, typeof(List<>)
                    .MakeGenericType(t).GetMethod("RemoveAll"));
            });
        }
    }
}
