using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Core.Dynamic.Attributes;
using SnippetAdmin.Data.Entity.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.Scheduler
{
    [Comment("任务记录")]
    [Table("T_Scheduler_JobRecord")]
    [DynamicApi]
    public class JobRecord
    {
        public int Id { get; set; }

        [Comment("任务Id")]
        public int JobId { get; set; }

        [Comment("触发方式")]
        public TriggerMode TriggerMode { get; set; }

        [Comment("任务状态")]
        public JobState JobState { get; set; }

        [Comment("执行时长")]
        public long? Duration { get; set; }

        [Comment("执行时间")]
        public DateTime? BeginTime { get; set; }

        [Comment("执行信息")]
        public string Infomation { get; set; }
    }
}
