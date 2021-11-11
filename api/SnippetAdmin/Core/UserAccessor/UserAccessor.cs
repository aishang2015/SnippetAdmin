using System.Security.Claims;

namespace SnippetAdmin.Core.UserAccessor
{
    public class UserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal UserInfo => _httpContextAccessor.HttpContext.User;

        public string UserName => _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
    }
}