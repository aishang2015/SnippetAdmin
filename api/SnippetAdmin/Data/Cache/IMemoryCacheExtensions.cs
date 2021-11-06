using Microsoft.Extensions.Caching.Memory;
using SnippetAdmin.Data.Entity.Scheduler;
using System.Collections.Generic;

namespace SnippetAdmin.Data.Cache
{
    public static class IMemoryCacheExtensions
    {
        #region username -> userid

        public static int GetUserId(this IMemoryCache memoryCache, string userName)
        {
            return memoryCache.Get<int>($"User{userName}");
        }

        public static void SetUserId(this IMemoryCache memoryCache, string userName, int userId)
        {
            memoryCache.Set($"User{userName}", userId);
        }

        public static void RemoveUserId(this IMemoryCache memoryCache, string userName)
        {
            memoryCache.Remove($"User{userName}");
        }

        #endregion username -> userid

        #region userid -> user roles id

        public static List<int> GetUserRole(this IMemoryCache memoryCache, int userId)
        {
            return memoryCache.Get<List<int>>($"UserRole{userId}");
        }

        public static void SetUserRole(this IMemoryCache memoryCache, int userId, List<int> userRoles)
        {
            memoryCache.Set($"UserRole{userId}", userRoles);
        }

        public static void RemoveUserRole(this IMemoryCache memoryCache, int userId)
        {
            memoryCache.Remove($"UserRole{userId}");
        }

        #endregion userid -> user roles id

        #region role id -> role elements id

        public static List<int> GetRoleElement(this IMemoryCache memoryCache, int roleId)
        {
            return memoryCache.Get<List<int>>($"RoleElement{roleId}");
        }

        public static List<int> SetRoleElement(this IMemoryCache memoryCache, int roleId, List<int> roleElements)
        {
            return memoryCache.Set($"RoleElement{roleId}", roleElements);
        }

        public static void RemoveRoleElement(this IMemoryCache memoryCache, int roleId)
        {
            memoryCache.Remove($"RoleElement{roleId}");
        }

        #endregion role id -> role elements id

        #region element id -> api addresses

        public static List<string> GetElementApi(this IMemoryCache memoryCache, int elementId)
        {
            return memoryCache.Get<List<string>>($"Element{elementId}");
        }

        public static List<string> SetElementApi(this IMemoryCache memoryCache, int elementId, List<string> elementApis)
        {
            return memoryCache.Set($"Element{elementId}", elementApis);
        }

        public static void RemoveElementApi(this IMemoryCache memoryCache, int elementId)
        {
            memoryCache.Remove($"Element{elementId}");
        }

        #endregion element id -> api addresses

        #region username -> isActive
        public static bool GetUserIsActive(this IMemoryCache memoryCache, string userName)
        {
            return memoryCache.Get<bool>($"UserIsActive{userName}");
        }

        public static void SetUserIsActive(this IMemoryCache memoryCache, string userName, bool isActive)
        {
            memoryCache.Set($"UserIsActive{userName}", isActive);
        }

        public static void RemoveUserIsActive(this IMemoryCache memoryCache, string userName)
        {
            memoryCache.Remove($"UserIsActive{userName}");
        }
        #endregion

        #region role id -> isActive
        public static bool GetRoleIsActive(this IMemoryCache memoryCache, int roleId)
        {
            return memoryCache.Get<bool>($"RoleIsActive{roleId}");
        }

        public static void SetRoleIsActive(this IMemoryCache memoryCache, int roleId, bool isActive)
        {
            memoryCache.Set($"RoleIsActive{roleId}", isActive);
        }

        public static void RemoveRoleIsActive(this IMemoryCache memoryCache, int roleId)
        {
            memoryCache.Remove($"RoleIsActive{roleId}");
        }

        #endregion

        #region job config

        public static List<Job> GetJobConfig(this IMemoryCache memoryCache)
        {
            return memoryCache.Get<List<Job>>($"JobConfig");
        }

        public static void SetJobConfig(this IMemoryCache memoryCache, List<Job> jobs)
        {
            memoryCache.Set($"JobConfig", jobs);
        }

        public static void RemoveJobConfig(this IMemoryCache memoryCache)
        {
            memoryCache.Remove($"JobConfig");
        }

        #endregion
    }
}