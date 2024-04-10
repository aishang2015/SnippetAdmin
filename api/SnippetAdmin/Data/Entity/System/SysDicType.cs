using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.System
{
	[Comment("字典类型")]
	[Table("T_Sys_DicType")]
	public class SysDicType
	{
		public int Id { get; set; }

		[Comment("字典类型名称")]
		public string? Name { get; set; }

		[Comment("字典类型编码")]
		public string? Code { get; set; }
	}
}
