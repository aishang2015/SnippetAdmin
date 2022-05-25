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

        private readonly Dictionary<string, Type> _typeDic = new();

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

            // 配置任务字典
            var jobs = await GetJobsAsync(stoppingToken);

            jobs.ForEach(job =>
            {
                Task.Factory.StartNew(async () =>
                {
                    // 列表里有旧任务先执行旧任务
                    RunOldJob(job);

                    while (true)
                    {
                        // 取得当前任务的下次执行的时间
                        var now = DateTime.UtcNow;
                        var nextTime = CronExpression.Parse(job.Cron, CronFormat.IncludeSeconds)
                            .GetNextOccurrence(now, TimeZoneInfo.Local);

                        // 如果取不到下次时间，则直接抛出异常
                        if (nextTime == null)
                        {
                            return;
                        }

                        // 数据库中创建任务到开始执行之间的间隔,设置为30s
                        var runSpan = TimeSpan.FromSeconds(30);

                        // 延迟
                        var delay = nextTime.Value.Subtract(now).Subtract(runSpan);

                        // 直接开始任务
                        if (delay.Seconds > 0)
                        {
                            await Task.Delay(delay);
                        }

                        // 创建task并执行新任务                        
                        RunNewJob(job, runSpan);

                        // 上边的delay减少了runSpan，这里补回来
                        await Task.Delay(runSpan);
                    }

                }, TaskCreationOptions.LongRunning);
            });

            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    jobs.AsParallel().ForAll(job =>
            //    {
            //        // 取得当前任务的下次执行的时间
            //        var now = DateTime.UtcNow;
            //        var nextTime = CronExpression.Parse(job.Cron, CronFormat.IncludeSeconds)
            //            .GetNextOccurrence(now, TimeZoneInfo.Local);

            //        // 如果取不到下次时间，则直接抛出异常
            //        if (nextTime == null)
            //        {
            //            throw new WrongCronException($"Job '{job.Name}' can not get the next run time from the cron expression.");
            //        }

            //        var delay = nextTime?.Subtract(now);

            //        // 距离下次执行时间小于30秒，才会创建任务进入等待状态
            //        if (delay?.Seconds > 30 * 1000)
            //        {
            //            return;
            //        }

            //        // 没有执行过，则执行最近一次
            //        if (!_lockDic.ContainsKey(job.Name))
            //        {
            //            _lockDic.TryAdd(job.Name, nextTime.Value);
            //        }
            //        else
            //        {
            //            // 判断此次任务是否已经执行
            //            if (_lockDic[job.Name] == nextTime)
            //            {
            //                return;
            //            }

            //            // 标记此次任务开始执行
            //            _lockDic[job.Name] = nextTime.Value;
            //        }

            //        // 这里虽然失去引用但是task会一直运行到结束，并不会被GC
            //        // https://stackoverflow.com/questions/2782802/can-net-task-instances-go-out-of-scope-during-run
            //        // https://stackoverflow.com/questions/18091002/what-gotchas-exist-with-tasks-and-garbage-collection
            //        // 创建异步任务
            //        RunNewJob(job, delay);
            //    });

            //    // 运行及时任务
            //    while (_jobQueue.TryTake(out Job job))
            //    {
            //        RunNewJob(job, TimeSpan.Zero, TriggerMode.Manual);
            //    }
            //}
        }

        /// <summary>
        /// 执行旧任务
        /// </summary>
        /// <param name="job"></param>
        private void RunOldJob(Job job)
        {
            new Task(async () =>
            {
                using var scope = _serviceProvider.CreateScope();
                using var taskDbcontext = scope.ServiceProvider.GetRequiredService<SnippetAdminDbContext>();
                var record = taskDbcontext.JobRecords.FirstOrDefault(jobRecord =>
                    jobRecord.JobId == job.Id && jobRecord.JobState == JobState.Prepared);

                if(record != null)
                {
                    // 执行任务,并计时
                    var jobInstance = scope.ServiceProvider.GetRequiredService(_typeDic[job.Name]) as IJob;
                    var startTime = DateTime.Now;
                    var sw = new Stopwatch();
                    try
                    {
                        // 保存取消token
                        var source = new CancellationTokenSource();
                        _cancelTokenDic.TryAdd(record.Id, source.Token);

                        sw.Start();
                        await jobInstance.DoAsync(source.Token);
                        sw.Stop();

                        // 结束时保存执行时长
                        record.JobState = JobState.Successed;
                    }
                    catch (Exception e)
                    {
                        // 结束时保存执行时长
                        record.JobState = JobState.Failed;
                        record.Infomation = e.Message + e.StackTrace;
                    }
                    finally
                    {
                        // 记录上次执行时间
                        var jobInfo = taskDbcontext.Jobs.Find(job.Id);
                        jobInfo.LastTime = startTime;
                        taskDbcontext.Jobs.Update(jobInfo);

                        record.BeginTime = startTime;
                        record.Duration = sw.ElapsedMilliseconds;
                        taskDbcontext.JobRecords.Update(record);
                        await taskDbcontext.SaveChangesAsync();

                        // 用完了移出
                        _cancelTokenDic.TryRemove(record.Id, out var token);
                    }
                }
            }).Start();
        }

        /// <summary>
        /// 在task中执行任务
        /// </summary>
        private void RunNewJob(Job job, TimeSpan? delay, TriggerMode triggerMode = TriggerMode.Auto)
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
            var jobs = dbContext.CacheSet<Job>().Where(j => j.IsActive).ToList();

            jobs.ForEach(j =>
            {
                var type = ReflectionUtil.GetAssemblyTypes().First(t => t.FullName == j.Name);
                _typeDic.TryAdd(j.Name, type);
            });

            return jobs;
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
