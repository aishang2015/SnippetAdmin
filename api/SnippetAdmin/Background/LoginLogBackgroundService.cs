using SnippetAdmin.Core.Helpers;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Entity.System;

namespace SnippetAdmin.Background
{
	public class LoginLogBackgroundService : BackgroundService
	{
		private readonly IServiceProvider _provider;

		public LoginLogBackgroundService(IServiceProvider provider)
		{
			_provider = provider;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				var log = await ChannelHelper<SysLoginLog>.Instance.Reader.ReadAsync(stoppingToken);

				if (log != null)
				{
					using var scope = _provider.CreateScope();
					using var db = scope.ServiceProvider.GetRequiredService<SnippetAdminDbContext>();
					await db.SysLoginLogs.AddAsync(log);
					await db.SaveChangesAsync();
				}

			}
		}
	}
}
