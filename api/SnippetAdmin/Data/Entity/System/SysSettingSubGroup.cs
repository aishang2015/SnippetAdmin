using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.System
{
	[Comment("系统配置子分组表")]
	[Table("T_Sys_SettingSubGroup")]
	public class SysSettingSubGroup
	{
		public int Id { get; set; }

		[Comment("分组代码")]
		public string GroupCode { get; set; }

		[Comment("图标")]
		public string Icon { get; set; }

		[Comment("名称")]
		public string Name { get; set; }

		[Comment("描述")]
		public string Describe { get; set; }

		[Comment("代码")]
		public string Code { get; set; }

		[Comment("顺序")]
		public int Index { get; set; }
	}
}
