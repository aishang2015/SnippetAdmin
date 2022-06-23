using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.System
{
    [Comment("字典类型")]
    [Table("T_Sys_DicType")]
    public class SysDicType
    {
        public int Id { get; set; }
    }
}
