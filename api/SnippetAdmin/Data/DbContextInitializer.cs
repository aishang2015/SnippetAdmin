﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SnippetAdmin.Constants;
using SnippetAdmin.Data.Cache;
using SnippetAdmin.Data.Entity.Rbac;
using SnippetAdmin.Data.Enums;
using System.Collections.Concurrent;

namespace SnippetAdmin.Data
{
    public class DbContextInitializer
    {
        public static ConcurrentDictionary<Type, bool> CacheAbleDic = new ConcurrentDictionary<Type, bool>();

        /// <summary>
        /// 初始化内存缓存
        /// </summary>
        private static readonly Action<IMemoryCache, SnippetAdminDbContext> InitialCache =
            (memoryCache, dbcontext) =>
            {
                var dbSetPropertyTypes = dbcontext.GetType().GetProperties()
                    .Where(property =>
                        property.PropertyType.IsGenericType && (
                        typeof(DbSet<>).IsAssignableFrom(property.PropertyType.GetGenericTypeDefinition()) ||
                        property.PropertyType.GetInterface(typeof(DbSet<>).FullName) != null))
                    .ToList();

                var toListMethod = typeof(DbContextInitializer).GetMethod("GetDataList");

                dbSetPropertyTypes.ForEach(dbSetProperty =>
                {
                    // 判断实体的cacheable特性
                    var entityType = dbSetProperty.PropertyType.GetGenericArguments()[0];
                    if (CacheAbleDic[entityType])
                    {
                        var method = toListMethod.MakeGenericMethod(entityType);
                        var data = method.Invoke(new DbContextInitializer(), new object[] { dbcontext });
                        memoryCache.Set(dbSetProperty.PropertyType.GetGenericArguments()[0].FullName, data);
                    }
                });
            };

        /// <summary>
        /// 根据cache特性进行判断，哪些实体可以被缓存
        /// </summary>
        private static Action<SnippetAdminDbContext> _setCacheAbleDic = (dbContext) =>
        {

            // 取得dbcontext的所有实体的缓存特性
            var dbSetPropertyTypes = dbContext.GetType().GetProperties()
                .Where(property =>
                    property.PropertyType.IsGenericType && (
                    typeof(DbSet<>).IsAssignableFrom(property.PropertyType.GetGenericTypeDefinition()) ||
                    property.PropertyType.GetInterface(typeof(DbSet<>).FullName) != null))
                .ToList();
            dbSetPropertyTypes.ForEach(dbSetProperty =>
            {
                // 判断实体的cacheable特性
                var entityType = dbSetProperty.PropertyType.GetGenericArguments()[0];
                var cacheAttribute = entityType.GetCustomAttributes(typeof(CachableAttribute), false).FirstOrDefault();
                CacheAbleDic.TryAdd(entityType, cacheAttribute != null && (cacheAttribute as CachableAttribute).CacheAble);
            });
        };

        private static Action<IMemoryCache, SnippetAdminDbContext, UserManager<RbacUser>,
            RoleManager<RbacRole>, ILogger<SnippetAdminDbContext>>
            _initialSnippetAdminDbContext = (memoryCache, dbContext, userManager, roleManager, logger) =>
            {
                logger.LogInformation("开始执行数据库初始化操作。");

                _setCacheAbleDic(dbContext);

                //dbContext.Database.Migrate();

                dbContext.Database.EnsureDeleted();

                // 加载用户数据
                if (dbContext.Database.EnsureCreated())
                {
                    // 这里是为了确保数据库表存在，防止delete库之后表不存在的问题
                    InitialCache(memoryCache, dbContext);

                    // 初始化权限
                    logger.LogInformation("初始化权限数据。");
                    InitialElements(dbContext).Wait();
                    logger.LogInformation("初始化权限数据完成。");

                    // 初始化用户角色
                    logger.LogInformation("初始化用户角色数据。");
                    InitialUserRoles(dbContext, userManager, roleManager).Wait();
                    logger.LogInformation("初始化用户角色完成。");
                }
                else
                {
                    InitialCache(memoryCache, dbContext);
                }


                logger.LogInformation("初始化数据操作执行完毕。");
            };

