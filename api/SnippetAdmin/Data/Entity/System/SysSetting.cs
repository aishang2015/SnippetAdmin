using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.System
{
    [Comment("系统配置表")]
    [Table("T_Sys_Setting")]
    public class SysSetting
    {
        public int Id { get; set; }

        [Comment("系统配置名称")]
        [Column("setting_type")]
        public SettingType SettingType { get; set; }

        [Comment("系统配置值")]
        [Column("value", TypeName = "varchar(1000)")]
        public string Value { get; set; }
    }
}
