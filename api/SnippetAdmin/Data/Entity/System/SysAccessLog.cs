using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.System
{
	[Comment("接口访问日志")]
	[Table("T_Sys_AccessLog")]
    [Index(nameof(TraceIdentifier))]
    [Index(nameof(AccessedTime))]
	[Index(nameof(Path))]
	[Index(nameof(RemoteIp))]
	[Index(nameof(ElapsedTime))]
	public class SysAccessLog
    {
        [Comment("主键")]
        public int Id { get; set; }

        [Comment("跟踪标识")]
        public string? TraceIdentifier { get; set; }

        [Comment("访问的模块")]
        public string? Module { get; set; }

        [Comment("访问方法")]
        public string? Method { get; set; }

        [Comment("访问路径")]
        public string? Path { get; set; }

        [Comment("访问用户ID")]
        public int UserId { get; set; }

        [Comment("访问者ip")]
        public string? RemoteIp { get; set; }

        [Comment("访问时间")]
        public DateTime AccessedTime { get; set; }

        [Comment("运行时间")]
        public long ElapsedTime { get; set; }

        [Comment("请求内容")]
        public string? RequestBody { get; set; }

        [Comment("返回状态码")]
        public int StatusCode { get; set; }

        [Comment("响应内容")]
        public string? ResponseBody { get; set; }
    }
}
