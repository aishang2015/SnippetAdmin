using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SnippetAdmin.Constants;
using SnippetAdmin.Data.Entity.RBAC;

namespace SnippetAdmin.Data.Auth
{
    public class SnippetAdminAuthorizeFilter : IAuthorizationFilter
    {
        private readonly SnippetAdminDbContext _dbContext;


        private readonly IHttpContextAccessor _httpContextAccessor;

        public SnippetAdminAuthorizeFilter(
            SnippetAdminDbContext dbContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // user not exist or is not actived
            if (!_dbContext.CacheSet<SnippetAdminUser>().Any(u =>
                u.UserName == _httpContextAccessor.HttpContext.User.UserName() && u.IsActive))
            {
                context.Result = new StatusCodeResult(403);
                return;
            }

            // get all user role
            var userId = _dbContext.CacheSet<SnippetAdminUser>().First(u =>
                u.UserName == _httpContextAccessor.HttpContext.User.UserName()).Id;
            var userRoles = _dbContext.CacheSet<IdentityUserRole<int>>().Where(ur => ur.UserId == userId);
            if (userRoles == null || !userRoles.Any())
            {
                context.Result = new StatusCodeResult(403);
                return;
            }

            // get actived roles
            var roleIds = _dbContext.CacheSet<SnippetAdminRole>()
                 .Where(r => userRoles.Select(ur => ur.RoleId).Contains(r.Id) && r.IsActive)
                 .Select(r => r.Id);

            // get role elements id
            var elementIds = _dbContext.CacheSet<IdentityRoleClaim<int>>()
                .Where(rc => rc.ClaimType == ClaimConstant.RoleRight && roleIds.Contains(rc.RoleId))
                .Select(rc => int.Parse(rc.ClaimValue));

            // get all api that could access
            var apiList = _dbContext.CacheSet<Element>()
                .Where(e => elementIds.Contains(e.Id))
                .Select(e => e.AccessApi.ToLower().Split(",").ToList())
                .SelectMany(e => e)
                .Distinct();

            // check have right to access this api
            var path = _httpContextAccessor.HttpContext.Request.Path.Value
                ?.TrimStart('/').ToLower();

            if (!apiList.Contains(path))
            {
                context.Result = new StatusCodeResult(403);
            }
        }
    }
}