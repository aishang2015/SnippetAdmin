using Microsoft.AspNetCore.Identity;
using SnippetAdmin.Data.Cache;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.RBAC
{
    [Table("T_RBAC_RoleClaim")]
    [Cachable]
    public class SnippetAdminRoleClaim : IdentityRoleClaim<int> { }
}
