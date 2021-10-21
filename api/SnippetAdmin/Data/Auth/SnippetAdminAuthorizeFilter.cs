using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using SnippetAdmin.Core.UserAccessor;
using SnippetAdmin.Data.Cache;
using System.Collections.Generic;
using System.Linq;

namespace SnippetAdmin.Data.Auth
{
    public class SnippetAdminAuthorizeFilter : IAuthorizationFilter
    {
        private IMemoryCache _memoryCache;

        private IUserAccessor _userAccessor;

        private IHttpContextAccessor _httpContextAccessor;

        public SnippetAdminAuthorizeFilter(
            IMemoryCache memoryCache,
            IUserAccessor userAccessor,
            IHttpContextAccessor httpContextAccessor)
        {
            _memoryCache = memoryCache;
            _userAccessor = userAccessor;
            _httpContextAccessor = httpContextAccessor;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var isUserActive = _memoryCache.GetUserIsActive(_userAccessor.UserName);
            if (!isUserActive)
            {
                context.Result = new StatusCodeResult(403);
                return;
            }

            var userId = _memoryCache.GetUserId(_userAccessor.UserName);
            var userRoles = _memoryCache.GetUserRole(userId);
            if (userRoles == null)
            {
                context.Result = new StatusCodeResult(403);
                return;
            }
            var roleElements = new List<int>();
            userRoles.Where(roleId => _memoryCache.GetRoleIsActive(roleId)).ToList()
                .ForEach(roleid => roleElements.AddRange(_memoryCache.GetRoleElement(roleid)));
            var apiList = new List<string>();
            roleElements.ForEach(element => apiList.AddRange(_memoryCache.GetElementApi(element)));

            var path = _httpContextAccessor.HttpContext.Request.Path.Value
                ?.TrimStart('/').ToLower();

            if (!apiList.Contains(path))
            {
                context.Result = new StatusCodeResult(403);
            }
        }
    }
}