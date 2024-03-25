using Quartz;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Entity.Scheduler;
using System.Diagnostics;

namespace SnippetAdmin.Jobs
{
    public abstract class SnippetAdminJob : IJob
    {
        private readonly SnippetAdminDbContext _dbContext;

        public SnippetAdminJob(SnippetAdminDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public abstract Task<CommonResult> DoExecute();

        public async Task Execute(IJobExecutionContext context)
        {
            var record = new JobRecord();
            record.JobName = context.JobDetail.Key.Name;
            record.BeginTime = DateTime.Now;
            record.JobState = Data.Enums.JobState.运行中;
            _dbContext.Add(record);

            var job = _dbContext.Jobs.FirstOrDefault(j => j.Key == context.JobDetail.Key.Name);
            if (job != null)
            {
                job.LastTime = record.BeginTime;
                _dbContext.Update(job);
            }
            await _dbContext.SaveChangesAsync();

            var sw = new Stopwatch();
            sw.Start();

            record.JobState = Data.Enums.JobState.成功;
            await _dbContext.SaveChangesAsync();

            try
            {
                var result = await DoExecute();
                if (!result.IsSuccess)
                {
                    record.JobState = Data.Enums.JobState.失败;
                }
                else
                {
                    record.JobState = Data.Enums.JobState.成功;
                }
                record.Infomation = result.Message;
            }
            catch (Exception e)
            {
                record.JobState = Data.Enums.JobState.失败;
                record.Infomation = e.Message + "-" + e.StackTrace;
            }
            finally
            {
                sw.Stop();
                record.Duration = sw.ElapsedMilliseconds;
                record.EndTime = DateTime.Now;
                _dbContext.Update(record);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
