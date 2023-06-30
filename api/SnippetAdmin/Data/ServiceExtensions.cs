using Hawthorn.EntityFramework.Sharding;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SnippetAdmin.Data.Entity.Rbac;
using SnippetAdmin.Data.Sharding;
using SnippetAdmin.EntityFrameworkCore;
using SnippetAdmin.EntityFrameworkCore.Cache;

namespace SnippetAdmin.Data
{
	public static class ServiceExtensions
	{
		public static IServiceCollection AddSnippetAdminDbContext(
			this IServiceCollection services, IConfiguration configuration,
			string optionKey = "DatabaseOption", Action<IdentityOptions> setupAction = null)
		{
			var databaseOption = configuration.GetSection(optionKey).Get<DatabaseOption>();
			if (databaseOption != null)
			{
				// 添加缓存拦截器
				services.AddMemoryCache();
				//services.AddScoped<SaveChangeInterceptor<SnippetAdminDbContext>>();
				//services.AddSingleton<IShardingInfoService, ShardingInfoService>();

				if (setupAction == null)
				{
					setupAction = option =>
					{
						// 密码强度设置
						option.Password.RequireDigit = false;
						option.Password.RequireLowercase = false;
						option.Password.RequireUppercase = false;
						option.Password.RequireNonAlphanumeric = false;
						option.Password.RequiredLength = 4;
					};
				}

				services.AddDbContext<SnippetAdminDbContext>((provider, option) =>
				{
					option.UseShardingDatabase(databaseOption);
					//option.AddInterceptors(provider.GetRequiredService<SaveChangeInterceptor<SnippetAdminDbContext>>());

				}).AddIdentity<RbacUser, RbacRole>(setupAction)
				.AddEntityFrameworkStores<SnippetAdminDbContext>()
				.AddDefaultTokenProviders();

				return services;
			}
			throw new NoDatabaseOptionException();
		}

		private static DbContextOptionsBuilder UseShardingDatabase(this DbContextOptionsBuilder option,
			DatabaseOption databaseOption)
		{
			option = databaseOption.Type switch
			{
				"InMemory" => option.UseInMemoryDatabase(databaseOption.Connection),

				"SQLite" => option.UseSqlite(databaseOption.Connection, builder =>
				{
					builder.UseRelationalNulls();
				}),

				"SQLServer" => option.UseSqlServer(databaseOption.Connection, builder =>
				{
					builder.UseRelationalNulls();
				}),

				"MySQL" => option.UseMySql(databaseOption.Connection, ServerVersion.AutoDetect(databaseOption.Connection), builder =>
				{
					builder.UseRelationalNulls();
				}),

				"PostgreSQL" => option.UseNpgsql(databaseOption.Connection, builder =>
				{
					builder.UseRelationalNulls();
				}),

				//// oracle版本11或12
				//"Oracle" => option.UseOracle(databaseOption.Connection, builder =>
				//{
				//    builder.UseOracleSQLCompatibility(databaseOption.Version);
				//    builder.UseRelationalNulls();
				//}),
				_ => option
			};
			//option.ReplaceService<IModelCacheKeyFactory, ShardingCacheFactory>();
			return option;
		}
	}
}