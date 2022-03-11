namespace SnippetAdmin.Core.Monitor.Metric
{
    public class Metrics
    {
        public EntityFrameworkCoreMetric EntityFrameworkCoreMetric { get; } = new EntityFrameworkCoreMetric();

        public RuntimeMetric RuntimeMetric { get; } = new RuntimeMetric();
    }
}
