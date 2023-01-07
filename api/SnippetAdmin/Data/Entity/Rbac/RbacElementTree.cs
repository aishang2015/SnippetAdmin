//------------------------------------------------------------------------------
// 生成时间 2021-09-01 10:11:39
//------------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.EntityFrameworkCore.Cache;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.Rbac
{
    [Comment("元素树")]
    [Table("T_Rbac_ElementTree")]
    [Cachable]
    public class RbacElementTree
    {
        [Comment("主键")]
        public int Id { get; set; }

        [Comment("祖先")]
        public int Ancestor { get; set; }

        [Comment("后代")]
        public int Descendant { get; set; }

        [Comment("深度")]
        public int Length { get; set; }
    }
}