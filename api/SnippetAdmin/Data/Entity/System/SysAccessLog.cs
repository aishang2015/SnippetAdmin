﻿using Microsoft.EntityFrameworkCore;
using SnippetAdmin.DynamicApi.Attributes;
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
		[Column("id")]
		public int Id { get; set; }

		[Comment("访问方法")]
		[Column("method")]
		public string Method { get; set; }

		[Comment("方法描述")]
		[Column("description")]
		public string Description { get; set; }

		[Comment("访问路径")]
		[Column("path")]
		public string Path { get; set; }

		[Comment("访问用户名")]
		[Column("username")]
		public string Username { get; set; }

		[Comment("访问者ip")]
		[Column("remote_ip")]
		public string RemoteIp { get; set; }

		[Comment("访问时间")]
		[Column("accessed_time")]
		public DateTime AccessedTime { get; set; }

		[Comment("运行时间")]
		[Column("elapsed_time")]
		public long ElapsedTime { get; set; }

		[Comment("请求内容")]
		[Column("request_body")]
		public string RequestBody { get; set; }

		[Comment("返回状态码")]
		[Column("status_code")]
		public int StatusCode { get; set; }

		[Comment("响应内容")]
		[Column("response_body")]
		public string ResponseBody { get; set; }
	}
}
