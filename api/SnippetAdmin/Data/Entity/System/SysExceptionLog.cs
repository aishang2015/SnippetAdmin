using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Core.Dynamic.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.System
{
    [DynamicApi("异常信息")]
    [Comment("异常信息")]
    [Table("T_Sys_ExceptionLog")]
    [Index(nameof(Type))]
    [Index(nameof(Path))]
    [Index(nameof(Username))]
    public class SysExceptionLog
    {
        public int Id { get; set; }

        [Comment("异常类型")]
        [Column("type", TypeName = "varchar(200)")]
        public string Type { get; set; }

        [Comment("异常消息")]
        [Column("message", TypeName = "varchar(200)")]
        public string Message { get; set; }

        [Comment("异常源")]
        [Column("source")]
        public string Source { get; set; }

        [Comment("堆栈跟踪")]
        [Column("stack_trace")]
        public string StackTrace { get; set; }

        [Comment("发生时间")]
        [Column("happened_time")]
        public DateTime HappenedTime { get; set; }

        [Comment("访问路径")]
        [Column("path", TypeName = "varchar(200)")]
        public string Path { get; set; }

        [Comment("访问用户名")]
        [Column("username", TypeName = "varchar(200)")]
        public string Username { get; set; }
    }
}
