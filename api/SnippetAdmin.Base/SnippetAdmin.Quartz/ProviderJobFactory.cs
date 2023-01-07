using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;

namespace SnippetAdmin.Quartz
{
	public class ProviderJobFactory : IJobFactory
	{
		private readonly IServiceProvider _provider;

		public ProviderJobFactory(IServiceProvider provider)
		{
			_provider = provider;
		}

		public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
		{
			return _provider.GetRequiredService<QuartzJobRunner>();
		}

		public void ReturnJob(IJob job)
		{
		}
	}
}
