using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Data.Entity;

namespace SnippetAdmin.Data
{
    public class SnippetAdminDbContext : IdentityDbContext<SnippetAdminUser, SnippetAdminRole, int>
    {
        public SnippetAdminDbContext(DbContextOptions<SnippetAdminDbContext> options) : base(options)
        {
            // 更改默认不跟踪所有实体
            // ef core 5推荐 NoTracking在多次相同查询时会返回不同的对象，NoTrackingWithIdentityResolution则会返回
            // 相同的对象
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTrackingWithIdentityResolution;

            // 关闭自动检测后，实体的变化需要手动调用Update，Delete等方法去进行检测。
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // 打印sql参数
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}