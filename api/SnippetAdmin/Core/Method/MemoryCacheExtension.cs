using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnippetAdmin.Core.Method
{
    public static class MemoryCacheExtension
    {
        private const string KeyStr = "CollectionCache";

        /// <summary>
        /// 将查询结果集合缓存（异步）
        /// </summary>
        public async static Task<IEnumerable<TEntity>> CacheCollectionAsync<TEntity>(this MemoryCache _memoryCache,
            IQueryable<TEntity> query)
        {
            return await _memoryCache.GetOrCreateAsync<IEnumerable<TEntity>>(GetCacheKey<TEntity>(),
                async entity => await query.ToListAsync());
        }

        /// <summary>
        /// 将查询结果集合缓存（同步）
        /// </summary>
        public static IEnumerable<TEntity> CacheCollection<TEntity>(this MemoryCache _memoryCache,
            IQueryable<TEntity> query)
        {
            return _memoryCache.GetOrCreate<IEnumerable<TEntity>>(GetCacheKey<TEntity>(),
                entity => query.ToList());
        }

        /// <summary>
        /// 将list集合缓存（同步）
        /// </summary>
        public static List<TEntity> CacheCollection<TEntity>(this MemoryCache _memoryCache,
            List<TEntity> collection)
        {
            return _memoryCache.GetOrCreate(GetCacheKey<TEntity>(), asentity => collection);
        }

        /// <summary>
        /// 将数组集合缓存（同步）
        /// </summary>
        public static TEntity[] CacheCollection<TEntity>(this MemoryCache _memoryCache,
            TEntity[] collection)
        {
            return _memoryCache.GetOrCreate(GetCacheKey<TEntity>(), asentity => collection);
        }

        /// <summary>
        /// 取得缓存集合
        /// </summary>
        public static List<TEntity> GetCollection<TEntity>(this MemoryCache _memoryCache)
        {
            return _memoryCache.TryGetValue(GetCacheKey<TEntity>(), out List<TEntity> result) ?
                result : null;
        }

        /// <summary>
        /// 清除缓存集合
        /// </summary>
        public static void ClearCollection<TEntity>(this MemoryCache _memoryCache)
        {
            _memoryCache.Remove(GetCacheKey<TEntity>() + KeyStr);
        }

        /// <summary>
        /// 获取缓存key
        /// </summary>
        private static string GetCacheKey<TEntity>()
        {
            return typeof(TEntity).Name + KeyStr;
        }
    }
}