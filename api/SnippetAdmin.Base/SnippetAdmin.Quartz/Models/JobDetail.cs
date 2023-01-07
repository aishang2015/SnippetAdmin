namespace SnippetAdmin.Quartz.Models
{
	public class JobDetail
	{
		/// <summary>
		/// 任务类型
		/// </summary>
		public Type JobType { get; set; }

		/// <summary>
		/// 任务名称
		/// </summary>
		public string JobName { get; set; }

		/// <summary>
		/// 任务描述
		/// </summary>
		public string JobDescribe { get; set; }

		/// <summary>
		/// cron表达式
		/// </summary>
		public string CronExpression { get; set; }
	}
}
