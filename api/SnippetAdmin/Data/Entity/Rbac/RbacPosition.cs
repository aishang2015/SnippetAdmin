//------------------------------------------------------------------------------
// 生成时间 2021-09-01 10:03:53
//------------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.EntityFrameworkCore.Cache;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.Rbac
{
	[Comment("职位")]
	[Table("T_Rbac_Position")]
	public class RbacPosition
	{
		[Comment("主键")]
		public int Id { get; set; }

		[Comment("名称")]
		public string? Name { get; set; }

		[Comment("编码")]
		public string? Code { get; set; }

		[Comment("排序")]
		public int Sorting { get; set; }
	}
}