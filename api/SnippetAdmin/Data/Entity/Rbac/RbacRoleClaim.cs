
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.EntityFrameworkCore.Cache;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.Rbac
{
    [Table("T_Rbac_RoleClaim")]
    [Comment("角色Claim捆绑表")]
    public class RbacRoleClaim : IdentityRoleClaim<int>
    {
        [Comment("主键")]
        public override int Id { get; set; }

        [Comment("角色ID")]
        public override int RoleId { get; set; }

        [Comment("角色Claim类型")]
        public override string? ClaimType { get; set; }

        [Comment("角色Claim值")]
        public override string? ClaimValue { get; set; }
    }
}
