using SnippetAdmin.Core.Monitor.Metric;
using System.Diagnostics.Tracing;

namespace SnippetAdmin.Core.Monitor
{
	public class MetricEventListener : EventListener
	{
		public AllMetrics Metrics { get; private set; } = new AllMetrics();

		protected override void OnEventSourceCreated(EventSource eventSource)
		{
			Console.WriteLine(eventSource.Name);
			if (eventSource.Name.Equals("Microsoft.EntityFrameworkCore") ||
				eventSource.Name.Equals("System.Runtime") ||
				eventSource.Name.Equals("Microsoft.AspNetCore.Hosting"))
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
				var (counterName, counterValue) = GetRelevantMetric(x as IDictionary<string, object>);
				var result = counterName switch
				{
					"active-db-contexts" => Metrics.EntityFrameworkCoreMetric.ActiveDbContexts = counterValue,
					"total-queries" => Metrics.EntityFrameworkCoreMetric.TotalQueries = counterValue,
					"queries-per-second" => Metrics.EntityFrameworkCoreMetric.QueriesPerSecond = counterValue,
					"total-save-changes" => Metrics.EntityFrameworkCoreMetric.TotalSaveChanges = counterValue,
					"save-changes-per-second" => Metrics.EntityFrameworkCoreMetric.SaveChangesPerSecond = counterValue,
					"compiled-query-cache-hit-rate" => Metrics.EntityFrameworkCoreMetric.CompiledQueryCacheHitRate = counterValue,
					"total-execution-strategy-operation-failures" => Metrics.EntityFrameworkCoreMetric.TotalExecutionStrategyOperationFailures = counterValue,
					"execution-strategy-operation-failures-per-second" => Metrics.EntityFrameworkCoreMetric.ExecutionStrategyOperationFailuresPerSecond = counterValue,
					"total-optimistic-concurrency-failures" => Metrics.EntityFrameworkCoreMetric.TotalOptimisticConcurrencyFailures = counterValue,
					"optimistic-concurrency-failures-per-second" => Metrics.EntityFrameworkCoreMetric.OptimisticConcurrencyFailuresPerSecond = counterValue,

					"time-in-gc" => Metrics.RuntimeMetric.TimeInGc = counterValue,
					"alloc-rate" => Metrics.RuntimeMetric.AllocRate = counterValue,
					"cpu-usage" => Metrics.RuntimeMetric.CpuUsage = counterValue,
					"exception-count" => Metrics.RuntimeMetric.ExceptionCount = counterValue,
					"gc-heap-size" => Metrics.RuntimeMetric.GcHeapSize = counterValue,
					"gen-0-gc-count" => Metrics.RuntimeMetric.Gen0GcCount = counterValue,
					"gen-0-size" => Metrics.RuntimeMetric.Gen0Size = counterValue,
					"gen-1-gc-count" => Metrics.RuntimeMetric.Gen1GcCount = counterValue,
					"gen-1-size" => Metrics.RuntimeMetric.Gen1Size = counterValue,
					"gen-2-gc-count" => Metrics.RuntimeMetric.Gen2GcCount = counterValue,
					"gen-2-size" => Metrics.RuntimeMetric.Gen2Size = counterValue,
					"loh-size" => Metrics.RuntimeMetric.LohSize = counterValue,
					"poh-size" => Metrics.RuntimeMetric.PohSize = counterValue,
					"gc-fragmentation" => Metrics.RuntimeMetric.GcFragmentation = counterValue,
					"monitor-lock-contention-count" => Metrics.RuntimeMetric.MonitorLockContentionCount = counterValue,
					"active-timer-count" => Metrics.RuntimeMetric.ActiveTimerCount = counterValue,
					"assembly-count" => Metrics.RuntimeMetric.AssemblyCount = counterValue,
					"threadpool-completed-items-count" => Metrics.RuntimeMetric.ThreadpoolCompletedItemsCount = counterValue,
					"threadpool-queue-length" => Metrics.RuntimeMetric.ThreadpoolQueueLength = counterValue,
					"threadpool-thread-count" => Metrics.RuntimeMetric.ThreadpoolThreadCount = counterValue,
					"working-set" => Metrics.RuntimeMetric.WorkingSet = counterValue,
					"il-bytes-jitted" => Metrics.RuntimeMetric.IlBytesJitted = counterValue,
					"method-jitted-count" => Metrics.RuntimeMetric.MethodJittedCount = counterValue,
					"gc-committed-bytes" => Metrics.RuntimeMetric.GcCommittedBytes = counterValue,

					"current-requests" => Metrics.HostingMetric.CurrentRequests = counterValue,
					"failed-requests" => Metrics.HostingMetric.FailedRequests = counterValue,
					"requests-per-second" => Metrics.HostingMetric.RequestRate = counterValue,
					"total-requests" => Metrics.HostingMetric.TotalRequests = counterValue,

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
