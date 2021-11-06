using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.Scheduler
{
    [Comment("任务定义")]
    [Table("T_Scheduler_Job")]
    public class Job
    {
        public int Id { get; set; }

        [Comment("任务类全名")]
        public string Name { get; set; }

        [Comment("任务描述")]
        public string Describe { get; set; }

        [Comment("Cron表达式")]
        public string Cron { get; set; }

        [Comment("是否活动的任务")]
        public bool IsActive { get; set; }

        [Comment("下次执行时间")]
        public DateTime? NextTime { get; set; }

        [Comment("上次执行时间")]
        public DateTime? LastTime { get; set; }

        [Comment("任务创建时间")]
        public DateTime? CreateTime { get; set; }
    }
}
