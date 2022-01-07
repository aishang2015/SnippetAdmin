using Microsoft.AspNetCore.Identity;
using SnippetAdmin.Data.Cache;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.RBAC
{
    [Table("T_RBAC_UserToken")]
    [Cachable]
    public class SnippetAdminUserToken : IdentityUserToken<int> { }
}
