using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
            var userName = Context.User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                .FirstOrDefault()?.Value;
            _logger.LogInformation($"{userName} connected to the hub.");
            return base.OnConnectedAsync();
        }
    }
}