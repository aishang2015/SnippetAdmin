using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Spi;
using SnippetAdmin.Quartz.Models;

namespace SnippetAdmin.Quartz
{
	public interface IQuartzService
	{
		Task Add(JobDetail jobDetail);
		Task Remove(string jobName);
		Task Pause(string jobName);
		Task Resume(string jobName);
		Task Trigger(string jobName);
	}

	public class QuartzService : IQuartzService
	{
		private static ISchedulerFactory _schedulerFactory;
		private static IScheduler _scheduler;

		public QuartzService(ISchedulerFactory schedulerFactory,
			IJobFactory jobFactory)
		{
			_schedulerFactory = schedulerFactory;
			_scheduler = _schedulerFactory.GetScheduler().Result;
		}

		public async Task Add(JobDetail jobDetail)
		{
			var job = JobBuilder.Create(jobDetail.JobType)
				.WithDescription(jobDetail.JobDescribe)
				.WithIdentity(jobDetail.JobKey, jobDetail.JobKey)
				.DisallowConcurrentExecution(true)      // 不要并发地执行同一个Job实例。
                .Build();

			var trigger = TriggerBuilder.Create()
					.WithIdentity(jobDetail.JobKey, jobDetail.JobKey)
					.WithDescription(jobDetail.JobDescribe)
					.WithCronSchedule(jobDetail.CronExpression)
					.Build();

			await _scheduler.ScheduleJob(job, trigger);
		}

		public async Task Remove(string jobKey)
		{
			var jobTrigger = await GetJobTrigger(jobKey);
			await _scheduler.PauseTrigger(jobTrigger.Key);
			await _scheduler.UnscheduleJob(jobTrigger.Key);
			await _scheduler.DeleteJob(jobTrigger.JobKey);
		}

		public async Task Pause(string jobKey)
		{
			var jobTrigger = await GetJobTrigger(jobKey);
			await _scheduler.PauseTrigger(jobTrigger.Key);
		}

		public async Task Resume(string jobKey)
		{
			var jobTrigger = await GetJobTrigger(jobKey);
			await _scheduler.ResumeTrigger(jobTrigger.Key);
		}

		public async Task Trigger(string jobKey)
		{
			var jobTrigger = await GetJobTrigger(jobKey);
			await _scheduler.TriggerJob(jobTrigger.JobKey);
		}

		private static async Task<ITrigger> GetJobTrigger(string jobKey)
		{
			var jobKeys = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(jobKey));
			var triggers = await _scheduler.GetTriggersOfJob(jobKeys.FirstOrDefault());
			var jobTrigger = triggers.FirstOrDefault();
			return jobTrigger;
		}
	}
}
