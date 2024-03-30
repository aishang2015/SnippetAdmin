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
	public class RbacOrganization
	{
		[Comment("主键")]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }

		[Comment("名称")]
		public string Name { get; set; }

		[Comment("编码")]
		public string Code { get; set; }

		[Comment("组织类型编码")]
		public string Type { get; set; }

		[Comment("图标")]
		public string Icon { get; set; }

		[Comment("图标Id")]
		public string IconId { get; set; }

		[Comment("电话")]
		public string Phone { get; set; }

		[Comment("地址")]
		public string Address { get; set; }

		[Comment("排序")]
		public int Sorting { get; set; }
	}
}