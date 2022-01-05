using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace SnippetAdmin.Business.Hubs
{
    public class BroadcastHub : Hub<IBroadcastClient>
    {
        private readonly ILogger _logger;

        public BroadcastHub(ILogger<BroadcastHub> logger)
        {
            _logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            var userName = Context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation($"{userName} connected to the hub.");
            return base.OnConnectedAsync();
        }
    }
}