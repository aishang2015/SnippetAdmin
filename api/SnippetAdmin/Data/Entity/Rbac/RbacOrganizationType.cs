using Microsoft.EntityFrameworkCore;
using SnippetAdmin.EntityFrameworkCore.Cache;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.Rbac
{
	[Comment("组织类型")]
	[Table("T_Rbac_OrganizationType")]
	public class RbacOrganizationType
	{
		[Comment("主键")]
		public int Id { get; set; }

		[Comment("名称")]
		public string Name { get; set; }

		[Comment("编码")]
		public string Code { get; set; }
	}
}
