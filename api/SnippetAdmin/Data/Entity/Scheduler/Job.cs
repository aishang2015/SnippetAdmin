﻿using Microsoft.EntityFrameworkCore;
using SnippetAdmin.EntityFrameworkCore.Cache;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.Scheduler
{
    [Comment("任务定义")]
    [Table("T_Scheduler_Job")]
    public class Job
    {
        [Comment("主键")]
        public int Id { get; set; }

        [Comment("执行的任务类")]
        public string? Type { get; set; }

        [Comment("任务key")]
        public string? Key { get; set; }

        [Comment("任务描述")]
        public string? Describe { get; set; }

        [Comment("Cron表达式")]
        public string? Cron { get; set; }

        [Comment("是否活动的任务")]
        public bool IsActive { get; set; }

        [Comment("上次执行时间")]
        public DateTime? LastTime { get; set; }

        [Comment("任务创建时间")]
        public DateTime? CreateTime { get; set; }
    }
}
