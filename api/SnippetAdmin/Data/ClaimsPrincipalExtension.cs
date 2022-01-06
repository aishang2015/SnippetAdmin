using System.Security.Claims;

namespace SnippetAdmin.Data
{
    public static class ClaimsPrincipalExtension
    {
        public static string UserName(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Name);
        }

        public static string UserClaimValue(this ClaimsPrincipal user, string claimType)
        {
            return user.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
        }
    }
}
