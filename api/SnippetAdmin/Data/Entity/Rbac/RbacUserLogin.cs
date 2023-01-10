using Microsoft.AspNetCore.Identity;
using SnippetAdmin.EntityFrameworkCore.Cache;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnippetAdmin.Data.Entity.Rbac
{
	[Table("T_Rbac_UserLogin")]
	[Cachable]
	public class RbacUserLogin : IdentityUserLogin<int>
	{
	}
}
