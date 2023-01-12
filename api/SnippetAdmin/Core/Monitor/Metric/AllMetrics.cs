namespace SnippetAdmin.Core.Monitor.Metric
{
	public class AllMetrics
	{
		public EntityFrameworkCoreMetric EntityFrameworkCoreMetric { get; } = new EntityFrameworkCoreMetric();

		public RuntimeMetric RuntimeMetric { get; } = new RuntimeMetric();


		public HostingMetric HostingMetric { get; } = new HostingMetric();
	}
}
