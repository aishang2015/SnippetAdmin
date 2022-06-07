using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Data.Cache;
using SnippetAdmin.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.Rbac
{
    [Comment("系统用户")]
    [Table("T_Rbac_User")]
    [Cachable]
    public class RbacUser : IdentityUser<int>
    {
        [Comment("头像")]
        public string Avatar { get; set; }

        [Comment("姓名")]
        public string RealName { get; set; }

        [Comment("性别")]
        public Gender Gender { get; set; }

        [Comment("电话")]
        public new string PhoneNumber { get; set; }

        [Comment("是否激活")]
        public bool IsActive { get; set; }

        [Comment("Github用户标识")]
        public int? GithubId { get; set; }

        [Comment("Baidu用户标识")]
        public string BaiduId { get; set; }
    }
}