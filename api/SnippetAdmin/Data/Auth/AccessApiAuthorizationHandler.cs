using Microsoft.AspNetCore.Authorization;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Extensions;
using SnippetAdmin.Data.Entity.Rbac;

namespace SnippetAdmin.Data.Auth
{
    public class AccessApiAuthorizationHandler : AuthorizationHandler<AccessApiRequirement>
    {
        private readonly SnippetAdminDbContext _dbContext;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccessApiAuthorizationHandler(
            SnippetAdminDbContext dbContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            AccessApiRequirement requirement)
        {
            // user not exist or is not actived
            if (!_dbContext.CacheSet<RbacUser>().Any(u =>
                u.UserName == _httpContextAccessor.HttpContext.User.GetUserName() && u.IsActive))
            {
                return Task.CompletedTask;
            }

            // get all user role
            var userId = _dbContext.CacheSet<RbacUser>().First(u =>
                u.UserName == _httpContextAccessor.HttpContext.User.GetUserName()).Id;
            var userRoles = _dbContext.CacheSet<RbacUserRole>().Where(ur => ur.UserId == userId);
            if (userRoles == null || !userRoles.Any())
            {
                return Task.CompletedTask;
            }

            // get actived roles
            var roleIds = _dbContext.CacheSet<RbacRole>()
                 .Where(r => userRoles.Select(ur => ur.RoleId).Contains(r.Id) && r.IsActive)
                 .Select(r => r.Id);

            // get role elements id
            var elementIds = _dbContext.CacheSet<RbacRoleClaim>()
                .Where(rc => rc.ClaimType == ClaimConstant.RoleRight && roleIds.Contains(rc.RoleId))
                .Select(rc => int.Parse(rc.ClaimValue));

            // get all api that could access
            var apiList = _dbContext.CacheSet<RbacElement>()
                .Where(e => elementIds.Contains(e.Id))
                .Select(e => e.AccessApi.ToLower().Split(",").ToList())
                .SelectMany(e => e)
                .Distinct();

            // check have right to access this api
            var path = _httpContextAccessor.HttpContext.Request.Path.Value
                ?.TrimStart('/').ToLower();

            if (!apiList.Contains(path))
            {
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
