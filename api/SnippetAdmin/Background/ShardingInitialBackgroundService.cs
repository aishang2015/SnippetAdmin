using Hawthorn.EntityFramework.Sharding;
using SnippetAdmin.Core.Helpers;
using SnippetAdmin.Data;

namespace SnippetAdmin.Background
{
	public class ShardingInitialBackgroundService : BackgroundService
	{
		private readonly IServiceProvider _provider;

		public ShardingInitialBackgroundService(IServiceProvider provider)
		{
			_provider = provider;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await Task.Yield();

			using var scope = _provider.CreateScope();
			using var db = scope.ServiceProvider.GetRequiredService<SnippetAdminDbContext>();
			var shardingInfo = _provider.GetRequiredService<IShardingInfoService>();
			var shardings = db.SysShardings.ToList();
			var types = ReflectionHelper.GetAssemblyTypes();
			shardingInfo.AddShardingInfo(shardings.Select(s =>
				(types.FirstOrDefault(t => t.Name == s.TableType), s.TableName)
			).ToArray());
		}
	}
}
