using Microsoft.AspNetCore.Mvc.ApiExplorer;
using SnippetAdmin.Core.Helpers;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Entity.System;
using System.Collections.Concurrent;

namespace SnippetAdmin.Background
{
	/// <summary>
	/// using to record the access log in db
	/// </summary>
	public class AccessedLogBackgroundService : BackgroundService
	{
		private readonly IServiceProvider _provider;

		private readonly ConcurrentDictionary<string, string> apiNameDic = new ConcurrentDictionary<string, string>();

		public AccessedLogBackgroundService(IServiceProvider provider)
		{
			_provider = provider;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await Task.Yield();
			var apiDesProvider = _provider.GetRequiredService<IApiDescriptionGroupCollectionProvider>();
			foreach (var item in apiDesProvider.ApiDescriptionGroups.Items.SelectMany(i => i.Items))
			{
				apiNameDic.AddOrUpdate(item.RelativePath, "", (key, value) => value);
			}


			while (!stoppingToken.IsCancellationRequested)
			{
				var log = await ChannelHelper<SysAccessLog>.Instance.Reader.ReadAsync();

				SnippetAdminDbContext executorDb;
				using var scope1 = _provider.CreateScope();
				using var db1 = scope1.ServiceProvider.GetRequiredService<SnippetAdminDbContext>();
				var isExist = db1.CheckSharingTableWithNoCreate<SysAccessLog>(DateTime.Now.ToString("yyyyMM"));
				if (!isExist)
				{
					await db1.CheckSharingTableWithCreate<SysAccessLog>(DateTime.Now.ToString("yyyyMM"));

					var scope2 = _provider.CreateScope();
					var db2 = scope2.ServiceProvider.GetRequiredService<SnippetAdminDbContext>();
					executorDb = db2;
				}
				else
				{
					executorDb = db1;
				}

				await executorDb.GetShardingTableSet<SysAccessLog>(DateTime.Now.ToString("yyyyMM")).AddAsync(log);
				await executorDb.SaveChangesAsync();
			}
		}
	}
}
