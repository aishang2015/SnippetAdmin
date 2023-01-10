using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace SnippetAdmin.EntityFrameworkCore.Cache
{
	public class CacheableExtension
	{
		/// <summary>
		/// 将所有变更写入内存缓存
		/// </summary>
		/// <param name="contextId">dbconext的id</param>
		public static void CacheTrackerDataToMemory<T>(IMemoryCache _memoryCache, Guid contextId) where T : DbContext
		{
			var entryList = _memoryCache.Get<List<CachedEntry>>(contextId);
			entryList?.ForEach(entry =>
			{
				var typeName = entry.Entity.GetType().FullName;
				var dataList = _memoryCache.Get(typeName);

				// 方法信息
				var addMethod = CacheableBase<T>.Instance.AddMethodInfoDic[typeName];
				var removeAllMethod = CacheableBase<T>.Instance.RemoveAllMethodInfoDic[typeName];
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
		}

		private static Predicate<object> GetPredicate(PropertyInfo[] idProperties, CachedEntry entry)
		{
			bool equalFun(int index, object obj) =>
				idProperties[index].GetValue(obj).ToString() ==
				idProperties[index].GetValue(entry.Entity).ToString();

			// 暴力枚举😁
			return idProperties.Length switch
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
