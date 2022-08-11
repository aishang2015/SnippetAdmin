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

        public abstract Task DoExecute();

        public async Task Execute(IJobExecutionContext context)
        {
            var record = new JobRecord();
            record.JobName = context.JobDetail.Key.Name;
            record.BeginTime = DateTime.Now;
            record.JobState = Data.Enums.JobState.运行中;
            _dbContext.Add(record);

            var job = _dbContext.Jobs.FirstOrDefault(j => j.Name == context.JobDetail.Key.Name);
            if (job != null)
            {
                job.LastTime = record.BeginTime;
                _dbContext.Update(job);
            }
            await _dbContext.SaveChangesAsync();
            record.JobState = Data.Enums.JobState.成功;

            var sw = new Stopwatch();
            sw.Start();

            try
            {
                await DoExecute();
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
