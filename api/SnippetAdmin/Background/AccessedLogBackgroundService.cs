using SnippetAdmin.Core.Helpers;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Entity.System;

namespace SnippetAdmin.Background
{
    /// <summary>
    /// using to record the access log in db
    /// </summary>
    public class AccessedLogBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _provider;

        public AccessedLogBackgroundService(IServiceProvider provider)
        {
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var log = await ChannelHelper<SysApiAccessLog>.Instance.Reader.ReadAsync();
                using var scope = _provider.CreateScope();
                using var db = scope.ServiceProvider.GetRequiredService<SnippetAdminDbContext>();
                await db.SysApiAccessLogs.AddAsync(log);
                await db.SaveChangesAsync();
            }
        }
    }
}
