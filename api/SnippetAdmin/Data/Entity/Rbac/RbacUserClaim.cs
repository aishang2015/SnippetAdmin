using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.EntityFrameworkCore.Cache;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.Rbac
{
    [Table("T_Rbac_UserClaim")]
    [Comment("用户声明表")]
    public class RbacUserClaim : IdentityUserClaim<int>
    {
        [Comment("主键")]
        public override int Id { get; set; }

        [Comment("用户id")]
        public override int UserId { get; set; }

        [Comment("声明类型")]
        public override string? ClaimType { get; set; }

        [Comment("声明值")]
        public override string? ClaimValue { get; set; }
    }
}
