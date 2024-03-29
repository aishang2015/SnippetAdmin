
using MassTransit;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Quartz.Impl.AdoJobStore.Common;
using SnippetAdmin.Constants;
using SnippetAdmin.Data.Entity.Rbac;
using SnippetAdmin.Data.Entity.Scheduler;
using SnippetAdmin.Data.Entity.System;
using System.Security.AccessControl;
using System.Security.Claims;

namespace SnippetAdmin.Data
{
    public class SnippetAdminDbContext : IdentityDbContext<RbacUser, RbacRole, int,
        RbacUserClaim, RbacUserRole, RbacUserLogin, RbacRoleClaim, RbacUserToken>
    {
        //public string ShardingKey { get; private set; }

        //private List<(Type, string)> _typeNames = new();

        //private readonly IMemoryCache _memoryCache;

        //private readonly IShardingInfoService _shardingInfoService;

        //private DbContextOptions _options;

        private readonly IHttpContextAccessor _contextAccessor;

        public SnippetAdminDbContext(DbContextOptions<SnippetAdminDbContext> options,
            IHttpContextAccessor contextAccessor) : base(options)
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
            //ShardingKey = shardingInfoService.GetShardingInfoKey();
            //_typeNames = shardingInfoService.GetShardingList();

            //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTrackingWithIdentityResolution;

            // 关闭自动检测后，实体的变化需要手动调用Update，Delete等方法去进行检测。
            //ChangeTracker.AutoDetectChangesEnabled = false;

            //_memoryCache = memoryCache;
            //_shardingInfoService = shardingInfoService;
            //_options = options;
            _contextAccessor = contextAccessor;

        }

        public DbSet<RbacElement> RbacElements { get; set; }

        public DbSet<RbacElementTree> RbacElementTrees { get; set; }

        public DbSet<RbacOrganization> RbacOrganizations { get; set; }

        public DbSet<RbacOrganizationTree> RbacOrganizationTrees { get; set; }

        public DbSet<RbacOrganizationType> RbacOrganizationTypes { get; set; }

        public DbSet<RbacPosition> RbacPositions { get; set; }

        public DbSet<SysAccessLog> SysAccessLogs { get; set; }

        public DbSet<SysDataLog> SysDataLogs { get; set; }

        public DbSet<SysDataLogDetail> SysDataLogDetails { get; set; }

        public DbSet<SysExceptionLog> SysExceptionLogs { get; set; }

        public DbSet<SysLoginLog> SysLoginLogs { get; set; }

        public DbSet<Job> Jobs { get; set; }

        public DbSet<JobRecord> JobRecords { get; set; }

        public DbSet<SysDicType> SysDicTypes { get; set; }

        public DbSet<SysDicValue> SysDicValues { get; set; }

        public DbSet<SysSetting> SysSettings { get; set; }

        public DbSet<SysSharding> SysShardings { get; set; }

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

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            if (_contextAccessor.HttpContext == null)
            {
                return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            }

            var user = _contextAccessor.HttpContext.User;
            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                var userId = int.Parse(user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value);
                var entries = ChangeTracker.Entries()
                    .Where(e => e.State is EntityState.Added or EntityState.Deleted or EntityState.Modified)
                    .ToList();

                var transactionId = Database.CurrentTransaction?.TransactionId ?? Guid.NewGuid();

                var designModel = this.GetService<IDesignTimeModel>().Model;
                foreach (var entry in entries)
                {
                    var logId = Guid.NewGuid();
                    var auditLog = new SysDataLog()
                    {
                        Id = logId,
                        TraceIdentifier = _contextAccessor.HttpContext.TraceIdentifier,
                        TransactionId = transactionId,
                        OperateTime = DateTime.UtcNow,
                        EntityName = entry.Metadata.Name,
                        Operation = (int)entry.State,
                        UserId = userId
                    };
                    SysDataLogs.Add(auditLog);

                    var properties = designModel.FindEntityType(entry.Entity.GetType()).GetProperties().Where(p => !string.IsNullOrEmpty(p.GetComment()));
                    var entryProperties = entry.Properties.Where(ep => properties.Any(p => p.Name == ep.Metadata.Name));

                    if (entry.State is EntityState.Modified)
                    {
                        //var attachValues = Entry(entry.Entity).GetDatabaseValues();
                        foreach (var property in entryProperties)
                        {
                            var auditLogDetail = new SysDataLogDetail()
                            {
                                DataLogId = logId,
                                EntityName = entry.Metadata.Name,
                                PropertyName = property.Metadata.Name,
                                NewValue = property.CurrentValue?.ToString(),
                                OldValue = property.OriginalValue?.ToString()
                            };
                            SysDataLogDetails.Add(auditLogDetail);
                        }
                    }
                    else if (entry.State is EntityState.Deleted)
                    {
                        foreach (var p in entryProperties)
                        {
                            var auditLogDetail = new SysDataLogDetail()
                            {
                                DataLogId = logId,
                                EntityName = entry.Metadata.Name,
                                PropertyName = p.Metadata.Name,
                                OldValue = p.OriginalValue?.ToString()
                            };
                            SysDataLogDetails.Add(auditLogDetail);
                        }
                    }
                    else if (entry.State is EntityState.Added)
                    {
                        foreach (var p in entryProperties)
                        {
                            var auditLogDetail = new SysDataLogDetail()
                            {
                                DataLogId = logId,
                                EntityName = entry.Metadata.Name,
                                PropertyName = p.Metadata.Name,
                                NewValue = p.OriginalValue?.ToString()
                            };
                            SysDataLogDetails.Add(auditLogDetail);
                        }
                    }
                }
            }
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }


    }
}