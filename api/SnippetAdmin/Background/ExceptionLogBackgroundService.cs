using SnippetAdmin.Core.Helpers;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Entity.System;

namespace SnippetAdmin.Background
{
    public class ExceptionLogBackgroundService : BackgroundService
    {

        private readonly IServiceProvider _provider;

        public ExceptionLogBackgroundService(IServiceProvider provider)
        {
            _provider = provider;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var log = await ChannelHelper<SysExceptionLog>.Instance.Reader.ReadAsync();

                using var scope = _provider.CreateScope();
                using var db = scope.ServiceProvider.GetRequiredService<SnippetAdminDbContext>();
                await db.SysExceptionLogs.AddAsync(log);
                await db.SaveChangesAsync();
            }
        }
    }
}
