using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.System
{
    [Comment("数据变更日志")]
    [Table("T_Sys_DataLog")]
    [Index(nameof(TraceIdentifier))]
    [Index(nameof(TransactionId))]
    public class SysDataLog
    {
        [Comment("主键")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        [Comment("跟踪标识")]
        public string TraceIdentifier { get; set; }

        [Comment("事务Id")]
        public Guid TransactionId { get; set; }

        [Comment("用户Id")]
        public int UserId { get; set; }

        [Comment("实体名称")]
        public string EntityName { get; set; } = null!;

        [Comment("2删除 3修改 4添加")]
        public int Operation { get; set; }

        [Comment("操作时间")]
        public DateTime OperateTime { get; set; }
    }
}
