using SnippetAdmin.Core.HostedService;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SnippetAdmin.Business.Jobs
{
    [Scheduler("0/30 * * * * ? ", "测试任务", true)]
    public class TestJob : IJob
    {
        public Task DoAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"测试任务执行 at {DateTime.Now}");
            return Task.CompletedTask;
        }
    }
}
