using Hawthorn.EntityFramework.Sharding;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.InMemory.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Sqlite.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Memory;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using SnippetAdmin.Data.Entity.Rbac;
using SnippetAdmin.Data.Entity.Scheduler;
using SnippetAdmin.Data.Entity.System;
using SnippetAdmin.EntityFrameworkCore.Cache;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace SnippetAdmin.Data
{
	public class SnippetAdminDbContext : IdentityDbContext<RbacUser, RbacRole, int,
		RbacUserClaim, RbacUserRole, RbacUserLogin, RbacRoleClaim, RbacUserToken>
	{
		public string ShardingKey { get; private set; }

		private List<(Type, string)> _typeNames = new();

		private readonly IMemoryCache _memoryCache;

		private readonly IShardingInfoService _shardingInfoService;

		private DbContextOptions _options;

		public SnippetAdminDbContext(DbContextOptions<SnippetAdminDbContext> options,
			IShardingInfoService shardingInfoService,
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
			ShardingKey = shardingInfoService.GetShardingInfoKey();
			_typeNames = shardingInfoService.GetShardingList();

			ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTrackingWithIdentityResolution;

			// 关闭自动检测后，实体的变化需要手动调用Update，Delete等方法去进行检测。
			ChangeTracker.AutoDetectChangesEnabled = false;

			_memoryCache = memoryCache;
			_shardingInfoService = shardingInfoService;
			_options = options;

		}

		public DbSet<RbacElement> RbacElements { get; set; }

		public DbSet<RbacElementTree> RbacElementTrees { get; set; }

		public DbSet<RbacOrganization> RbacOrganizations { get; set; }

		public DbSet<RbacOrganizationTree> RbacOrganizationTrees { get; set; }

		public DbSet<RbacOrganizationType> RbacOrganizationTypes { get; set; }

		public DbSet<RbacPosition> RbacPositions { get; set; }

		public DbSet<SysExceptionLog> SysExceptionLogs { get; set; }

		public DbSet<SysLoginLog> SysLoginLogs { get; set; }

		public DbSet<Job> Jobs { get; set; }

		public DbSet<JobRecord> JobRecords { get; set; }

		public DbSet<SysDicType> SysDicTypes { get; set; }

		public DbSet<SysDicValue> SysDicValues { get; set; }

		public DbSet<SysSetting> SysSettings { get; set; }

		public DbSet<SysSettingGroup> SysSettingGroups { get; set; }

		public DbSet<SysSettingSubGroup> SysSettingSubGroups { get; set; }

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

			if (_typeNames.Count > 0)
			{
				foreach (var typeName in _typeNames)
				{
					builder.SharedTypeEntity(typeName.Item2, typeName.Item1).ToTable(typeName.Item2);
				}
			}

		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			// 打印sql参数
			optionsBuilder.EnableSensitiveDataLogging();
		}

		public IEnumerable<T> CacheSet<T>() where T : class
		{
			if (CacheableBase<SnippetAdminDbContext>.Instance.CacheableTypeList.Contains(typeof(T)))
			{
				return _memoryCache.Get(typeof(T).FullName) as List<T>;
			}
			else
			{
				return Set<T>().AsQueryable();
			}
		}

		// todo 需要优化
		/// <summary>
		/// 检查分表
		/// </summary>
		/// <remark>
		/// 如果未实行分表则进行创建分表,这里AddShardingInfo会把分表数据保存，但是只有等到
		/// 下次OnModelCreating时才会去SharedTypeEntity配置分表信息，因此此操作需要在一个
		/// 独立的scope中去进行,这里把分表信息保存会执行一次SaveChangesAsync，
		/// </remark>
		public async Task CheckSharingTableWithCreate<T>(string keyword) where T : class
		{
			string tableName = GetShardingTableName<T>(keyword);
			if (!CacheSet<SysSharding>().Any(s => s.TableName == tableName))
			{
				await CreateTable(tableName, typeof(T));
			}
		}

		public bool CheckSharingTableWithNoCreate<T>(string keyword) where T : class
		{
			string tableName = GetShardingTableName<T>(keyword);
			return CacheSet<SysSharding>().Any(s => s.TableName == tableName);
		}

		// 获取访问记录分表
		public DbSet<T> GetShardingTableSet<T>(string keyword) where T : class
		{
			string tableName = GetShardingTableName<T>(keyword);
			if (!CacheSet<SysSharding>().Any(s => s.TableName == tableName))
			{
				return null;
			}
			return Set<T>(tableName);
		}

		/// <summary>
		/// 获取分表的表名
		/// </summary>
		/// <remarks>
		/// 如果没有table标记，则用类型名和关键字拼接作为表明，如果有table标记则用table标记和关键字拼接作为表名
		/// </remarks>
		private static string GetShardingTableName<T>(string keyword) where T : class
		{
			var attribute = typeof(T).GetCustomAttribute<TableAttribute>();
			var tableName = attribute == null ? typeof(T).Name + keyword :
				attribute.Name + "_" + keyword;
			return tableName;
		}

		public async Task CreateTable(string tableName, Type type)
		{
			using var context = new TableDbContext(GetOption(), tableName, type);
			var creator = context.GetService<IRelationalDatabaseCreator>();
			try
			{
				await creator.CreateTablesAsync();
				_shardingInfoService.AddShardingInfo((type, tableName));

				var newShardingInfo = new SysSharding { TableName = tableName, TableType = type.Name };
				SysShardings.Add(newShardingInfo);
				(CacheSet<SysSharding>() as List<SysSharding>).Add(newShardingInfo);
				await SaveChangesAsync();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		private DbContextOptions<TableDbContext> GetOption()
		{
			var builder = new DbContextOptionsBuilder<TableDbContext>()
				.EnableServiceProviderCaching(false);

#pragma warning disable EF1001 // Internal EF Core API usage.
			var inMemoryOptions = _options.FindExtension<InMemoryOptionsExtension>();
			if (inMemoryOptions != null)
			{
				return builder.UseInMemoryDatabase(inMemoryOptions.StoreName)
					.Options;
			}

			var sqliteOptions = _options.FindExtension<SqliteOptionsExtension>();
			if (sqliteOptions != null)
			{
				return builder.UseSqlite(sqliteOptions.ConnectionString)
					.Options;
			}

			var sqlServerOptions = _options.FindExtension<SqlServerOptionsExtension>();
			if (sqlServerOptions != null)
			{
				return builder.UseSqlServer(sqlServerOptions.ConnectionString)
					.Options;
			}

			var mysqlOptions = _options.FindExtension<MySqlOptionsExtension>();
			if (mysqlOptions != null)
			{
				return builder.UseMySql(mysqlOptions.ConnectionString,
					ServerVersion.AutoDetect(mysqlOptions.ConnectionString))
					.Options;
			}

			var npgsqlOptions = _options.FindExtension<NpgsqlOptionsExtension>();
			if (inMemoryOptions != null)
			{
				return builder.UseNpgsql(npgsqlOptions.ConnectionString)
					.Options;
			}

			//var oracleOptions = _options.GetExtension<OracleOptionsExtension>();
			//if (oracleOptions != null)
			//{
			//	// todo
			//	return builder.UseOracle(oracleOptions.ConnectionString)
			//		.Options;
			//}

			return null;
#pragma warning restore EF1001 // Internal EF Core API usage.
		}

		public override void Dispose()
		{
			CacheableExtension.CacheTrackerDataToMemory<SnippetAdminDbContext>(_memoryCache, ContextId.InstanceId);
			base.Dispose();
		}

		public override async ValueTask DisposeAsync()
		{
			CacheableExtension.CacheTrackerDataToMemory<SnippetAdminDbContext>(_memoryCache, ContextId.InstanceId);
			await base.DisposeAsync();
		}
	}
}