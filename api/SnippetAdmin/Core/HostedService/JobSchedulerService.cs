using Cronos;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SnippetAdmin.Core.Utils;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Cache;
using SnippetAdmin.Data.Entity.Enums;
using SnippetAdmin.Data.Entity.Scheduler;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SnippetAdmin.Core.HostedService
{
    /// <summary>
    /// 任务创建类
    /// </summary>
    public class JobSchedulerService : BackgroundService
    {
        private ConcurrentDictionary<string, Type> _typeDic =
            new ConcurrentDictionary<string, Type>();

        private ConcurrentDictionary<string, int> _lockDic =
            new ConcurrentDictionary<string, int>();

        private ConcurrentDictionary<int, CancellationToken> _cancelTokenDic =
            new ConcurrentDictionary<int, CancellationToken>();

        private readonly IServiceProvider _serviceProvider;

        private readonly IMemoryCache _memoryCache;

        public JobSchedulerService(IServiceProvider serviceProvider,
            IMemoryCache memoryCache)
        {
            _serviceProvider = serviceProvider;
            _memoryCache = memoryCache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(1);
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(3 * 1000);
                foreach (var job in _memoryCache.GetJobConfig().Where(j => j.IsActive))
                {
                    // 设置类型
                    if (!_typeDic.ContainsKey(job.Name))
                    {
                        var type = ReflectionUtil.GetAssemblyTypes().First(t => t.FullName == job.Name);
                        _typeDic.TryAdd(job.Name, type);
                    }

                    // 设置锁
                    if (!_lockDic.ContainsKey(job.Name))
                    {
                        _lockDic.TryAdd(job.Name, 0);
                    }

                    // 取得锁，判断下次任务是否已经准备好
                    var theLock = _lockDic[job.Name];

                    if (theLock == 0)
                    {
                        // 取得下次执行的时间
                        var now = DateTime.UtcNow;
                        var nextTime = CronExpression.Parse(job.Cron, CronFormat.IncludeSeconds)
                            .GetNextOccurrence(now, TimeZoneInfo.Local);
                        var nextTimeSpan = nextTime?.Subtract(now).Seconds ?? 0;

                        // 距离下次执行时间小于30秒，才会创建任务进入等待状态
                        if (nextTimeSpan > 30 * 1000)
                        {
                            continue;
                        }

                        // 上锁
                        Interlocked.Increment(ref theLock);
                        _lockDic[job.Name] = theLock;

                        // 这里虽然失去引用但是task会一直运行到结束，并不会被GC
                        // https://stackoverflow.com/questions/2782802/can-net-task-instances-go-out-of-scope-during-run
                        // https://stackoverflow.com/questions/18091002/what-gotchas-exist-with-tasks-and-garbage-collection
                        // 创建异步任务
                        new Task(async () =>
                        {
                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var taskDbcontext = scope.ServiceProvider.GetRequiredService<SnippetAdminDbContext>();
                                var jobInstance = scope.ServiceProvider.GetRequiredService(_typeDic[job.Name]) as IJob;

                                // 生成任务执行记录，记录开始时间
                                var record = taskDbcontext.JobRecords.Add(new JobRecord
                                {
                                    JobId = job.Id,
                                    JobState = JobState.Prepared,
                                });
                                await taskDbcontext.SaveChangesAsync();

                                // 延迟执行
                                await Task.Delay(nextTimeSpan);

                                // 开始执行,记录开始时间
                                record.Entity.JobState = JobState.IsRunning;
                                record.Entity.BeginTime = DateTime.Now;
                                taskDbcontext.JobRecords.Update(record.Entity);
                                await taskDbcontext.SaveChangesAsync();

                                // 执行任务,并计时
                                var sw = new Stopwatch();
                                try
                                {
                                    // 保存取消token
                                    var source = new CancellationTokenSource();
                                    _cancelTokenDic.TryAdd(record.Entity.Id, source.Token);

                                    sw.Start();
                                    await jobInstance.DoAsync(source.Token);
                                    sw.Stop();

                                    // 结束时保存执行时长
                                    record.Entity.JobState = JobState.Successed;
                                }
                                catch (Exception e)
                                {
                                    // 结束时保存执行时长
                                    record.Entity.JobState = JobState.Failed;
                                    record.Entity.Infomation = e.Message;
                                }
                                finally
                                {
                                    record.Entity.Duration = sw.ElapsedMilliseconds;
                                    taskDbcontext.JobRecords.Update(record.Entity);
                                    await taskDbcontext.SaveChangesAsync();

                                    // 用完了移出
                                    _cancelTokenDic.TryRemove(record.Entity.Id, out var token);
                                }

                                // 释放锁
                                Interlocked.Decrement(ref theLock);
                                _lockDic[job.Name] = theLock;
                            }
                        }).Start();
                    }
                }


            }
        }
    }
}
