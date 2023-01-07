using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.System
{
	/// <summary>
	/// 月份分表
	/// </summary>
	[Comment("接口访问日志")]
	[Table("T_Sys_AccessLog")]
	[Index(nameof(AccessedTime))]
	[Index(nameof(Path))]
	public class SysAccessLog
	{
		[Comment("主键")]
		public int Id { get; set; }

		[Comment("访问方法")]
		public string Method { get; set; }

		[Comment("方法描述")]
		public string Description { get; set; }

		[Comment("访问路径")]
		public string Path { get; set; }

		[Comment("访问用户名")]
		public string Username { get; set; }

		[Comment("访问者ip")]
		public string RemoteIp { get; set; }

		[Comment("访问时间")]
		public DateTime AccessedTime { get; set; }

		[Comment("运行时间")]
		public long ElapsedTime { get; set; }

		[Comment("请求内容")]
		public string RequestBody { get; set; }

		[Comment("返回状态码")]
		public int StatusCode { get; set; }

		[Comment("响应内容")]
		public string ResponseBody { get; set; }
	}
}
