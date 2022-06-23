using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.System
{
    [Comment("字典值")]
    [Table("T_Sys_DicValue")]
    public class SysDicValue
    {
        public int Id { get; set; }
    }
}
