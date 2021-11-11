using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.System
{
    [Comment("刷新Token")]
    [Table("T_System_RefreshToken")]
    public class RefreshToken
    {
        public int Id { get; set; }

        [Comment("用户名")]
        public string UserName { get; set; }

        [Comment("Token内容")]
        public string Content { get; set; }

        [Comment("Token过期时间")]
        public DateTime ExpireTime { get; set; }

        [Comment("Token生成时间")]
        public DateTime CreatedTime { get; set; }
    }
}
