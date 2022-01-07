using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SnippetAdmin.Data.Cache;
using SnippetAdmin.Data.Entity.RBAC;
using SnippetAdmin.Data.Entity.Scheduler;
using SnippetAdmin.Data.Entity.System;

namespace SnippetAdmin.Data
{
    public class SnippetAdminDbContext : IdentityDbContext<SnippetAdminUser, SnippetAdminRole, int,
        SnippetAdminUserClaim, SnippetAdminUserRole, SnippetAdminUserLogin, SnippetAdminRoleClaim, SnippetAdminUserToken>
    {
        private readonly IMemoryCache _memoryCache;

        public SnippetAdminDbContext(DbContextOptions<SnippetAdminDbContext> options,
            IMemoryCache memoryCache) : base(options)
        {
            // 更改默认不跟踪所有实体
            // ef core 5推荐 NoTracking在多次相同查询时会返回不同的对象，NoTrackingWithIdentityResolution则会返回
            // 相同的对象
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTrackingWithIdentityResolution;

            // 关闭自动检测后，实体的变化需要手动调用Update，Delete等方法去进行检测。
            ChangeTracker.AutoDetectChangesEnabled = false;

            _memoryCache = memoryCache;
        }

        public DbSet<Element> Elements { get; set; }

        public DbSet<ElementTree> ElementTrees { get; set; }

        public DbSet<Organization> Organizations { get; set; }

        public DbSet<OrganizationTree> OrganizationTrees { get; set; }

        public DbSet<Position> Positions { get; set; }

        public DbSet<UserOrganizationPosition> UserOrganizationPositions { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<Job> Jobs { get; set; }

        public DbSet<JobRecord> JobRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // 打印sql参数
            optionsBuilder.EnableSensitiveDataLogging();
        }

        public IEnumerable<T> CacheSet<T>() where T : class
        {
            if (MemoryCacheInitializer.CacheAbleDic[typeof(T)])
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