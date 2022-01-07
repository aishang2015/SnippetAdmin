using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Data.Cache;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.RBAC
{
    [Comment("系统角色")]
    [Table("T_RBAC_Role")]
    [Cachable]
    public class SnippetAdminRole : IdentityRole<int>
    {
        [Comment("编码")]
        public string Code { get; set; }

        [Comment("备注")]
        public string Remark { get; set; }

        [Comment("是否激活")]
        public bool IsActive { get; set; }
    }
}