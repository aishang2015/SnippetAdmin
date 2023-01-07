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
			//_scheduler.JobFactory = jobFactory;
			//if (!_scheduler.IsStarted)
			//{
			//    _scheduler.Start().Wait();
			//}
		}

		public async Task Add(JobDetail jobDetail)
		{
			var job = JobBuilder.Create(jobDetail.JobType)
				.WithDescription(jobDetail.JobDescribe)
				.WithIdentity(jobDetail.JobName, jobDetail.JobName)
				.Build();

			var trigger = TriggerBuilder.Create()
					.WithIdentity(jobDetail.JobName, jobDetail.JobName)
					.WithDescription(jobDetail.JobDescribe)
					.WithCronSchedule(jobDetail.CronExpression)
					.Build();

			await _scheduler.ScheduleJob(job, trigger);
		}

		public async Task Remove(string jobName)
		{
			var jobTrigger = await GetJobTrigger(jobName);
			await _scheduler.PauseTrigger(jobTrigger.Key);
			await _scheduler.UnscheduleJob(jobTrigger.Key);
			await _scheduler.DeleteJob(jobTrigger.JobKey);
		}

		public async Task Pause(string jobName)
		{
			var jobTrigger = await GetJobTrigger(jobName);
			await _scheduler.PauseTrigger(jobTrigger.Key);
		}

		public async Task Resume(string jobName)
		{
			var jobTrigger = await GetJobTrigger(jobName);
			await _scheduler.ResumeTrigger(jobTrigger.Key);
		}

		public async Task Trigger(string jobName)
		{
			var jobTrigger = await GetJobTrigger(jobName);
			await _scheduler.TriggerJob(jobTrigger.JobKey);
		}

		private static async Task<ITrigger> GetJobTrigger(string jobName)
		{
			var jobKeys = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(jobName));
			var triggers = await _scheduler.GetTriggersOfJob(jobKeys.FirstOrDefault());
			var jobTrigger = triggers.FirstOrDefault();
			return jobTrigger;
		}
	}
}
