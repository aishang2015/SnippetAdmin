using Microsoft.AspNetCore.SignalR;

namespace SnippetAdmin.Business.Hubs
{
    public class MetricsHub : Hub<IMetricsHubClient>
    {
    }
}
