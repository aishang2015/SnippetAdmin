using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using SnippetAdmin.Constants;
using SnippetAdmin.Data.Entity.RBAC;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SnippetAdmin.Data.Cache
{
    public class CacheSavingInterceptor : ISaveChangesInterceptor
    {
        private readonly IMemoryCache _memoryCache;

        private List<EntityEntry<SnippetAdminUser>> _addOrUpdataUsers;

        private List<EntityEntry<SnippetAdminUser>> _deleteUsers;

        private List<EntityEntry<IdentityUserRole<int>>> _addUserRoles;
        private List<EntityEntry<IdentityUserRole<int>>> _updateUserRoles;
        private List<EntityEntry<IdentityUserRole<int>>> _deleteUserRoles;

        private List<EntityEntry<IdentityRoleClaim<int>>> _addRoleClaims;
        private List<EntityEntry<IdentityRoleClaim<int>>> _deleteRoleClaims;

        private List<EntityEntry<Element>> _addOrUpdateElements;
        private List<EntityEntry<Element>> _deleteElements;

        public CacheSavingInterceptor(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void SaveChangesFailed(DbContextErrorEventData eventData)
        {
        }

        public Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            ConfirmCachingData(eventData);
            return result;
        }

        public ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            ConfirmCachingData(eventData);
            return new ValueTask<int>(result);
        }

        public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            StoreCachingData(eventData);
            return result;
        }

        public ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            StoreCachingData(eventData);
            return new ValueTask<InterceptionResult<int>>(result);
        }

        private void StoreCachingData(DbContextEventData eventData)
        {
            _addOrUpdataUsers = eventData.Context.ChangeTracker.Entries<SnippetAdminUser>()
                .Where(e => e.State is EntityState.Added or EntityState.Modified).ToList();
            _deleteUsers = eventData.Context.ChangeTracker.Entries<SnippetAdminUser>()
                .Where(e => e.State is EntityState.Deleted).ToList();

            _addUserRoles = eventData.Context.ChangeTracker.Entries<IdentityUserRole<int>>()
                .Where(e => e.State is EntityState.Added).ToList();
            _updateUserRoles = eventData.Context.ChangeTracker.Entries<IdentityUserRole<int>>()
                .Where(e => e.State is EntityState.Modified).ToList();
            _deleteUserRoles = eventData.Context.ChangeTracker.Entries<IdentityUserRole<int>>()
                .Where(e => e.State is EntityState.Deleted).ToList();

            _addRoleClaims = eventData.Context.ChangeTracker.Entries<IdentityRoleClaim<int>>()
                .Where(e => e.Entity.ClaimType == ClaimConstant.RoleRight)
                .Where(e => e.State is EntityState.Added).ToList();
            _deleteRoleClaims = eventData.Context.ChangeTracker.Entries<IdentityRoleClaim<int>>()
                .Where(e => e.Entity.ClaimType == ClaimConstant.RoleRight)
                .Where(e => e.State is EntityState.Deleted).ToList();

            _addOrUpdateElements = eventData.Context.ChangeTracker.Entries<Element>()
                .Where(e => e.State is EntityState.Added or EntityState.Modified).ToList();
            _deleteElements = eventData.Context.ChangeTracker.Entries<Element>()
                .Where(e => e.State is EntityState.Deleted).ToList();
        }

        private void ConfirmCachingData(SaveChangesCompletedEventData eventData)
        {
            // 缓存用户id和用户名
            _addOrUpdataUsers.ForEach(e => _memoryCache.SetUserId(e.Entity.UserName, e.Entity.Id));
            _deleteUsers.ForEach(e => _memoryCache.RemoveUserId(e.Entity.UserName));

            // 缓存用户角色关系
            _addUserRoles.ForEach(e =>
            {
                var cache = _memoryCache.GetUserRole(e.Entity.UserId) ?? new List<int>();
                cache.Add(e.Entity.RoleId);
                _memoryCache.SetUserRole(e.Entity.UserId, cache);
            });
            _updateUserRoles.ForEach(e =>
            {
                var cache = _memoryCache.GetUserRole(e.Entity.UserId);
                cache.Remove(e.OriginalValues.GetValue<int>("UserId"));
                cache.Add(e.Entity.RoleId);
                _memoryCache.SetUserRole(e.Entity.UserId, cache);
            });
            _deleteUserRoles.ForEach(e =>
            {
                var cache = _memoryCache.GetUserRole(e.Entity.UserId);
                cache.Remove(e.Entity.RoleId);
                _memoryCache.SetUserRole(e.Entity.UserId, cache);
            });

            // 缓存角色和元素关联信息
            _addRoleClaims.ForEach(e =>
            {
                var cache = _memoryCache.GetRoleElement(e.Entity.RoleId) ?? new List<int>();
                cache.Add(int.Parse(e.Entity.ClaimValue));
                _memoryCache.SetRoleElement(e.Entity.RoleId, cache);
            });
            _deleteRoleClaims.ForEach(e =>
            {
                var cache = _memoryCache.GetRoleElement(e.Entity.RoleId);
                cache.Remove(int.Parse(e.Entity.ClaimValue));
                _memoryCache.SetRoleElement(e.Entity.RoleId, cache);
            });

            // 缓存元素信息
            _addOrUpdateElements.ForEach(e => _memoryCache.SetElementApi(e.Entity.Id, e.Entity.AccessApi.ToLower().Split(",").ToList()));
            _deleteElements.ForEach(e => _memoryCache.RemoveElementApi(e.Entity.Id));
        }
    }
}