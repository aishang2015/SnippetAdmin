namespace SnippetAdmin.Core.Monitor.Metric
{
    public class EntityFrameworkCoreMetric
    {
        public string ActiveDbContexts { get; set; }

        public string TotalQueries { get; set; }

        public string QueriesPerSecond { get; set; }

        public string TotalSaveChanges { get; set; }

        public string SaveChangesPerSecond { get; set; }

        public string CompiledQueryCacheHitRate { get; set; }

        public string TotalExecutionStrategyOperationFailures { get; set; }

        public string ExecutionStrategyOperationFailuresPerSecond { get; set; }

        public string TotalOptimisticConcurrencyFailures { get; set; }

        public string OptimisticConcurrencyFailuresPerSecond { get; set; }
    }
}
