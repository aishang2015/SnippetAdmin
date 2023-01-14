using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.System
{
	[Comment("系统配置表")]
	[Table("T_Sys_Setting")]
	public class SysSetting
	{
		public int Id { get; set; }

		[Comment("分组代码")]
		public string GroupCode { get; set; }

		[Comment("子分组代码")]
		public string SubGroupCode { get; set; }

		[Comment("图标")]
		public string Icon { get; set; }

		[Comment("名称")]
		public string Name { get; set; }

		[Comment("描述")]
		public string Describe { get; set; }

		[Comment("代码")]
		public string Code { get; set; }

		[Comment("配置值")]
		public string Value { get; set; }

		[Comment("输入类型：1：字符串，2：数值，3：长文本，4：开关，5：单选，6：复选，7：下拉选，8：日期，9：日期时间")]
		public int InputType { get; set; }

		[Comment("选项，单选，复选等类型使用")]
		public string Options { get; set; }

		[Comment("顺序")]
		public int Index { get; set; }

		[Comment("是否在页面上显示")]
		public bool IsShow { get; set; }

		#region 校验

		public int? Min { get; set; }

		public int? Max { get; set; }

		public string Regex { get; set; }

		#endregion
	}
}
