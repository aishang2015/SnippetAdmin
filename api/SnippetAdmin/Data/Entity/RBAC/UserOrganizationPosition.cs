using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.RBAC
{
    [Comment("用户组织职位表")]
    [Table("T_RBAC_USER_ORGANIZATION_POSITION")]
    public class UserOrganizationPosition
    {
        [Comment("主键")]
        [Column("id")]
        public int Id { get; set; }

        [Comment("用户id")]
        [Column("user_id")]
        public int UserId { get; set; }

        [Comment("组织id")]
        [Column("organization_id")]
        public int OrganizationId { get; set; }

        [Comment("职位id")]
        [Column("position_id")]
        public int? PositionId { get; set; }
    }
}