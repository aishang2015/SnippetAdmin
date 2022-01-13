using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Data.Cache;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.RBAC
{
    [Comment("组织类型")]
    [Table("T_RBAC_OrganizationType")]
    [Cachable]
    public class OrganizationType
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
    }
}
