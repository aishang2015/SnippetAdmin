using SnippetAdmin.Core.Scheduler;

namespace SnippetAdmin.Jobs
{
    [Scheduler("0/30 * * * * ? ", "测试任务", true, false)]
    public class TestJob : IJob
    {
        public Task DoAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"测试任务执行 at {DateTime.Now}");
            return Task.CompletedTask;
        }
    }
}
