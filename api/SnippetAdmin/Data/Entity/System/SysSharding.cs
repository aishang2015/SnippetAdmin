using Microsoft.EntityFrameworkCore;
using SnippetAdmin.EntityFrameworkCore.Cache;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.System
{
	[Comment("系统分表记录表")]
	[Table("T_Sys_Sharding")]
	public class SysSharding
	{
		public int Id { get; set; }

		[Comment("分表表名")]
		public string TableName { get; set; }

		[Comment("分表类型名")]
		public string TableType { get; set; }
	}
}
