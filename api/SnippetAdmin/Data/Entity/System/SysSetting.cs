using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.System
{
	[Comment("系统配置表")]
	[Table("T_Sys_Setting")]
	public class SysSetting
	{
		public int Id { get; set; }

		[Comment("关键字")]
		public string Key { get; set; }

		[Comment("名称")]
		public string Value { get; set; }
	}
}
