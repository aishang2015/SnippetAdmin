using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SnippetAdmin.Data.Entity.RBAC
{
    public class SnippetAdminRole : IdentityRole<int>
    {
        [Comment("名称")]
        public new string Name { get; set; }

        [Comment("编码")]
        public string Code { get; set; }

        [Comment("备注")]
        public string Remark { get; set; }

        [Comment("是否激活")]
        public bool IsActive { get; set; }
    }
}