using Microsoft.AspNetCore.Identity;
using SnippetAdmin.EntityFrameworkCore.Cache;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.Rbac
{
    [Table("T_Rbac_UserToken")]
    [Cachable]
    public class RbacUserToken : IdentityUserToken<int> { }
}
