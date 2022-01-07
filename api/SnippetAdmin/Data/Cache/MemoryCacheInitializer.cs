﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace SnippetAdmin.Data.Cache
{
    public class MemoryCacheInitializer
    {
        public static ConcurrentDictionary<Type, bool> CacheAbleDic = new ConcurrentDictionary<Type, bool>();

        public static readonly Action<IMemoryCache, SnippetAdminDbContext> InitialCache =
            (memoryCache, dbcontext) =>
            {
                var dbSetPropertyTypes = dbcontext.GetType().GetProperties()
                    .Where(property =>
                        property.PropertyType.IsGenericType && (
                        typeof(DbSet<>).IsAssignableFrom(property.PropertyType.GetGenericTypeDefinition()) ||
                        property.PropertyType.GetInterface(typeof(DbSet<>).FullName) != null))
                    .ToList();

                var toListMethod = typeof(MemoryCacheInitializer).GetMethod("GetDataList");

                dbSetPropertyTypes.ForEach(dbSetProperty =>
                {
                    // 判断实体的cacheable特性
                    var entityType = dbSetProperty.PropertyType.GetGenericArguments()[0];
                    var cacheAttribute = entityType.GetCustomAttributes(typeof(CachableAttribute), false).FirstOrDefault();
                    CacheAbleDic.TryAdd(entityType, cacheAttribute != null && (cacheAttribute as CachableAttribute).CacheAble);

                    if (CacheAbleDic[entityType])
                    {
                        var method = toListMethod.MakeGenericMethod(entityType);
                        var data = method.Invoke(new MemoryCacheInitializer(), new object[] { dbcontext });
                        memoryCache.Set(dbSetProperty.PropertyType.GetGenericArguments()[0].FullName, data);
                    }
                });
            };

        public List<T> GetDataList<T>(SnippetAdminDbContext dbContext) where T : class
        {
            return dbContext.Set<T>().ToList();
        }
    }
}
