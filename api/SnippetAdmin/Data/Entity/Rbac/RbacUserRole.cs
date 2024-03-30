using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.EntityFrameworkCore.Cache;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.Rbac
{
    [Table("T_Rbac_UserRole")]
    public class RbacUserRole : IdentityUserRole<int>
    {
        [Comment("用户ID")]
        public override int UserId { get; set; }

        [Comment("角色ID")]
        public override int RoleId { get; set; }
    }
}
