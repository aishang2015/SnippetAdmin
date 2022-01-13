using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Constants;
using SnippetAdmin.Data.Cache;
using SnippetAdmin.Data.Entity.Enums;
using SnippetAdmin.Data.Entity.RBAC;
using System.Collections.Concurrent;

namespace SnippetAdmin.Data
{
    public static class DbContextInitializer
    {
        public static ConcurrentDictionary<Type, bool> CacheAbleDic = new ConcurrentDictionary<Type, bool>();

        private static Action<SnippetAdminDbContext, UserManager<SnippetAdminUser>,
            RoleManager<SnippetAdminRole>, ILogger<SnippetAdminDbContext>>
            _initialSnippetAdminDbContext = (dbContext, userManager, roleManager, logger) =>
            {
                logger.LogInformation("开始执行数据库初始化操作。");

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

                //dbContext.Database.Migrate();

                //dbContext.Database.EnsureDeleted();

                // 加载用户数据
                if (dbContext.Database.EnsureCreated())
                {
                    logger.LogInformation("数据库创建完毕，开始添加数据。");

                    // 初始化权限
                    logger.LogInformation("初始化权限数据。");
                    InitialElements(dbContext).Wait();
                    logger.LogInformation("初始化权限数据完成。");

                    // 初始化用户角色
                    logger.LogInformation("初始化用户角色数据。");
                    var user = new SnippetAdminUser
                    {
                        UserName = "admin",
                        RealName = "admin",
                        Email = "admin@admin.com",
                        PhoneNumber = "16655558888",
                        IsActive = true
                    };
                    var role = new SnippetAdminRole
                    {
                        Name = "管理员",
                        Code = "Administrator",
                        IsActive = true
                    };
                    userManager.CreateAsync(user, "admin").Wait();
                    roleManager.CreateAsync(role).Wait();
                    userManager.AddToRoleAsync(user, "管理员").Wait();

                    // 赋予管理员权限
                    foreach (var e in dbContext.Elements)
                    {
                        dbContext.RoleClaims.Add(new SnippetAdminRoleClaim
                        {
                            RoleId = 1,
                            ClaimType = ClaimConstant.RoleRight,
                            ClaimValue = e.Id.ToString(),
                        });
                    }
                    logger.LogInformation("初始化用户角色完成。");
                    dbContext.SaveChanges();

                }

                logger.LogInformation("初始化数据操作执行完毕。");
            };

        public static Action<SnippetAdminDbContext, UserManager<SnippetAdminUser>, RoleManager<SnippetAdminRole>, ILogger<SnippetAdminDbContext>> InitialSnippetAdminDbContext
        {
            get => _initialSnippetAdminDbContext;
        }

