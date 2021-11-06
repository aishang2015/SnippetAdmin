using System.Threading;
using System.Threading.Tasks;

namespace SnippetAdmin.Core.HostedService
{
    public interface IJob
    {
        public Task DoAsync(CancellationToken stoppingToken);
    }
}
