using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SnippetAdmin.Core.HostedService
{
    public abstract class LoopingBackgroundService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(1);
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        await DoWorkAsync();
                    }
                }
                catch (Exception e)
                {
                    await AnalysisException(e);
                }
            }
        }

        /// <summary>
        /// 需要在循环中做的工作
        /// </summary>
        protected abstract Task DoWorkAsync();

        /// <summary>
        /// 异常时需要的操作
        /// </summary>
        protected abstract Task AnalysisException(Exception e);
    }
}