using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace SnippetAdmin.EntityFrameworkCore.Cache
{
    public static class CacheableInitializer
    {
        public static void LoadAllCacheableData<TDbContext>(
            this CacheableBase<TDbContext> instance,
            TDbContext dbContext,
            IMemoryCache cache)
            where TDbContext : DbContext
        {
            var toListMethod = typeof(CacheableInitializer).GetMethod("GetDataList",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            instance.CacheableTypeList.ForEach(t =>
            {
                var method = toListMethod.MakeGenericMethod(typeof(TDbContext), t);
                var data = method.Invoke(instance, new object[] { dbContext });
                cache.Set(t.FullName, data);
            });
        }

#pragma warning disable IDE0051 // 删除未使用的私有成员

        private static List<T> GetDataList<TDbContext, T>(TDbContext dbContext)
            where TDbContext : DbContext
            where T : class
        {
            return dbContext.Set<T>().ToList();
        }

#pragma warning restore IDE0051 // 删除未使用的私有成员
    }
}
