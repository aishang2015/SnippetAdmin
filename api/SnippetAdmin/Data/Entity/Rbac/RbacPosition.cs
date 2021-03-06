//------------------------------------------------------------------------------
// 生成时间 2021-09-01 10:03:53
//------------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.EntityFrameworkCore.Cache;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.Rbac
{
    [Comment("职位")]
    [Table("T_Rbac_Position")]
    [Cachable]
    public class RbacPosition
    {
        [Comment("主键")]
        [Column("id")]
        public int Id { get; set; }

        [Comment("名称")]
        [Column("name")]
        public string Name { get; set; }

        [Comment("编码")]
        [Column("code")]
        public string Code { get; set; }

        [Comment("排序")]
        [Column("sorting")]
        public int Sorting { get; set; }
    }
}