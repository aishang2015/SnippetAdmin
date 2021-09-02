//------------------------------------------------------------------------------
// 生成时间 2021-09-01 11:06:40
//------------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Data.Entity.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.RBAC
{
    [Comment("元素树")]
    [Table("T_RBAC_Element")]
    public class Element
    {
        [Comment("主键")]
        [Column("id")]
        public int Id { get; set; }

        [Comment("元素名称")]
        [Column("name")]
        public string Name { get; set; }

        [Comment("元素类型")]
        [Column("type")]
        public ElementType Type { get; set; }

        [Comment("元素标识")]
        [Column("identity")]
        public string Identity { get; set; }

        [Comment("访问接口")]
        [Column("access_api")]
        public string AccessApi { get; set; }
    }
}