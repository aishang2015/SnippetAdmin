//------------------------------------------------------------------------------
// 生成时间 2021-09-01 09:56:26
//------------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.EntityFrameworkCore.Cache;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.Rbac
{
    [Comment("组织")]
    [Table("T_Rbac_Organization")]
    [Cachable]
    public class RbacOrganization
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

        [Comment("组织类型编码")]
        [Column("type")]
        public string Type { get; set; }

        [Comment("图标")]
        [Column("icon")]
        public string Icon { get; set; }

        [Comment("图标Id")]
        [Column("iconId")]
        public string IconId { get; set; }

        [Comment("电话")]
        [Column("phone")]
        public string Phone { get; set; }

        [Comment("地址")]
        [Column("address")]
        public string Address { get; set; }

        [Comment("排序")]
        [Column("sorting")]
        public int Sorting { get; set; }
    }
}