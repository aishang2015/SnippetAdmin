using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Data.Enums;
using SnippetAdmin.EntityFrameworkCore.Cache;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.Rbac
{
    [Comment("系统用户")]
    [Table("T_Rbac_User")]
    [Cachable]
    public class RbacUser : IdentityUser<int>
    {
        [Comment("主键")]
        public override int Id { get; set; }

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

        public int? GithubId { get; set; }

        public string BaiduId { get; set; }

        [Comment("是否删除")]
        public bool IsDeleted { get; set; }

    }
}