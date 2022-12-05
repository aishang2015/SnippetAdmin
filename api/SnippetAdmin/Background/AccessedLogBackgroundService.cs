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

				using var scope1 = _provider.CreateScope();
				using var db1 = scope1.ServiceProvider.GetRequiredService<SnippetAdminDbContext>();
				await db1.CheckSharingTable<SysAccessLog>(DateTime.Now.ToString("yyyyMM"));

				using var scope2 = _provider.CreateScope();
				using var db2 = scope2.ServiceProvider.GetRequiredService<SnippetAdminDbContext>();

				await db2.GetShardingTableSet<SysAccessLog>(DateTime.Now.ToString("yyyyMM")).AddAsync(log);
				await db2.SaveChangesAsync();
			}
		}
	}
}
