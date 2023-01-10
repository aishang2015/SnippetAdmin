using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Data.Enums;
using SnippetAdmin.DynamicApi.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.Scheduler
{
	[Comment("任务记录")]
	[Table("T_Scheduler_JobRecord")]
	public class JobRecord
	{
		[DynamicDicKey]
		public int Id { get; set; }

		[Comment("任务名称")]
		[DynamicDicValue]
		public string JobName { get; set; }

		[Comment("任务状态")]
		public JobState JobState { get; set; }

		[Comment("执行时长")]
		public long? Duration { get; set; }

		[Comment("开始时间")]
		public DateTime? BeginTime { get; set; }

		[Comment("结束时间")]
		public DateTime? EndTime { get; set; }

		[Comment("执行信息")]
		public string Infomation { get; set; }
	}
}
