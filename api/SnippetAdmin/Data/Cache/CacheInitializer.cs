using Microsoft.Extensions.Caching.Memory;
using SnippetAdmin.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SnippetAdmin.Data.Cache
{
    public static class CacheInitializer
    {
        public static Action<IMemoryCache, SnippetAdminDbContext> InitialCache =
            (memoryCache, dbcontext) =>
            {
                // 缓存元素信息
                var elements = dbcontext.Elements.ToList();
                elements.ForEach(e => memoryCache.SetElementApi(e.Id, e.AccessApi.ToLower().Split(",").ToList()));

                // 缓存角色和元素关联信息
                var roleClaims = dbcontext.RoleClaims.ToList()
                    .Where(e => e.ClaimType == ClaimConstant.RoleRight).ToList();
                foreach (var roleId in roleClaims.Select(r => r.RoleId).Distinct())
                {
                    var value = roleClaims.Where(r => r.RoleId == roleId).Select(r => int.Parse(r.ClaimValue)).ToList();
                    memoryCache.SetRoleElement(roleId, value);
                }

                // 缓存用户id和用户名
                var users = dbcontext.Users.ToList();
                users.ForEach(e => memoryCache.SetUserId(e.UserName, e.Id));

                // 缓存用户角色关系
                var userRoles = dbcontext.UserRoles.ToList();
                foreach (var userId in userRoles.Select(ur => ur.UserId).Distinct())
                {
                    var roles = userRoles.Where(ur => ur.UserId == userId).Select(ur => ur.RoleId).ToList();
                    memoryCache.SetUserRole(userId, roles);
                }
            };
    }
}