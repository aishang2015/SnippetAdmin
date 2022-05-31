using Convience.Util.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Caching.Memory;
using SnippetAdmin.Data.Entity.RBAC;
using System.Collections.Concurrent;
using System.Dynamic;
using System.Reflection;

namespace SnippetAdmin.Data.Cache
{
    public class MemoryCacheInterceptor : ISaveChangesInterceptor
    {
        private class CachedEntry
        {
            public object Entity { get; set; }
            public EntityState State { get; set; }
            public IEntityType Metadata { get; set; }
        }

        private readonly IMemoryCache _memoryCache;

        private List<CachedEntry> _entryList;

        private static ConcurrentDictionary<string, MethodInfo> _addMethodInfoDic = new();

        private static ConcurrentDictionary<string, MethodInfo> _removeAllMethodInfoDic = new();

        private static AutoResetEvent autoResetEvent = new AutoResetEvent(true);

        public MemoryCacheInterceptor(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void SaveChangesFailed(DbContextErrorEventData eventData) { }

        public Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            TempCacheTrackerData(eventData);
            return result;
        }

        public async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            TempCacheTrackerData(eventData);
            return result;
        }

        public int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            CacheTrackerDataToMemory();
            return result;
        }

        public ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            CacheTrackerDataToMemory();
            return new ValueTask<int>(result);
        }

        /// <summary>
        /// 将savechange之前的数据进行保存
        /// </summary>
        /// <param name="eventData"></param>
        private void TempCacheTrackerData(DbContextEventData eventData)
        {
            // OrderBy的目的是为了先删除再添加缓存，否则像是USERROlE表会出问题
            _entryList = eventData.Context.ChangeTracker.Entries().Select(e => new CachedEntry
            {
                Entity = e.Entity,
                State = e.State,
                Metadata = e.Metadata
            }).OrderBy(e => e.State).ToList();
        }

        /// <summary>
        /// cache all dic
        /// </summary>
        private void CacheTrackerDataToMemory()
        {
            autoResetEvent.WaitOne();
            _entryList.Where(entry => DbContextInitializer.CacheAbleDic[entry.Entity.GetType()]).ToList().ForEach(entry =>
            {
                var typeName = entry.Entity.GetType().FullName;
                var dataList = _memoryCache.Get(typeName);

                // 方法信息
                var addMethod = _addMethodInfoDic.GetOrAdd(typeName, dataList.GetType().GetMethod("Add"));
                var removeAllMethod = _removeAllMethodInfoDic.GetOrAdd(typeName, dataList.GetType().GetMethod("RemoveAll"));
                switch (entry.State)
                {
                    case EntityState.Added:

                        // 添加
                        addMethod.Invoke(dataList, new object[] { entry.Entity });
                        break;
                    case EntityState.Deleted:

                        // 删除
                        var idProperties = entry.Metadata.FindPrimaryKey().Properties
                            .Select(p => p.PropertyInfo).ToArray();
                        var predicate = GetPredicate(idProperties, entry);
                        removeAllMethod.Invoke(dataList, new object[] { predicate });

                        break;
                    case EntityState.Modified:

                        // 删除
                        idProperties = entry.Metadata.FindPrimaryKey().Properties
                            .Select(p => p.PropertyInfo).ToArray();
                        predicate = GetPredicate(idProperties, entry);
                        removeAllMethod.Invoke(dataList, new object[] { predicate });

                        // 添加
                        addMethod.Invoke(dataList, new object[] { entry.Entity });
                        break;
                }
            });
            autoResetEvent.Set();
        }

        private static Predicate<Object> GetPredicate(PropertyInfo[] idProperties, CachedEntry entry)
        {
            // 表达式树方式
            //var predicate = ExpressionExtension.TrueExpression<object>();
            //foreach (var idProperty in idProperties)
            //{
            //    var idValue = idProperty.GetValue(entry.Entity).ToString();
            //    ExpressionExtension.AndAll(predicate, o => idProperty.GetValue(o).ToString() == idValue);
            //}
            //var lambda = predicate.Compile();
            //Predicate<object> p = o => lambda(o);

            Func<int, object, string> tValueGetter = (index, obj) => idProperties[index].GetValue(obj).ToString();
            Func<int, string> pValueGetter = (index) => idProperties[index].GetValue(entry.Entity).ToString();

            // 暴力枚举😁
            return idProperties.Count() switch
            {
                1 => o => tValueGetter(0, o) == pValueGetter(0),

                2 => o => tValueGetter(0, o) == pValueGetter(0) &&
                        tValueGetter(1, o) == pValueGetter(1),

                3 => o => tValueGetter(0, o) == pValueGetter(0) &&
                        tValueGetter(1, o) == pValueGetter(1) &&
                        tValueGetter(2, o) == pValueGetter(2),

                4 => o => tValueGetter(0, o) == pValueGetter(0) &&
                        tValueGetter(1, o) == pValueGetter(1) &&
                        tValueGetter(2, o) == pValueGetter(2) &&
                        tValueGetter(3, o) == pValueGetter(3),

                5 => o => tValueGetter(0, o) == pValueGetter(0) &&
                        tValueGetter(1, o) == pValueGetter(1) &&
                        tValueGetter(2, o) == pValueGetter(2) &&
                        tValueGetter(3, o) == pValueGetter(3) &&
                        tValueGetter(4, o) == pValueGetter(4),

                6 => o => tValueGetter(0, o) == pValueGetter(0) &&
                        tValueGetter(1, o) == pValueGetter(1) &&
                        tValueGetter(2, o) == pValueGetter(2) &&
                        tValueGetter(3, o) == pValueGetter(3) &&
                        tValueGetter(4, o) == pValueGetter(4) &&
                        tValueGetter(5, o) == pValueGetter(5),

                _ => o => false
            };
        }
    }
}
