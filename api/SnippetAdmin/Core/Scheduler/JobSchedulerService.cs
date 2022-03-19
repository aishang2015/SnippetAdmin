using Cronos;
using SnippetAdmin.Core.Scheduler.Exceptions;
using SnippetAdmin.Core.Utils;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Entity.Enums;
using SnippetAdmin.Data.Entity.Scheduler;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace SnippetAdmin.Core.Scheduler
{
    /// <summary>
    /// 任务创建类
    /// </summary>
    public class JobSchedulerService : BackgroundService
    {
        private static BlockingCollection<Job> _jobQueue = new BlockingCollection<Job>();

        private readonly ConcurrentDictionary<string, Type> _typeDic = new();

        private readonly ConcurrentDictionary<string, DateTime> _lockDic = new();

        private readonly ConcurrentDictionary<int, CancellationToken> _cancelTokenDic = new();

        private readonly IServiceProvider _serviceProvider;

        public JobSchedulerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(1, stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                // 取得任务配置
                var jobs = await GetJobsAsync(stoppingToken);
                jobs.AsParallel().ForAll(job =>
                {
                    // 取得所有配置的任务的Type
                    if (!_typeDic.ContainsKey(job.Name))
                    {
                        var type = ReflectionUtil.GetAssemblyTypes().First(t => t.FullName == job.Name);
                        _typeDic.TryAdd(job.Name, type);
                    }

                    // 取得当前任务的下次执行的时间
                    var now = DateTime.UtcNow;
                    var nextTime = CronExpression.Parse(job.Cron, CronFormat.IncludeSeconds)
                        .GetNextOccurrence(now, TimeZoneInfo.Local);

                    // 如果取不到下次时间，则直接抛出异常
                    if (nextTime == null)
                    {
                        throw new WrongCronException($"Job '{job.Name}' can not get the next run time from the cron expression.");
                    }

                    var delay = nextTime?.Subtract(now);

                    // 距离下次执行时间小于30秒，才会创建任务进入等待状态
                    if (delay?.Seconds > 30 * 1000)
                    {
                        return;
                    }

                    // 没有执行过，则执行最近一次
                    if (!_lockDic.ContainsKey(job.Name))
                    {
                        _lockDic.TryAdd(job.Name, nextTime.Value);
                    }
                    else
                    {
                        // 判断此次任务是否已经执行
                        if (_lockDic[job.Name] == nextTime)
                        {
                            return;
                        }

                        // 标记此次任务开始执行
                        _lockDic[job.Name] = nextTime.Value;
                    }

                    // 这里虽然失去引用但是task会一直运行到结束，并不会被GC
                    // https://stackoverflow.com/questions/2782802/can-net-task-instances-go-out-of-scope-during-run
                    // https://stackoverflow.com/questions/18091002/what-gotchas-exist-with-tasks-and-garbage-collection
                    // 创建异步任务
                    RunJob(job, delay);
                });

                // 运行及时任务
                while (_jobQueue.TryTake(out Job job))
                {
                    // 取得所有配置的任务的Type
                    if (!_typeDic.ContainsKey(job.Name))
                    {
                        var type = ReflectionUtil.GetAssemblyTypes().First(t => t.FullName == job.Name);
                        _typeDic.TryAdd(job.Name, type);
                    }

                    RunJob(job, TimeSpan.Zero, TriggerMode.Manual);
                }
            }
        }

        /// <summary>
        /// 在task中执行任务
        /// </summary>
        private void RunJob(Job job, TimeSpan? delay, TriggerMode triggerMode = TriggerMode.Auto)
        {
            new Task(async () =>
            {
                using var scope = _serviceProvider.CreateScope();

                var taskDbcontext = scope.ServiceProvider.GetRequiredService<SnippetAdminDbContext>();
                var jobInstance = scope.ServiceProvider.GetRequiredService(_typeDic[job.Name]) as IJob;

                // 生成任务执行记录，记录开始时间
                var record = taskDbcontext.JobRecords.Add(new JobRecord
                {
                    JobId = job.Id,
                    JobState = JobState.Prepared,
                    TriggerMode = triggerMode
                });
                await taskDbcontext.SaveChangesAsync();

                // 延迟执行
                await Task.Delay(delay.Value);

                // 开始执行,记录开始时间
                var startTime = DateTime.Now;
                record.Entity.JobState = JobState.IsRunning;
                record.Entity.BeginTime = startTime;
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
                    record.Entity.Infomation = e.Message + e.StackTrace;
                }
                finally
                {
                    // 记录上次执行时间
                    var jobInfo = taskDbcontext.Jobs.Find(job.Id);
                    jobInfo.LastTime = startTime;
                    taskDbcontext.Jobs.Update(jobInfo);

                    record.Entity.Duration = sw.ElapsedMilliseconds;
                    taskDbcontext.JobRecords.Update(record.Entity);
                    await taskDbcontext.SaveChangesAsync();

                    // 用完了移出
                    _cancelTokenDic.TryRemove(record.Entity.Id, out var token);
                }
            }).Start();
        }

        /// <summary>
        /// 取得配置中能够运行的任务
        /// </summary>
        private async Task<List<Job>> GetJobsAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateAsyncScope();

            // 任务执行周期
            await Task.Delay(3 * 1000, stoppingToken);

            var dbContext = scope.ServiceProvider.GetRequiredService<SnippetAdminDbContext>();
            return dbContext.CacheSet<Job>().Where(j => j.IsActive).ToList();
        }

        /// <summary>
        /// 立即执行一个任务
        /// </summary>
        /// <param name="job"></param>
        public static void ActiveJobOnce(Job job)
        {
            _jobQueue.Add(job);
        }
    }
}
