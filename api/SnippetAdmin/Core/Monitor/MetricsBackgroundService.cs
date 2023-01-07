using Microsoft.AspNetCore.SignalR;

namespace SnippetAdmin.Core.Monitor
{
	public class MetricsBackgroundService : BackgroundService
	{
		private readonly IHubContext<MetricsHub, IMetricsHubClient> _metricsHub;

		private readonly MetricEventListener _metricEventListener;

		public MetricsBackgroundService(IHubContext<MetricsHub, IMetricsHubClient> metricsHub,
			MetricEventListener metricEventListener)
		{
			_metricsHub = metricsHub;
			_metricEventListener = metricEventListener;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await Task.Delay(1000);
			while (!stoppingToken.IsCancellationRequested)
			{
				// 调用客户端HandleMessage处理方法
				await _metricsHub.Clients.All.ReceiveMetrics(_metricEventListener.Metrics);
				await Task.Delay(1000, stoppingToken);
			}
		}
	}
}
