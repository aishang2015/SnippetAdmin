//------------------------------------------------------------------------------
// 生成时间 2021-09-01 10:03:53
//------------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.RBAC
{
    [Comment("职位")]
    [Table("T_RBAC_Position")]
    public class Position
    {
        [Comment("主键")]
        [Column("id")]
        public int Id { get; set; }

        [Comment("所属组织Id")]
        [Column("orgnazation_id")]
        public string OrgnazationId { get; set; }

        [Comment("名称")]
        [Column("name")]
        public string Name { get; set; }

        [Comment("下级是否可见")]
        [Column("is_lower_visible")]
        public bool IsLowerVisible { get; set; }
    }
}