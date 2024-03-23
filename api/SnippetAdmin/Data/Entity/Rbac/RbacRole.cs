using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.EntityFrameworkCore.Cache;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.Rbac
{
	[Comment("系统角色")]
	[Table("T_Rbac_Role")]
	[Cachable]
	public class RbacRole : IdentityRole<int>
    {
        [Comment("主键")]
        public override int Id { get; set; }

		[Comment("编码")]
		public string Code { get; set; }

		[Comment("备注")]
		public string Remark { get; set; }

		[Comment("是否激活")]
		public bool IsActive { get; set; }
	}
}