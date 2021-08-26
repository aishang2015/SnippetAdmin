using System.Security.Claims;

namespace SnippetAdmin.Core.UserAccessor
{
    public interface IUserAccessor
    {
        public ClaimsPrincipal UserInfo { get; }
        public string UserName { get; }
    }
}