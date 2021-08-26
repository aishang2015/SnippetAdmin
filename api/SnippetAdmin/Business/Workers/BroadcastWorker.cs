using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using SnippetAdmin.Business.Hubs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SnippetAdmin.Business.Workers
{
    public class BroadcastWorker : BackgroundService
    {
        private readonly IHubContext<BroadcastHub, IBroadcastClient> _broadcastHub;

        public BroadcastWorker(IHubContext<BroadcastHub, IBroadcastClient> broadcastHub)
        {
            _broadcastHub = broadcastHub;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // 调用客户端HandleMessage处理方法
                await _broadcastHub.Clients.All.HandleMessage($"服务器广播：服务器时间为{DateTime.Now}");
                await Task.Delay(30000);
            }
        }
    }
}