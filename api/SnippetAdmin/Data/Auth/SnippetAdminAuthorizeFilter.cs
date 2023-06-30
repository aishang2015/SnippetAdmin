using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Extensions;
using SnippetAdmin.Data.Entity.Rbac;

namespace SnippetAdmin.Data.Auth
{
	/// <summary>
	/// 修改为使用策略模式的认证模式
	/// </summary>
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
			if (!_dbContext.Users.Any(u =>
				u.UserName == _httpContextAccessor.HttpContext.User.GetUserName() && u.IsActive))
			{
				context.Result = new StatusCodeResult(403);
				return;
			}

			// get all user role
			var userId = _dbContext.Users.First(u =>
				u.UserName == _httpContextAccessor.HttpContext.User.GetUserName()).Id;
			var userRoles = _dbContext.UserRoles.Where(ur => ur.UserId == userId);
			if (userRoles == null || !userRoles.Any())
			{
				context.Result = new StatusCodeResult(403);
				return;
			}

			// get actived roles
			var roleIds = _dbContext.Roles
				 .Where(r => userRoles.Select(ur => ur.RoleId).Contains(r.Id) && r.IsActive)
				 .Select(r => r.Id);

			// get role elements id
			var elementIds = _dbContext.RoleClaims
				.Where(rc => rc.ClaimType == ClaimConstant.RoleRight && roleIds.Contains(rc.RoleId))
				.Select(rc => int.Parse(rc.ClaimValue));

			// get all api that could access
			var apiList = _dbContext.RbacElements
				.Where(e => elementIds.Contains(e.Id))
				.Select(e => e.AccessApi)
				.ToList()
				.SelectMany(api => api.ToLower().Split(",").ToList())
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