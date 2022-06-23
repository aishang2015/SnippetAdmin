using SnippetAdmin.Core.Monitor.Metric;
using System.Diagnostics.Tracing;

namespace SnippetAdmin.Core.Monitor
{
    public class MetricEventListener : EventListener
    {
        public AllMetrics Metrics { get; private set; } = new AllMetrics();

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            //Console.WriteLine(eventSource.Name);
            if (eventSource.Name.Equals("Microsoft.EntityFrameworkCore") ||
                eventSource.Name.Equals("System.Runtime"))
            {
                EnableEvents(eventSource, EventLevel.Verbose, EventKeywords.All, new Dictionary<string, string>()
                {
                    ["EventCounterIntervalSec"] = "1"
                });
            }
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            eventData.Payload.ToList().ForEach(x =>
            {
                var tuple = GetRelevantMetric(x as IDictionary<string, object>);
                var result = tuple.Item1 switch
                {
                    "active-db-contexts" => Metrics.EntityFrameworkCoreMetric.ActiveDbContexts = tuple.Item2,
                    "total-queries" => Metrics.EntityFrameworkCoreMetric.TotalQueries = tuple.Item2,
                    "queries-per-second" => Metrics.EntityFrameworkCoreMetric.QueriesPerSecond = tuple.Item2,
                    "total-save-changes" => Metrics.EntityFrameworkCoreMetric.TotalSaveChanges = tuple.Item2,
                    "save-changes-per-second" => Metrics.EntityFrameworkCoreMetric.SaveChangesPerSecond = tuple.Item2,
                    "compiled-query-cache-hit-rate" => Metrics.EntityFrameworkCoreMetric.CompiledQueryCacheHitRate = tuple.Item2,
                    "total-execution-strategy-operation-failures" => Metrics.EntityFrameworkCoreMetric.TotalExecutionStrategyOperationFailures = tuple.Item2,
                    "execution-strategy-operation-failures-per-second" => Metrics.EntityFrameworkCoreMetric.ExecutionStrategyOperationFailuresPerSecond = tuple.Item2,
                    "total-optimistic-concurrency-failures" => Metrics.EntityFrameworkCoreMetric.TotalOptimisticConcurrencyFailures = tuple.Item2,
                    "optimistic-concurrency-failures-per-second" => Metrics.EntityFrameworkCoreMetric.OptimisticConcurrencyFailuresPerSecond = tuple.Item2,

                    "time-in-gc" => Metrics.RuntimeMetric.TimeInGc = tuple.Item2,
                    "alloc-rate" => Metrics.RuntimeMetric.AllocRate = tuple.Item2,
                    "cpu-usage" => Metrics.RuntimeMetric.CpuUsage = tuple.Item2,
                    "exception-count" => Metrics.RuntimeMetric.ExceptionCount = tuple.Item2,
                    "gc-heap-size" => Metrics.RuntimeMetric.GcHeapSize = tuple.Item2,
                    "gen-0-gc-count" => Metrics.RuntimeMetric.Gen0GcCount = tuple.Item2,
                    "gen-0-size" => Metrics.RuntimeMetric.Gen0Size = tuple.Item2,
                    "gen-1-gc-count" => Metrics.RuntimeMetric.Gen1GcCount = tuple.Item2,
                    "gen-1-size" => Metrics.RuntimeMetric.Gen1Size = tuple.Item2,
                    "gen-2-gc-count" => Metrics.RuntimeMetric.Gen2GcCount = tuple.Item2,
                    "gen-2-size" => Metrics.RuntimeMetric.Gen2Size = tuple.Item2,
                    "loh-size" => Metrics.RuntimeMetric.LohSize = tuple.Item2,
                    "poh-size" => Metrics.RuntimeMetric.PohSize = tuple.Item2,
                    "gc-fragmentation" => Metrics.RuntimeMetric.GcFragmentation = tuple.Item2,
                    "monitor-lock-contention-count" => Metrics.RuntimeMetric.MonitorLockContentionCount = tuple.Item2,
                    "active-timer-count" => Metrics.RuntimeMetric.ActiveTimerCount = tuple.Item2,
                    "assembly-count" => Metrics.RuntimeMetric.AssemblyCount = tuple.Item2,
                    "threadpool-completed-items-count" => Metrics.RuntimeMetric.ThreadpoolCompletedItemsCount = tuple.Item2,
                    "threadpool-queue-length" => Metrics.RuntimeMetric.ThreadpoolQueueLength = tuple.Item2,
                    "threadpool-thread-count" => Metrics.RuntimeMetric.ThreadpoolThreadCount = tuple.Item2,
                    "working-set" => Metrics.RuntimeMetric.WorkingSet = tuple.Item2,
                    "il-bytes-jitted" => Metrics.RuntimeMetric.IlBytesJitted = tuple.Item2,
                    "method-jitted-count" => Metrics.RuntimeMetric.MethodJittedCount = tuple.Item2,
                    "gc-committed-bytes" => Metrics.RuntimeMetric.GcCommittedBytes = tuple.Item2,

                    _ => string.Empty
                };
            });
        }

        private static (string, string) GetRelevantMetric(IDictionary<string, object> eventPayload)
        {
            if (eventPayload == null)
                return (null, null);

            var counterName = "";
            var counterValue = "";
            if (eventPayload.TryGetValue("Name", out object displayValue))
            {
                counterName = displayValue.ToString();
            }
            if (eventPayload.TryGetValue("Mean", out object value) ||
                eventPayload.TryGetValue("Increment", out value))
            {
                counterValue = value.ToString();
            }

            return (counterName, counterValue);
        }
    }
}
