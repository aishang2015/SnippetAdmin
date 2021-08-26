using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SnippetAdmin.Data.Entity;
using System;

namespace SnippetAdmin.Data
{
    public static class DbContextInitializer
    {
        public static Action<SnippetAdminDbContext, UserManager<SnippetAdminUser>, ILogger<SnippetAdminDbContext>>
            InitialSnippetAdminDbContext = (dbContext, userManager, logger) =>
            {
                logger.LogInformation("开始执行初始化数据操作。");
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
                logger.LogInformation("初始化数据操作执行完毕。");
            };

        public static void InitialDatabase<TDbContext>(TDbContext dbContext,
            ILogger<TDbContext> logger) where TDbContext : DbContext
        {
            logger.LogInformation("开始执行初始化数据操作。");
            dbContext.Database.EnsureCreated();
            logger.LogInformation("初始化数据操作执行完毕。");
        }
    }
}