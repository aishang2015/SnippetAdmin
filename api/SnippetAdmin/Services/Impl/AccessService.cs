using SnippetAdmin.Constants;
using SnippetAdmin.Core.Exceptions;
using SnippetAdmin.Core.Extensions;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Entity.Rbac;

namespace SnippetAdmin.Services.Impl
{
	public class AccessService : IAccessService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		private readonly SnippetAdminDbContext _dbContext;

		public AccessService(IHttpContextAccessor httpContextAccessor, SnippetAdminDbContext dbContext)
		{
			_httpContextAccessor = httpContextAccessor;
			_dbContext = dbContext;
		}

		/// <summary>
		/// get current request's user's organization ids not contained lower levels org id
		/// </summary> 
		public List<int> GetCurrentUserOrgId()
		{
			var userName = _httpContextAccessor.HttpContext.User.GetUserName();
			var user = _dbContext.CacheSet<RbacUser>().FirstOrDefault(u => u.UserName == userName);

			if (user == null)
			{
				throw new UserNotFoundException();
			}

			return _dbContext.CacheSet<RbacUserClaim>()
				.Where(uc => uc.UserId == user.Id && uc.ClaimType == ClaimConstant.UserOrganization)
				.Select(uc => int.Parse(uc.ClaimValue))
				.ToList();
		}

		/// <summary>
		/// get current request's user's organization ids contains all lower levels org ids  
		/// </summary> 
		public List<int> GetCurrentUserGroupOrgId()
		{
			var userName = _httpContextAccessor.HttpContext.User.GetUserName();
			var user = _dbContext.CacheSet<RbacUser>().FirstOrDefault(u => u.UserName == userName);

			if (user == null)
			{
				throw new UserNotFoundException();
			}

			var userOrgIds = _dbContext.CacheSet<RbacUserClaim>()
				.Where(uc => uc.UserId == user.Id && uc.ClaimType == ClaimConstant.UserOrganization)
				.Select(uc => int.Parse(uc.ClaimValue))
				.ToList();

			return _dbContext.CacheSet<RbacOrganizationTree>()
				.Where(o => userOrgIds.Contains(o.Ancestor))
				.Select(o => o.Descendant).Distinct().ToList();
		}
	}
}
