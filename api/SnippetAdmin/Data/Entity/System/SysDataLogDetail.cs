using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.System
{
    [Comment("数据变更日志详情")]
    [Table("T_Sys_DataLogDetail")]
    [Index(nameof(DataLogId))]
    public class SysDataLogDetail
    {
        [Comment("主键")]
        public int Id { get; set; }

        [Comment("审计日志Id")]
        public Guid DataLogId { get; set; }

        [Comment("实体名称")]
        public string EntityName { get; set; } = null!;

        [Comment("属性名")]
        public string PropertyName { get; set; } = null!;

        [Comment("旧值")]
        public string OldValue { get; set; }

        [Comment("新值")]
        public string NewValue { get; set; }
    }
}
