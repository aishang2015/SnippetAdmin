using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SnippetAdmin.Data.Entity.Enums;
using SnippetAdmin.Data.Entity.RBAC;
using System;
using System.Threading.Tasks;

namespace SnippetAdmin.Data
{
    public static class DbContextInitializer
    {
        public static Action<SnippetAdminDbContext, UserManager<SnippetAdminUser>, ILogger<SnippetAdminDbContext>>
            InitialSnippetAdminDbContext = (dbContext, userManager, logger) =>
            {
                logger.LogInformation("开始执行初始化数据操作。");
                dbContext.Database.EnsureDeleted();

                // 加载用户数据
                if (dbContext.Database.EnsureCreated())
                {
                    logger.LogInformation("数据库初始化完毕，开始添加数据。");
                    userManager.CreateAsync(new SnippetAdminUser
                    {
                        UserName = "admin",
                        Email = "admin@tttttttttt.com.cn",
                        PhoneNumber = "16655558888",
                    }, "admin").Wait();
                    logger.LogInformation("数据创建完毕。");
                }

                // 加载菜单数据
                InitialElements(dbContext).Wait();

                logger.LogInformation("初始化数据操作执行完毕。");
            };

        public static void InitialDatabase<TDbContext>(TDbContext dbContext,
            ILogger<TDbContext> logger) where TDbContext : DbContext
        {
            logger.LogInformation("开始执行初始化数据操作。");
            dbContext.Database.EnsureCreated();
            logger.LogInformation("初始化数据操作执行完毕。");
        }

        public static async Task InitialElements(SnippetAdminDbContext _dbContext)
        {
            await _dbContext.Database.ExecuteSqlRawAsync("truncate table t_rbac_element");
            _dbContext.Elements.Add(new Element { Id = 1, Name = "主页", Identity = "home", Type = ElementType.Menu, AccessApi = "" });
            _dbContext.Elements.Add(new Element { Id = 2, Name = "RBAC管理", Identity = "rbac", Type = ElementType.Menu, AccessApi = "" });
            _dbContext.Elements.Add(new Element { Id = 3, Name = "关于", Identity = "about", Type = ElementType.Menu, AccessApi = "" });
            _dbContext.Elements.Add(new Element { Id = 4, Name = "页面管理", Identity = "page", Type = ElementType.Menu, AccessApi = "" });
            _dbContext.Elements.Add(new Element { Id = 5, Name = "用户管理", Identity = "user", Type = ElementType.Menu, AccessApi = "" });
            await _dbContext.Database.ExecuteSqlRawAsync("truncate table t_rbac_elementtree");
            _dbContext.ElementTrees.Add(new ElementTree { Id = 1, Ancestor = 1, Descendant = 1, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 2, Ancestor = 2, Descendant = 2, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 3, Ancestor = 3, Descendant = 3, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 4, Ancestor = 2, Descendant = 4, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 5, Ancestor = 4, Descendant = 4, Length = 0 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 6, Ancestor = 2, Descendant = 5, Length = 1 });
            _dbContext.ElementTrees.Add(new ElementTree { Id = 7, Ancestor = 5, Descendant = 5, Length = 0 });
            await _dbContext.SaveChangesAsync();
        }
    }
}