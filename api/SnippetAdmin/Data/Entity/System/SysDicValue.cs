using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.System
{
    [Comment("字典项目")]
    [Table("T_Sys_DicValue")]
    [Index(nameof(TypeId))]
    public class SysDicValue
    {
        public int Id { get; set; }

        [Comment("字典类型id")]
        [Column("type_id")]
        public int TypeId { get; set; }

        [Comment("字典项目名称")]
        [Column("name", TypeName = "varchar(200)")]
        public string Name { get; set; }

        [Comment("字典项目编码")]
        [Column("code", TypeName = "varchar(200)")]
        public string Code { get; set; }

        [Comment("排序值")]
        [Column("sorting")]
        public int Sorting { get; set; }
    }
}
