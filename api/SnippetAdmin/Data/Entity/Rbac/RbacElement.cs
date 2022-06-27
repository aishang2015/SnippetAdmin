//------------------------------------------------------------------------------
// 生成时间 2021-09-01 11:06:40
//------------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Data.Cache;
using SnippetAdmin.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.Rbac;

[Comment("元素树")]
[Table("T_Rbac_Element")]
[Cachable]
public class RbacElement
{
    [Comment("主键")]
    [Column("id")]
    public int Id { get; set; }

    [Comment("元素名称")]
    [Column("name")]
    public string Name { get; set; }

    [Comment("元素类型")]
    [Column("type")]
    public ElementType Type { get; set; }

    [Comment("元素标识")]
    [Column("identity")]
    public string Identity { get; set; }

    [Comment("访问接口")]
    [Column("access_api")]
    public string AccessApi { get; set; }

    [Comment("排序")]
    [Column("sorting")]
    public int Sorting { get; set; }
}