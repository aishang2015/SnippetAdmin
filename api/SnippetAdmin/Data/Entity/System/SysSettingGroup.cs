using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.System
{
	[Comment("系统配置分组表")]
	[Table("T_Sys_SettingGroup")]
	public class SysSettingGroup
	{
		public int Id { get; set; }

		[Comment("图标")]
		public string Icon { get; set; }

		[Comment("名称")]
		public string Name { get; set; }

		[Comment("代码")]
		public string Code { get; set; }

		[Comment("顺序")]
		public int Index { get; set; }
	}
}
