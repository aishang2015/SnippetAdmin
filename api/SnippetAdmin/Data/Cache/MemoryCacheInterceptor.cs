using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;

namespace SnippetAdmin.Data.Cache
{
    public class MemoryCacheInterceptor : ISaveChangesInterceptor
    {
        private readonly IMemoryCache _memoryCache;

        private Dictionary<Type, List<object>> _addEntityDic;

        private Dictionary<Type, List<object>> _deleteEntityDic;

        private Dictionary<Type, List<object>> _modifyEntityDic;

        public MemoryCacheInterceptor(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void SaveChangesFailed(DbContextErrorEventData eventData) { }

        public Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            CacheEntityDic();
            return result;
        }

        public ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            CacheEntityDic();
            return new ValueTask<int>(result);
        }

        public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            MakeEntityDic(eventData);
            return result;
        }

        public ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            MakeEntityDic(eventData);
            return new ValueTask<InterceptionResult<int>>(result);
        }

        /// <summary>
        /// save all entity that need to be cache
        /// </summary>
        /// <param name="eventData"></param>
        private void MakeEntityDic(DbContextEventData eventData)
        {
            var entries = eventData.Context.ChangeTracker.Entries();
            _addEntityDic = ExtractEntityDic(entries, EntityState.Added);
            _deleteEntityDic = ExtractEntityDic(entries, EntityState.Deleted);
            _modifyEntityDic = ExtractEntityDic(entries, EntityState.Modified);
        }

        /// <summary>
        /// cache all dic
        /// </summary>
        private void CacheEntityDic()
        {
            _addEntityDic.ToList().ForEach(kv =>
            {
                var data = _memoryCache.Get(kv.Key.FullName);
                if (data != null)
                {
                    var addMethod = data.GetType().GetMethod("Add");
                    foreach (var obj in kv.Value)
                    {
                        addMethod.Invoke(data, new object[] { Convert.ChangeType(obj, kv.Key) });
                    }
                }
            });
            _modifyEntityDic.ToList().ForEach(kv =>
            {
                var modifyDataIds = kv.Value.Select(d => d.GetType().GetProperty("Id").GetValue(d));
                var data = _memoryCache.Get(kv.Key.FullName);

                if (data != null)
                {
                    // remove all modified entity
                    var removeMethod = data.GetType().GetMethod("RemoveAll");
                    Predicate<object> action = d => modifyDataIds.Contains(d.GetType().GetProperty("Id").GetValue(d));
                    removeMethod.Invoke(data, new object[] { action });

                    // re add 
                    var addMethod = data.GetType().GetMethod("Add");
                    foreach (var obj in kv.Value)
                    {
                        addMethod.Invoke(data, new object[] { Convert.ChangeType(obj, kv.Key) });
                    }
                }
            });
            _deleteEntityDic.ToList().ForEach(kv =>
            {
                var deleteDataIds = kv.Value.Select(d => d.GetType().GetProperty("Id").GetValue(d));
                var data = _memoryCache.Get(kv.Key.FullName);
                if (data != null)
                {
                    var removeMethod = data.GetType().GetMethod("RemoveAll");
                    Predicate<object> action = d => deleteDataIds.Contains(d.GetType().GetProperty("Id").GetValue(d));
                    removeMethod.Invoke(data, new object[] { action });
                }
            });
        }

        /// <summary>
        /// get entity from tracker
        /// </summary>
        private static Dictionary<Type, List<object>> ExtractEntityDic(IEnumerable<EntityEntry> entries,
            EntityState entityState)
        {
            return entries
                .Where(entry => entry.State == entityState)
                .Select(entry => entry.Entity)
                .GroupBy(entity => entity.GetType())
                .ToDictionary(entityGroup => entityGroup.Key, entityGroup => entityGroup.ToList());
        }
    }
}
