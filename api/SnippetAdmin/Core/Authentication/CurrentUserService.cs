using Microsoft.AspNetCore.Http;
using System.Linq;

namespace SnippetAdmin.Core.Authentication
{
    public interface ICurrentUserService
    {
        public string GetUserClaim(string claimType);
    }

    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CurrentUserService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string GetUserClaim(string claimType)
        {
            return _contextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(c => c.Type == claimType)?.Value;
        }
    }
}