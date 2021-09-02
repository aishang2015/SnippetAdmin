//------------------------------------------------------------------------------
// 生成时间 2021-09-01 09:56:26
//------------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.RBAC
{
    [Comment("组织")]
    [Table("T_RBAC_Orgnazation")]
    public class Orgnazation
    {
        [Comment("主键")]
        [Column("id")]
        public int Id { get; set; }

        [Comment("名称")]
        [Column("name")]
        public string Name { get; set; }

        [Comment("电话")]
        [Column("phone")]
        public string Phone { get; set; }

        [Comment("地址")]
        [Column("address")]
        public string Address { get; set; }
    }
}