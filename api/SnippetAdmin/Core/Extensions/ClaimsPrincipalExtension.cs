using System.Security.Claims;

namespace SnippetAdmin.Core.Extensions
{
	public static class ClaimsPrincipalExtension
	{
		public static string GetUserName(this ClaimsPrincipal user)
		{
			return user.FindFirstValue(ClaimTypes.Name);
		}

		public static int GetUserId(this ClaimsPrincipal user)
		{
			var idStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
			return int.Parse(idStr);
		}

		public static string GetUserClaimValue(this ClaimsPrincipal user, string claimType)
		{
			return user.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
		}
	}
}
