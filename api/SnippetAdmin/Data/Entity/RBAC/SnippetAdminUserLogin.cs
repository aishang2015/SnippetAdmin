using Microsoft.AspNetCore.Identity;
using SnippetAdmin.Data.Cache;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.RBAC
{
    [Table("T_RBAC_UserLogin")]
    [Cachable]
    public class SnippetAdminUserLogin : IdentityUserLogin<int>
    {
    }
}
