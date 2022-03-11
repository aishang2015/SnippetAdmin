using SnippetAdmin.Core.Monitor.Metric;

namespace SnippetAdmin.Business.Hubs
{
    public interface IMetricsHubClient
    {
        /// <summary>
        /// 客户端接收消息方法
        /// </summary>
        Task ReceiveMetrics(Metrics metrics);
    }
}
