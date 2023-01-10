using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;

namespace SnippetAdmin.EntityFrameworkCore.Cache
{
	public class SaveChangeInterceptor<T> : ISaveChangesInterceptor where T : DbContext
	{
		private readonly IMemoryCache _memoryCache;

		private List<CachedEntry> _entries = new List<CachedEntry>();

		public SaveChangeInterceptor(IMemoryCache memoryCache)
		{
			_memoryCache = memoryCache;
		}

		// 准备
		public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
		{
			LoadTrackData(eventData);
			return result;
		}

		public ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
		{
			LoadTrackData(eventData);
			return new ValueTask<InterceptionResult<int>>(result);
		}

		// 成功
		public int SavedChanges(SaveChangesCompletedEventData eventData, int result)
		{
			CacheTrackerData(eventData);
			return result;
		}

		public async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData,
			int result, CancellationToken cancellationToken = default)
		{
			CacheTrackerData(eventData);
			return result;
		}

		// 失败时不做处理
		// todo 当事务失败时会使之前的某几个savechange回滚

		/// <summary>
		/// 取得将要缓存的数据
		/// </summary>
		private void LoadTrackData(DbContextEventData eventData)
		{
			var contextId = eventData.Context.ContextId.InstanceId;

			_entries = eventData.Context.ChangeTracker.Entries()
				.Where(e => CacheableBase<T>.Instance.CacheableTypeList.Contains(e.Entity.GetType()))
				.Where(e => e.State == EntityState.Added || e.State == EntityState.Deleted || e.State == EntityState.Modified)
				.Select(e => new CachedEntry
				{
					Entity = e.Entity,
					State = e.State,
					Metadata = e.Metadata
				}).ToList();
		}


		/// <summary>
		/// 将要处理的数据保存到缓存
		/// </summary>
		private void CacheTrackerData(DbContextEventData eventData)
		{
			var contextId = eventData.Context.ContextId.InstanceId;
			if (_entries.Count > 0)
			{
				var cachedEntryList = _memoryCache.Get<List<CachedEntry>>(contextId);
				if (cachedEntryList != null)
				{
					cachedEntryList.AddRange(_entries);
				}
				else
				{
					_memoryCache.Set(contextId, _entries);
				}
			}
		}

	}
}
