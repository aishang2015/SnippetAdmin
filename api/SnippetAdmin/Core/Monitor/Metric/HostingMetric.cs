namespace SnippetAdmin.Core.Monitor.Metric
{
	public class HostingMetric
	{
		public string CurrentRequests { get; set; }

		public string FailedRequests { get; set; }

		public string RequestRate { get; set; }

		public string TotalRequests { get; set; }
	}
}
