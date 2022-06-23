using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SnippetAdmin.Data.Entity.Rbac;
using SnippetAdmin.Data.Entity.Scheduler;
using SnippetAdmin.Data.Entity.System;

namespace SnippetAdmin.Data
{
    public class SnippetAdminDbContext : IdentityDbContext<RbacUser, RbacRole, int,
        RbacUserClaim, RbacUserRole, RbacUserLogin, RbacRoleClaim, RbacUserToken>
    {
        private readonly IMemoryCache _memoryCache;

        public SnippetAdminDbContext(DbContextOptions<SnippetAdminDbContext> options,
            IMemoryCache memoryCache) : base(options)
        {
            // 执行迁移命令之前需要暂时注释掉Program.cs中的mvcBuilder.AddDynamicController();
            // 迁移命令,生成一个【FirstMigration】的迁移
            // Add-Migration FirstMigration -Context SnippetAdminDbContext -OutputDir Data/Migrations/MySqlMigrations
            // 应用迁移
            // Update-Database
            // 删除迁移
            // Remove-Migration
            // 列出迁移
            // Get-Migration
            // 生成脚本，生成一个AddElementSortingMigration到RemoveElementSortingMigration变化的脚本
            // 如果不加from或to则生成一个初始到最后迁移的脚本
            // Script-Migration AddElementSortingMigration RemoveElementSortingMigration 

            // 更改默认不跟踪所有实体
            // ef core 5推荐 NoTracking在多次相同查询时会返回不同的对象，NoTrackingWithIdentityResolution则会返回
            // 相同的对象
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTrackingWithIdentityResolution;

            // 关闭自动检测后，实体的变化需要手动调用Update，Delete等方法去进行检测。
            ChangeTracker.AutoDetectChangesEnabled = false;

            _memoryCache = memoryCache;
        }

        public DbSet<RbacElement> RbacElements { get; set; }

        public DbSet<RbacElementTree> RbacElementTrees { get; set; }

        public DbSet<RbacOrganization> RbacOrganizations { get; set; }

        public DbSet<RbacOrganizationTree> RbacOrganizationTrees { get; set; }

        public DbSet<RbacOrganizationType> RbacOrganizationTypes { get; set; }

        public DbSet<RbacPosition> RbacPositions { get; set; }

        public DbSet<SysRefreshToken> SysRefreshTokens { get; set; }

        public DbSet<SysApiAccessLog> SysApiAccessLogs { get; set; }

        public DbSet<SysExceptionLog> SysExceptionLogs { get; set; }

        public DbSet<Job> Jobs { get; set; }

        public DbSet<JobRecord> JobRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RbacUser>().ToTable("T_RBAC_User");
            builder.Entity<RbacRole>().ToTable("T_RBAC_Role");
            builder.Entity<RbacUserRole>().ToTable("T_RBAC_UserRole");
            builder.Entity<RbacUserClaim>().ToTable("T_RBAC_UserClaim");
            builder.Entity<RbacRoleClaim>().ToTable("T_RBAC_RoleClaim");
            builder.Entity<RbacUserLogin>().ToTable("T_RBAC_UserLogin");
            builder.Entity<RbacUserToken>().ToTable("T_RBAC_UserToken");

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // 打印sql参数
            optionsBuilder.EnableSensitiveDataLogging();
        }

        public IEnumerable<T> CacheSet<T>() where T : class
        {
            if (DbContextInitializer.CacheAbleDic[typeof(T)])
            {
                return _memoryCache.Get(typeof(T).FullName) as List<T>;
            }
            else
            {
                return Set<T>().AsQueryable();
            }
        }
    }
}