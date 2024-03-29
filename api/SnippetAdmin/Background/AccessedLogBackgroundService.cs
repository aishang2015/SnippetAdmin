﻿using Microsoft.AspNetCore.Mvc.ApiExplorer;
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
				var log = await ChannelHelper<SysAccessLog>.Instance.Reader.ReadAsync(stoppingToken);

				if (log != null)
				{
					using var scope1 = _provider.CreateScope();
					using var executorDb = scope1.ServiceProvider.GetRequiredService<SnippetAdminDbContext>();

					await executorDb.SysAccessLogs.AddAsync(log);
					await executorDb.SaveChangesAsync();
				}

			}
		}
	}
}
