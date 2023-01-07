namespace SnippetAdmin.Core.Monitor.Metric
{
	public class RuntimeMetric
	{
		public string TimeInGc { get; set; }

		public string AllocRate { get; set; }

		public string CpuUsage { get; set; }

		public string ExceptionCount { get; set; }

		public string GcHeapSize { get; set; }

		public string Gen0GcCount { get; set; }

		public string Gen0Size { get; set; }

		public string Gen1GcCount { get; set; }

		public string Gen1Size { get; set; }

		public string Gen2GcCount { get; set; }

		public string Gen2Size { get; set; }

		public string LohSize { get; set; }

		public string PohSize { get; set; }

		public string GcFragmentation { get; set; }

		public string MonitorLockContentionCount { get; set; }

		public string ActiveTimerCount { get; set; }

		public string AssemblyCount { get; set; }

		public string ThreadpoolCompletedItemsCount { get; set; }

		public string ThreadpoolQueueLength { get; set; }

		public string ThreadpoolThreadCount { get; set; }

		public string WorkingSet { get; set; }

		public string IlBytesJitted { get; set; }

		public string MethodJittedCount { get; set; }

		public string GcCommittedBytes { get; set; }
	}
}
