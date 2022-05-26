using Cronos;
using Microsoft.EntityFrameworkCore;
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
        private BlockingCollection<Job> _jobQueue = new BlockingCollection<Job>();

        /// <summary>
        /// job和对应的实体类的字典
        /// </summary>
        private readonly Dictionary<string, Type> _typeDic = new();

        private readonly ConcurrentDictionary<string, ManualResetEvent> _resetEventDic = new();

        private readonly ConcurrentDictionary<string, CancellationTokenSource> _jobCancelTokenDic = new();

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
                _jobCancelTokenDic.TryAdd(job.Name, new CancellationTokenSource());
                Task.Factory.StartNew(async () =>
                {
                    // 判断是否激活的任务
                    _resetEventDic[job.Name].WaitOne();

                    // 列表里有旧任务先执行旧任务
                    RunOldJob(job);

                    while (!_jobCancelTokenDic[job.Name].Token.IsCancellationRequested)
                    {
                        // 判断是否激活的任务
                        _resetEventDic[job.Name].WaitOne();

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
                    jobRecord.JobId == job.Id &&
                    (jobRecord.JobState == JobState.Prepared || jobRecord.JobState == JobState.IsRunning));

                if (record != null)
                {
                    // 执行任务,并计时
                    var jobInstance = scope.ServiceProvider.GetRequiredService(_typeDic[job.Name]) as IJob;
                    var startTime = DateTime.Now;
                    var sw = new Stopwatch();
                    try
                    {
                        sw.Start();
                        await jobInstance.DoAsync(_jobCancelTokenDic[job.Name].Token);
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
                    }
                }
            }).Start();
        }

        /// <summary>
        /// 在task中执行任务
        /// </summary>
        private void RunNewJob(Job job, TimeSpan? delay, TriggerMode triggerMode = TriggerMode.Auto)
        {
            // task被gc回收的问题
            // https://stackoverflow.com/questions/2782802/can-net-task-instances-go-out-of-scope-during-run
            // https://stackoverflow.com/questions/18091002/what-gotchas-exist-with-tasks-and-garbage-collection

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

                // 计时器
                var sw = new Stopwatch();
                var startTime = DateTime.Now;
                try
                {
                    await taskDbcontext.SaveChangesAsync();

                    // 延迟执行
                    await Task.Delay(delay.Value);

                    // 开始执行,记录开始时间
                    startTime = DateTime.Now;
                    record.Entity.JobState = JobState.IsRunning;
                    record.Entity.BeginTime = startTime;
                    taskDbcontext.JobRecords.Update(record.Entity);
                    await taskDbcontext.SaveChangesAsync();

                    sw.Start();
                    await jobInstance.DoAsync(_jobCancelTokenDic[job.Name].Token);
                    sw.Stop();

                    // 结束时保存执行时长
                    record.Entity.Duration = sw.ElapsedMilliseconds;
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
                    taskDbcontext.JobRecords.Update(record.Entity);
                    await taskDbcontext.SaveChangesAsync();
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
            var jobs = dbContext.CacheSet<Job>().ToList();

            jobs.ForEach(j =>
            {
                var type = ReflectionUtil.GetAssemblyTypes().First(t => t.FullName == j.Name);
                _typeDic.TryAdd(j.Name, type);

                var resetEvent = new ManualResetEvent(j.IsActive);
                _resetEventDic.TryAdd(j.Name, resetEvent);
            });

            return jobs;
        }

        /// <summary>
        /// 立即执行一个任务
        /// </summary>
        /// <param name="job"></param>
        public void ActiveJobOnce(Job job)
        {
            _jobQueue.Add(job);
        }

        /// <summary>
        /// 激活任务
        /// </summary>
        public void ActivateJob(string jobName)
        {
            // 设置为有信号
            _resetEventDic[jobName].Set();
        }

        /// <summary>
        /// 暂停任务
        /// </summary>
        public void PauseJob(string jobName)
        {
            // 设置为无信号
            _resetEventDic[jobName].Reset();
        }

        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="jobName"></param>
        public void CancelJob(string jobName)
        {
            if (!_jobCancelTokenDic[jobName].IsCancellationRequested)
            {
                _jobCancelTokenDic[jobName].Cancel();
            }
        }
    }
}
