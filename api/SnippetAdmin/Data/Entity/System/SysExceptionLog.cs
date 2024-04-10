using Microsoft.EntityFrameworkCore;
using SnippetAdmin.DynamicApi.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.System
{
	[DynamicApi("异常信息", typeof(SnippetAdminDbContext))]
	[Comment("异常信息")]
	[Table("T_Sys_ExceptionLog")]
	[Index(nameof(Type))]
	[Index(nameof(Path))]
	[Index(nameof(Username))]
	public class SysExceptionLog
	{
		public int Id { get; set; }

		[Comment("异常类型")]
		public string? Type { get; set; }

		[Comment("异常消息")]
		public string? Message { get; set; }

		[Comment("异常源")]
		public string? Source { get; set; }

		[Comment("堆栈跟踪")]
		public string? StackTrace { get; set; }

		[Comment("发生时间")]
		public DateTime HappenedTime { get; set; }

		[Comment("访问路径")]
		public string? Path { get; set; }

		[Comment("访问用户名")]
		public string? Username { get; set; }
	}
}
