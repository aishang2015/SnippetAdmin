using Microsoft.AspNetCore.Authorization;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Extensions;

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
			if (!_dbContext.Users.Any(u =>
				u.UserName == _httpContextAccessor.HttpContext.User.GetUserName() && u.IsActive))
			{
				return Task.CompletedTask;
			}


			// get actived roles
			var userId = _httpContextAccessor.HttpContext.User.GetUserId();
			var roleIds = (from r in _dbContext.Roles
						   join ur in _dbContext.UserRoles on r.Id equals ur.RoleId
						   where ur.UserId == userId && r.IsActive
						   select r.Id).ToList();


			var apiList = (from rc in _dbContext.RoleClaims
						   join e in _dbContext.RbacElements on rc.ClaimValue equals e.Id.ToString()
						   where roleIds.Contains(rc.RoleId)
						   select e.AccessApi).ToList()
						   .SelectMany(api => api.ToLower().Split(",").ToList()).Distinct();

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