        public static Action<IMemoryCache, SnippetAdminDbContext, UserManager<RbacUser>, RoleManager<RbacRole>, ILogger<SnippetAdminDbContext>> InitialSnippetAdminDbContext
        {
            get => _initialSnippetAdminDbContext;
        }

        /// <summary>
        /// 初始化页面菜单
        /// </summary>
        /// <param name="_dbContext"></param>
        /// <returns></returns>
        private static async Task InitialElements(SnippetAdminDbContext _dbContext)
        {
            // 元素数据
            _dbContext.RbacElements.Add(new RbacElement { Id = 1, Name = "主页", Identity = "home", Type = ElementType.Menu, AccessApi = "", Sorting = 1 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 2, Name = "权限管理", Identity = "rbac", Type = ElementType.Menu, AccessApi = "", Sorting = 2 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 3, Name = "关于", Identity = "about", Type = ElementType.Menu, AccessApi = "", Sorting = 5 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 7, Name = "用户信息", Identity = "user", Type = ElementType.Menu, AccessApi = "", Sorting = 1 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 9, Name = "角色信息", Identity = "role", Type = ElementType.Menu, AccessApi = "", Sorting = 2 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 10, Name = "组织信息", Identity = "org", Type = ElementType.Menu, AccessApi = "", Sorting = 4 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 11, Name = "页面权限", Identity = "permission", Type = ElementType.Menu, AccessApi = "", Sorting = 5 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 12, Name = "添加成员", Identity = "add-member", Type = ElementType.ButtonLink, AccessApi = "api/Organization/GetPositionDic,api/User/AddOrgMember", Sorting = 0 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 13, Name = "创建用户", Identity = "create-user", Type = ElementType.ButtonLink, AccessApi = "api/User/AddOrUpdateUser", Sorting = 0 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 16, Name = "编辑用户", Identity = "edit-user", Type = ElementType.ButtonLink, AccessApi = "api/User/AddOrUpdateUser", Sorting = 0 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 17, Name = "删除用户", Identity = "remove-user", Type = ElementType.ButtonLink, AccessApi = "api/User/RemoveUser", Sorting = 0 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 18, Name = "移出组织", Identity = "move-out", Type = ElementType.ButtonLink, AccessApi = "api/User/RemoveOrgMember", Sorting = 0 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 19, Name = "创建角色", Identity = "create-role", Type = ElementType.ButtonLink, AccessApi = "api/Role/AddOrUpdateRole", Sorting = 0 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 20, Name = "编辑角色", Identity = "edit-role", Type = ElementType.ButtonLink, AccessApi = "api/Role/AddOrUpdateRole", Sorting = 0 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 21, Name = "删除角色", Identity = "remove-role", Type = ElementType.ButtonLink, AccessApi = "api/Role/RemoveRole", Sorting = 0 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 22, Name = "创建组织", Identity = "create-org", Type = ElementType.ButtonLink, AccessApi = "api/Organization/CreateOrganization", Sorting = 0 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 23, Name = "编辑组织", Identity = "edit-org", Type = ElementType.ButtonLink, AccessApi = "api/Organization/UpdateOrganization", Sorting = 0 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 24, Name = "删除组织", Identity = "remove-org", Type = ElementType.ButtonLink, AccessApi = "api/Organization/DeleteOrganization", Sorting = 0 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 25, Name = "职位设置", Identity = "set-pos", Type = ElementType.ButtonLink, AccessApi = "api/Organization/SetPosition", Sorting = 0 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 26, Name = "添加页面元素", Identity = "add-element", Type = ElementType.ButtonLink, AccessApi = "api/Element/CreateElement", Sorting = 0 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 27, Name = "导出", Identity = "export-element", Type = ElementType.ButtonLink, AccessApi = "api/Element/ExportElementData", Sorting = 0 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 28, Name = "编辑", Identity = "edit-element", Type = ElementType.ButtonLink, AccessApi = "api/Element/UpdateElement", Sorting = 0 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 29, Name = "删除元素", Identity = "remove-element", Type = ElementType.ButtonLink, AccessApi = "api/Element/DeleteElement", Sorting = 0 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 30, Name = "设定密码", Identity = "set-password", Type = ElementType.ButtonLink, AccessApi = "api/User/SetUserPassword", Sorting = 0 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 31, Name = "用户激活", Identity = "active-user", Type = ElementType.ButtonLink, AccessApi = "api/User/ActiveUser", Sorting = 0 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 32, Name = "页面", Identity = "user-page", Type = ElementType.ButtonLink, AccessApi = "api/Organization/GetOrganizationTree,api/Role/GetRoleDic,api/User/SearchUser,api/User/GetUser,api/Position/GetPositionDic", Sorting = -1 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 33, Name = "页面", Identity = "role-page", Type = ElementType.ButtonLink, AccessApi = "api/Role/GetRoles,api/Element/GetElementTree,api/Role/GetRole", Sorting = -1 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 35, Name = "页面", Identity = "org-page", Type = ElementType.ButtonLink, AccessApi = "api/Organization/GetOrganization,api/Organization/GetOrganizationTree,api/Organization/GetOrganizationTypes", Sorting = -1 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 36, Name = "页面", Identity = "right-page", Type = ElementType.ButtonLink, AccessApi = "api/ApiInfo/GetApiPaths,api/Element/GetElement,api/Element/GetElementTree", Sorting = -1 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 37, Name = "激活角色", Identity = "active-role", Type = ElementType.ButtonLink, AccessApi = "api/Role/ActiveRole", Sorting = 0 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 38, Name = "创建或编辑组织类型", Identity = "add-update-org-type", Type = ElementType.ButtonLink, AccessApi = "api/Organization/AddOrUpdateOrganizationType", Sorting = 0 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 39, Name = "删除组织类型", Identity = "remove-org-type", Type = ElementType.ButtonLink, AccessApi = "api/Organization/RemoveOrganizationType", Sorting = 0 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 40, Name = "职位信息", Identity = "position", Type = ElementType.Menu, AccessApi = "", Sorting = 4 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 41, Name = "页面", Identity = "position-page", Type = ElementType.ButtonLink, AccessApi = "api/Position/GetPositions,api/Position/GetPosition", Sorting = -1 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 42, Name = "添加", Identity = "add-position", Type = ElementType.ButtonLink, AccessApi = "api/Position/AddOrUpdatePosition", Sorting = 0 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 45, Name = "编辑 ", Identity = "edit-position", Type = ElementType.ButtonLink, AccessApi = "api/Position/AddOrUpdatePosition", Sorting = 0 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 46, Name = "删除", Identity = "delete-position", Type = ElementType.ButtonLink, AccessApi = "api/Position/DeletePosition", Sorting = 0 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 47, Name = "任务调度", Identity = "task", Type = ElementType.Menu, AccessApi = "", Sorting = 3 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 48, Name = "系统设置", Identity = "system", Type = ElementType.Menu, AccessApi = "", Sorting = 4 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 49, Name = "任务管理", Identity = "taskManage", Type = ElementType.Menu, AccessApi = "", Sorting = 1 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 50, Name = "任务记录", Identity = "taskRecord", Type = ElementType.Menu, AccessApi = "", Sorting = 2 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 51, Name = "访问记录", Identity = "accessRecord", Type = ElementType.Menu, AccessApi = "", Sorting = 1 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 52, Name = "异常记录", Identity = "exceptionRecord", Type = ElementType.Menu, AccessApi = "", Sorting = 2 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 53, Name = "页面", Identity = "taskManagePage", Type = ElementType.ButtonLink, AccessApi = "", Sorting = -1 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 54, Name = "页面", Identity = "taskRecordPage", Type = ElementType.ButtonLink, AccessApi = "", Sorting = -1 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 55, Name = "页面", Identity = "accessRecordPage", Type = ElementType.ButtonLink, AccessApi = "", Sorting = -1 });
            _dbContext.RbacElements.Add(new RbacElement { Id = 56, Name = "页面", Identity = "exceptionRecordPage", Type = ElementType.ButtonLink, AccessApi = "", Sorting = -1 });

            // 元素树数据
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 1, Ancestor = 1, Descendant = 1, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 2, Ancestor = 2, Descendant = 2, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 3, Ancestor = 3, Descendant = 3, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 4, Ancestor = 2, Descendant = 4, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 5, Ancestor = 4, Descendant = 4, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 6, Ancestor = 2, Descendant = 5, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 7, Ancestor = 5, Descendant = 5, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 8, Ancestor = 6, Descendant = 6, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 9, Ancestor = 2, Descendant = 7, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 10, Ancestor = 7, Descendant = 7, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 11, Ancestor = 2, Descendant = 8, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 12, Ancestor = 8, Descendant = 8, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 13, Ancestor = 2, Descendant = 9, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 14, Ancestor = 9, Descendant = 9, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 15, Ancestor = 2, Descendant = 10, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 16, Ancestor = 10, Descendant = 10, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 17, Ancestor = 2, Descendant = 11, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 18, Ancestor = 11, Descendant = 11, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 19, Ancestor = 2, Descendant = 12, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 20, Ancestor = 7, Descendant = 12, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 21, Ancestor = 12, Descendant = 12, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 22, Ancestor = 2, Descendant = 13, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 23, Ancestor = 7, Descendant = 13, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 24, Ancestor = 13, Descendant = 13, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 25, Ancestor = 14, Descendant = 14, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 26, Ancestor = 15, Descendant = 15, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 27, Ancestor = 2, Descendant = 16, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 28, Ancestor = 7, Descendant = 16, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 29, Ancestor = 16, Descendant = 16, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 30, Ancestor = 2, Descendant = 17, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 31, Ancestor = 7, Descendant = 17, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 32, Ancestor = 17, Descendant = 17, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 33, Ancestor = 2, Descendant = 18, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 34, Ancestor = 7, Descendant = 18, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 35, Ancestor = 18, Descendant = 18, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 36, Ancestor = 2, Descendant = 19, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 37, Ancestor = 9, Descendant = 19, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 38, Ancestor = 19, Descendant = 19, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 39, Ancestor = 2, Descendant = 20, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 40, Ancestor = 9, Descendant = 20, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 41, Ancestor = 20, Descendant = 20, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 42, Ancestor = 2, Descendant = 21, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 43, Ancestor = 9, Descendant = 21, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 44, Ancestor = 21, Descendant = 21, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 45, Ancestor = 2, Descendant = 22, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 46, Ancestor = 10, Descendant = 22, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 47, Ancestor = 22, Descendant = 22, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 48, Ancestor = 2, Descendant = 23, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 49, Ancestor = 10, Descendant = 23, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 50, Ancestor = 23, Descendant = 23, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 51, Ancestor = 2, Descendant = 24, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 52, Ancestor = 10, Descendant = 24, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 53, Ancestor = 24, Descendant = 24, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 54, Ancestor = 2, Descendant = 25, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 55, Ancestor = 10, Descendant = 25, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 56, Ancestor = 25, Descendant = 25, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 57, Ancestor = 2, Descendant = 26, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 58, Ancestor = 11, Descendant = 26, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 59, Ancestor = 26, Descendant = 26, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 60, Ancestor = 2, Descendant = 27, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 61, Ancestor = 11, Descendant = 27, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 62, Ancestor = 27, Descendant = 27, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 63, Ancestor = 2, Descendant = 28, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 64, Ancestor = 11, Descendant = 28, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 65, Ancestor = 28, Descendant = 28, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 66, Ancestor = 2, Descendant = 29, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 67, Ancestor = 11, Descendant = 29, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 68, Ancestor = 29, Descendant = 29, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 69, Ancestor = 2, Descendant = 30, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 70, Ancestor = 7, Descendant = 30, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 71, Ancestor = 30, Descendant = 30, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 72, Ancestor = 2, Descendant = 31, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 73, Ancestor = 7, Descendant = 31, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 74, Ancestor = 31, Descendant = 31, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 75, Ancestor = 2, Descendant = 32, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 76, Ancestor = 7, Descendant = 32, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 77, Ancestor = 32, Descendant = 32, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 78, Ancestor = 2, Descendant = 33, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 79, Ancestor = 9, Descendant = 33, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 80, Ancestor = 33, Descendant = 33, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 81, Ancestor = 34, Descendant = 34, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 82, Ancestor = 2, Descendant = 35, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 83, Ancestor = 10, Descendant = 35, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 84, Ancestor = 35, Descendant = 35, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 85, Ancestor = 2, Descendant = 36, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 86, Ancestor = 11, Descendant = 36, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 87, Ancestor = 36, Descendant = 36, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 88, Ancestor = 2, Descendant = 37, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 89, Ancestor = 9, Descendant = 37, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 90, Ancestor = 37, Descendant = 37, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 91, Ancestor = 2, Descendant = 38, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 92, Ancestor = 10, Descendant = 38, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 93, Ancestor = 38, Descendant = 38, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 94, Ancestor = 2, Descendant = 39, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 95, Ancestor = 10, Descendant = 39, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 96, Ancestor = 39, Descendant = 39, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 97, Ancestor = 2, Descendant = 40, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 98, Ancestor = 40, Descendant = 40, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 99, Ancestor = 2, Descendant = 41, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 100, Ancestor = 40, Descendant = 41, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 101, Ancestor = 41, Descendant = 41, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 102, Ancestor = 2, Descendant = 42, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 103, Ancestor = 40, Descendant = 42, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 104, Ancestor = 42, Descendant = 42, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 105, Ancestor = 43, Descendant = 43, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 106, Ancestor = 44, Descendant = 44, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 107, Ancestor = 2, Descendant = 45, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 108, Ancestor = 40, Descendant = 45, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 109, Ancestor = 45, Descendant = 45, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 110, Ancestor = 2, Descendant = 46, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 111, Ancestor = 40, Descendant = 46, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 112, Ancestor = 46, Descendant = 46, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 113, Ancestor = 47, Descendant = 47, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 114, Ancestor = 48, Descendant = 48, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 115, Ancestor = 47, Descendant = 49, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 116, Ancestor = 49, Descendant = 49, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 117, Ancestor = 47, Descendant = 50, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 118, Ancestor = 50, Descendant = 50, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 119, Ancestor = 48, Descendant = 51, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 120, Ancestor = 51, Descendant = 51, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 121, Ancestor = 48, Descendant = 52, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 122, Ancestor = 52, Descendant = 52, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 123, Ancestor = 47, Descendant = 53, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 124, Ancestor = 49, Descendant = 53, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 125, Ancestor = 53, Descendant = 53, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 126, Ancestor = 47, Descendant = 54, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 127, Ancestor = 50, Descendant = 54, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 128, Ancestor = 54, Descendant = 54, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 129, Ancestor = 48, Descendant = 55, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 130, Ancestor = 51, Descendant = 55, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 131, Ancestor = 55, Descendant = 55, Length = 0 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 132, Ancestor = 48, Descendant = 56, Length = 2 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 133, Ancestor = 52, Descendant = 56, Length = 1 });
            _dbContext.RbacElementTrees.Add(new RbacElementTree { Id = 134, Ancestor = 56, Descendant = 56, Length = 0 });
            await _dbContext.SaveChangesAsync();


        }

        /// <summary>
        /// 初始化用户角色权限
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        private static async Task InitialUserRoles(SnippetAdminDbContext dbContext, UserManager<RbacUser> userManager, RoleManager<RbacRole> roleManager)
        {
            var user = new RbacUser
            {
                UserName = "admin",
                RealName = "admin",
                Email = "admin@admin.com",
                PhoneNumber = "16655558888",
                IsActive = true
            };
            var role = new RbacRole
            {
                Name = "管理员",
                Code = "Administrator",
                IsActive = true
            };
            await userManager.CreateAsync(user, "admin");
            await roleManager.CreateAsync(role);
            await userManager.AddToRoleAsync(user, "管理员");

            // 赋予管理员权限
            foreach (var e in dbContext.RbacElements)
            {
                dbContext.RoleClaims.Add(new RbacRoleClaim
                {
                    RoleId = 1,
                    ClaimType = ClaimConstant.RoleRight,
                    ClaimValue = e.Id.ToString(),
                });
            }
            await dbContext.SaveChangesAsync();
        }

        public List<T> GetDataList<T>(SnippetAdminDbContext dbContext) where T : class
        {
            return dbContext.Set<T>().ToList();
        }
    }
}