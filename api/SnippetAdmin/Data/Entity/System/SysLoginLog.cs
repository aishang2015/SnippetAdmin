using Microsoft.EntityFrameworkCore;
using SnippetAdmin.DynamicApi.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.System
{
	[DynamicApi("登录日志", typeof(SnippetAdminDbContext))]
	[Comment("登录日志")]
	[Table("T_Sys_LoginLog")]
	public class SysLoginLog
	{
		[Comment("主键")]
		public int Id { get; set; }

		[Comment("访问用户名")]
		public string Username { get; set; }

		[Comment("访问者ip")]
		public string RemoteIp { get; set; }

		[Comment("访问时间")]
		public DateTime AccessedTime { get; set; }

		[Comment("是否成功")]
		public bool IsSucceed { get; set; }
	}
}
