using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Core.Dynamic.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.System
{
    [DynamicApi("登录日志")]
    [Comment("登录日志")]
    [Table("T_Sys_LoginLog")]
    public class SysLoginLog
    {
        [Comment("主键")]
        [Column("id")]
        public int Id { get; set; }

        [Comment("访问用户名")]
        [Column("username", TypeName = "varchar(200)")]
        public string Username { get; set; }

        [Comment("访问者ip")]
        [Column("remote_ip", TypeName = "varchar(20)")]
        public string RemoteIp { get; set; }

        [Comment("访问时间")]
        [Column("accessed_time")]
        public DateTime AccessedTime { get; set; }

        [Comment("是否成功")]
        [Column("is_succeed")]
        public bool IsSucceed { get; set; }
    }
}
