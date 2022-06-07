using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Reflection;

namespace SnippetAdmin.Data.Cache
{
    public class MemoryCacheInterceptor : DbTransactionInterceptor, ISaveChangesInterceptor
    {
        private class CachedEntry
        {
            public object Entity { get; set; }
            public EntityState State { get; set; }
            public IEntityType Metadata { get; set; }
        }

        private readonly IMemoryCache _memoryCache;

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

        public ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            TempCacheTrackerData(eventData);
            return new ValueTask<InterceptionResult<int>>(result);
        }

        public int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            return result;
        }

        public ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            return new ValueTask<int>(result);
        }

        public override void TransactionCommitted(DbTransaction transaction, TransactionEndEventData eventData)
        {
            CacheTrackerDataToMemory(eventData.Context.ContextId.InstanceId);
            base.TransactionCommitted(transaction, eventData);
        }

        public override Task TransactionCommittedAsync(DbTransaction transaction, TransactionEndEventData eventData, CancellationToken cancellationToken = default)
        {
            CacheTrackerDataToMemory(eventData.Context.ContextId.InstanceId);
            return base.TransactionCommittedAsync(transaction, eventData, cancellationToken);
        }

        /// <summary>
        /// 将savechange之前的数据进行保存
        /// </summary>
        /// <param name="eventData"></param>
        private void TempCacheTrackerData(DbContextEventData eventData)
        {
            var contextId = eventData.Context.ContextId.InstanceId;

            // OrderBy的目的是为了先删除再添加缓存，否则像是USERROlE表会出问题
            var entryList = eventData.Context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Deleted || e.State == EntityState.Modified)
                .Select(e => new CachedEntry
                {
                    Entity = e.Entity,
                    State = e.State,
                    Metadata = e.Metadata
                }).ToList();

            var cachedEntryList = _memoryCache.Get<List<CachedEntry>>(contextId);
            if (cachedEntryList != null)
            {
                cachedEntryList.AddRange(entryList);
            }
            else
            {
                _memoryCache.Set(contextId, entryList);
            }
        }

        /// <summary>
        /// 将所有变更写入内存缓存
        /// </summary>
        /// <param name="contextId">dbconext的id</param>
        private void CacheTrackerDataToMemory(Guid contextId)
        {
            // 暂时先这样写
            autoResetEvent.WaitOne();

            var entryList = _memoryCache.Get<List<CachedEntry>>(contextId);
            entryList?.Where(entry => DbContextInitializer.CacheAbleDic[entry.Entity.GetType()])
                .ToList().ForEach(entry =>
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

            // 事务结束后清理掉
            _memoryCache.Remove(contextId);

            autoResetEvent.Set();
        }

        private static Predicate<object> GetPredicate(PropertyInfo[] idProperties, CachedEntry entry)
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

            Func<int, object, bool> equalFun = (index, obj) =>
                idProperties[index].GetValue(obj).ToString() ==
                idProperties[index].GetValue(entry.Entity).ToString();

            // 暴力枚举😁
            return idProperties.Count() switch
            {
                1 => o => equalFun(0, o),
                2 => o => equalFun(0, o) && equalFun(1, o),
                3 => o => equalFun(0, o) && equalFun(1, o) && equalFun(2, o),
                4 => o => equalFun(0, o) && equalFun(1, o) && equalFun(2, o) && equalFun(3, o),
                5 => o => equalFun(0, o) && equalFun(1, o) && equalFun(2, o) && equalFun(3, o) && equalFun(4, o),
                6 => o => equalFun(0, o) && equalFun(1, o) && equalFun(2, o) && equalFun(3, o) && equalFun(4, o) && equalFun(5, o),
                _ => o => false
            };
        }
    }
}
