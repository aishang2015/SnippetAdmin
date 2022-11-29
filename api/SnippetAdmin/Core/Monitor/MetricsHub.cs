using Microsoft.AspNetCore.SignalR;
using SnippetAdmin.Core.Monitor.Metric;

namespace SnippetAdmin.Core.Monitor
{
    public interface IMetricsHubClient
    {
        /// <summary>
        /// 客户端接收消息方法
        /// </summary>
        Task ReceiveMetrics(AllMetrics metrics);
    }

    public class MetricsHub : Hub<IMetricsHubClient>
    {
		public override Task OnConnectedAsync()
		{
			return base.OnConnectedAsync();
		}
	}
}
