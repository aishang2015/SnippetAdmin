namespace SnippetAdmin.Core.Scheduler
{
	public interface IJob
	{
		public Task DoAsync(CancellationToken stoppingToken);
	}
}