        public static async Task InitialElements(SnippetAdminDbContext _dbContext)
        {
            // 元素数据
            _dbContext.Elements.Add(new Element { Id = 1, Name = "主页", Identity = "home", Type = ElementType.Menu, AccessApi = "" });
            _dbContext.Elements.Add(new Element { Id = 2, Name = "RBAC管理", Identity = "rbac", Type = ElementType.Menu, AccessApi = "" });
            _dbContext.Elements.Add(new Element { Id = 3, Name = "关于", Identity = "about", Type = ElementType.Menu, AccessApi = "" });
            _dbContext.Elements.Add(new Element { Id = 7, Name = "用户信息", Identity = "user", Type = ElementType.Menu, AccessApi = "" });
            _dbContext.Elements.Add(new Element { Id = 9, Name = "角色信息", Identity = "role", Type = ElementType.Menu, AccessApi = "" });
            _dbContext.Elements.Add(new Element { Id = 10, Name = "组织信息", Identity = "org", Type = ElementType.Menu, AccessApi = "" });
            _dbContext.Elements.Add(new Element { Id = 11, Name = "页面权限", Identity = "permission", Type = ElementType.Menu, AccessApi = "" });
            _dbContext.Elements.Add(new Element { Id = 12, Name = "添加成员", Identity = "add-member", Type = ElementType.ButtonLink, AccessApi = "api/Organization/GetPositionDic,api/User/AddOrgMember" });
            _dbContext.Elements.Add(new Element { Id = 13, Name = "创建用户", Identity = "create-user", Type = ElementType.ButtonLink, AccessApi = "api/User/AddOrUpdateUser" });
            _dbContext.Elements.Add(new Element { Id = 16, Name = "编辑用户", Identity = "edit-user", Type = ElementType.ButtonLink, AccessApi = "api/User/AddOrUpdateUser" });
            _dbContext.Elements.Add(new Element { Id = 17, Name = "删除用户", Identity = "remove-user", Type = ElementType.ButtonLink, AccessApi = "api/User/RemoveUser" });
            _dbContext.Elements.Add(new Element { Id = 18, Name = "移出组织", Identity = "move-out", Type = ElementType.ButtonLink, AccessApi = "api/User/RemoveOrgMember" });
            _dbContext.Elements.Add(new Element { Id = 19, Name = "创建角色", Identity = "create-role", Type = ElementType.ButtonLink, AccessApi = "api/Role/AddOrUpdateRole" });
            _dbContext.Elements.Add(new Element { Id = 20, Name = "编辑角色", Identity = "edit-role", Type = ElementType.ButtonLink, AccessApi = "api/Role/AddOrUpdateRole" });
            _dbContext.Elements.Add(new Element { Id = 21, Name = "删除角色", Identity = "remove-role", Type = ElementType.ButtonLink, AccessApi = "api/Role/RemoveRole" });
            _dbContext.Elements.Add(new Element { Id = 22, Name = "创建组织", Identity = "create-org", Type = ElementType.ButtonLink, AccessApi = "api/Organization/CreateOrganization" });
            _dbContext.Elements.Add(new Element { Id = 23, Name = "编辑组织", Identity = "edit-org", Type = ElementType.ButtonLink, AccessApi = "api/Organization/UpdateOrganization" });
            _dbContext.Elements.Add(new Element { Id = 24, Name = "删除组织", Identity = "remove-org", Type = ElementType.ButtonLink, AccessApi = "api/Organization/DeleteOrganization" });
            _dbContext.Elements.Add(new Element { Id = 25, Name = "职位设置", Identity = "set-pos", Type = ElementType.ButtonLink, AccessApi = "api/Organization/SetPosition" });
            _dbContext.Elements.Add(new Element { Id = 26, Name = "添加页面元素", Identity = "add-element", Type = ElementType.ButtonLink, AccessApi = "api/Element/CreateElement" });
            _dbContext.Elements.Add(new Element { Id = 27, Name = "导出", Identity = "export-element", Type = ElementType.ButtonLink, AccessApi = "api/Element/ExportElementData" });
            _dbContext.Elements.Add(new Element { Id = 28, Name = "编辑", Identity = "edit-element", Type = ElementType.ButtonLink, AccessApi = "api/Element/UpdateElement" });
            _dbContext.Elements.Add(new Element { Id = 29, Name = "删除元素", Identity = "remove-element", Type = ElementType.ButtonLink, AccessApi = "api/Element/DeleteElement" });
            _dbContext.Elements.Add(new Element { Id = 30, Name = "设定密码", Identity = "set-password", Type = ElementType.ButtonLink, AccessApi = "api/User/SetUserPassword" });
            _dbContext.Elements.Add(new Element { Id = 31, Name = "用户激活", Identity = "active-user", Type = ElementType.ButtonLink, AccessApi = "api/User/ActiveUser" });
            _dbContext.Elements.Add(new Element { Id = 32, Name = "页面", Identity = "user-page", Type = ElementType.ButtonLink, AccessApi = "api/Organization/GetOrganizationTree,api/Role/GetRoleDic,api/User/SearchUser,api/User/GetUser" });
            _dbContext.Elements.Add(new Element { Id = 33, Name = "页面", Identity = "role-page", Type = ElementType.ButtonLink, AccessApi = "api/Role/GetRoles,api/Element/GetElementTree,api/Role/GetRole" });
            _dbContext.Elements.Add(new Element { Id = 35, Name = "页面", Identity = "org-page", Type = ElementType.ButtonLink, AccessApi = "api/Organization/GetOrganization,api/Organization/GetOrganizationTree,api/Organization/GetOrganizationTypes" });
            _dbContext.Elements.Add(new Element { Id = 36, Name = "页面", Identity = "right-page", Type = ElementType.ButtonLink, AccessApi = "api/ApiInfo/GetApiPaths,api/Element/GetElement,api/Element/GetElementTree" });
            _dbContext.Elements.Add(new Element { Id = 37, Name = "激活角色", Identity = "active-role", Type = ElementType.ButtonLink, AccessApi = "api/Role/ActiveRole" });
            _dbContext.Elements.Add(new Element { Id = 38, Name = "创建或编辑组织类型", Identity = "add-update-org-type", Type = ElementType.ButtonLink, AccessApi = "api/Organization/AddOrUpdateOrganizationType" });
            _dbContext.Elements.Add(new Element { Id = 39, Name = "删除组织类型", Identity = "remove-org-type", Type = ElementType.ButtonLink, AccessApi = "api/Organization/RemoveOrganizationType" });

            // 元素树数据
            _dbContext.ElementTrees.Add(new ElementTree { Id = 1, Ancestor = 1, Descendant = 1, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 2, Ancestor = 2, Descendant = 2, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 3, Ancestor = 3, Descendant = 3, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 4, Ancestor = 2, Descendant = 4, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 5, Ancestor = 4, Descendant = 4, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 6, Ancestor = 2, Descendant = 5, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 7, Ancestor = 5, Descendant = 5, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 8, Ancestor = 6, Descendant = 6, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 9, Ancestor = 2, Descendant = 7, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 10, Ancestor = 7, Descendant = 7, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 11, Ancestor = 2, Descendant = 8, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 12, Ancestor = 8, Descendant = 8, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 13, Ancestor = 2, Descendant = 9, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 14, Ancestor = 9, Descendant = 9, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 15, Ancestor = 2, Descendant = 10, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 16, Ancestor = 10, Descendant = 10, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 17, Ancestor = 2, Descendant = 11, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 18, Ancestor = 11, Descendant = 11, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 19, Ancestor = 2, Descendant = 12, Length = 2 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 20, Ancestor = 7, Descendant = 12, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 21, Ancestor = 12, Descendant = 12, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 22, Ancestor = 2, Descendant = 13, Length = 2 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 23, Ancestor = 7, Descendant = 13, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 24, Ancestor = 13, Descendant = 13, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 25, Ancestor = 14, Descendant = 14, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 26, Ancestor = 15, Descendant = 15, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 27, Ancestor = 2, Descendant = 16, Length = 2 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 28, Ancestor = 7, Descendant = 16, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 29, Ancestor = 16, Descendant = 16, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 30, Ancestor = 2, Descendant = 17, Length = 2 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 31, Ancestor = 7, Descendant = 17, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 32, Ancestor = 17, Descendant = 17, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 33, Ancestor = 2, Descendant = 18, Length = 2 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 34, Ancestor = 7, Descendant = 18, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 35, Ancestor = 18, Descendant = 18, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 36, Ancestor = 2, Descendant = 19, Length = 2 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 37, Ancestor = 9, Descendant = 19, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 38, Ancestor = 19, Descendant = 19, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 39, Ancestor = 2, Descendant = 20, Length = 2 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 40, Ancestor = 9, Descendant = 20, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 41, Ancestor = 20, Descendant = 20, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 42, Ancestor = 2, Descendant = 21, Length = 2 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 43, Ancestor = 9, Descendant = 21, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 44, Ancestor = 21, Descendant = 21, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 45, Ancestor = 2, Descendant = 22, Length = 2 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 46, Ancestor = 10, Descendant = 22, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 47, Ancestor = 22, Descendant = 22, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 48, Ancestor = 2, Descendant = 23, Length = 2 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 49, Ancestor = 10, Descendant = 23, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 50, Ancestor = 23, Descendant = 23, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 51, Ancestor = 2, Descendant = 24, Length = 2 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 52, Ancestor = 10, Descendant = 24, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 53, Ancestor = 24, Descendant = 24, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 54, Ancestor = 2, Descendant = 25, Length = 2 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 55, Ancestor = 10, Descendant = 25, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 56, Ancestor = 25, Descendant = 25, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 57, Ancestor = 2, Descendant = 26, Length = 2 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 58, Ancestor = 11, Descendant = 26, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 59, Ancestor = 26, Descendant = 26, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 60, Ancestor = 2, Descendant = 27, Length = 2 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 61, Ancestor = 11, Descendant = 27, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 62, Ancestor = 27, Descendant = 27, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 63, Ancestor = 2, Descendant = 28, Length = 2 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 64, Ancestor = 11, Descendant = 28, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 65, Ancestor = 28, Descendant = 28, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 66, Ancestor = 2, Descendant = 29, Length = 2 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 67, Ancestor = 11, Descendant = 29, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 68, Ancestor = 29, Descendant = 29, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 69, Ancestor = 2, Descendant = 30, Length = 2 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 70, Ancestor = 7, Descendant = 30, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 71, Ancestor = 30, Descendant = 30, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 72, Ancestor = 2, Descendant = 31, Length = 2 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 73, Ancestor = 7, Descendant = 31, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 74, Ancestor = 31, Descendant = 31, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 75, Ancestor = 2, Descendant = 32, Length = 2 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 76, Ancestor = 7, Descendant = 32, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 77, Ancestor = 32, Descendant = 32, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 78, Ancestor = 2, Descendant = 33, Length = 2 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 79, Ancestor = 9, Descendant = 33, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 80, Ancestor = 33, Descendant = 33, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 81, Ancestor = 34, Descendant = 34, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 82, Ancestor = 2, Descendant = 35, Length = 2 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 83, Ancestor = 10, Descendant = 35, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 84, Ancestor = 35, Descendant = 35, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 85, Ancestor = 2, Descendant = 36, Length = 2 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 86, Ancestor = 11, Descendant = 36, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 87, Ancestor = 36, Descendant = 36, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 88, Ancestor = 2, Descendant = 37, Length = 2 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 89, Ancestor = 9, Descendant = 37, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 90, Ancestor = 37, Descendant = 37, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 91, Ancestor = 2, Descendant = 38, Length = 2 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 92, Ancestor = 10, Descendant = 38, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 93, Ancestor = 38, Descendant = 38, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 94, Ancestor = 2, Descendant = 39, Length = 2 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 95, Ancestor = 10, Descendant = 39, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 96, Ancestor = 39, Descendant = 39, Length = 0 });
            await _dbContext.SaveChangesAsync();
        }
    }
}