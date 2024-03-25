using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SnippetAdmin.EntityFrameworkCore;
using SnippetAdmin.EntityFrameworkCore.Cache;
using SnippetAdmin.EntityFrameworkCore.Sharding;

namespace SnippetAdmin.Data
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDatabase<TIdentityDbContext, TIdentityUser, TIdentityRole>(
            this IServiceCollection services, IConfiguration configuration,
            string optionKey = "DatabaseOption", Action<IdentityOptions> setupAction = null)
            where TIdentityDbContext : DbContext
            where TIdentityUser : class
            where TIdentityRole : class
        {
            var databaseOption = configuration.GetSection(optionKey).Get<DatabaseOption>();
            if (databaseOption != null)
            {
                // 添加缓存拦截器
                services.AddMemoryCache();
                services.AddScoped<SaveChangeInterceptor<TIdentityDbContext>>();

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

                services.AddDbContext<TIdentityDbContext>((provider, option) =>
                {
                    option.UseDatabase(databaseOption);
                    option.AddInterceptors(provider.GetRequiredService<SaveChangeInterceptor<TIdentityDbContext>>());

                }).AddIdentity<TIdentityUser, TIdentityRole>(setupAction)
                .AddEntityFrameworkStores<TIdentityDbContext>()
                .AddDefaultTokenProviders();

                return services;
            }
            throw new NoDatabaseOptionException();
        }

        public static IServiceCollection AddDatabase<TDbContext>(this IServiceCollection services,
            IConfiguration configuration, string optionKey = "DatabaseOption") where TDbContext : DbContext
        {
            var databaseOption = configuration.GetSection(optionKey).Get<DatabaseOption>();
            if (databaseOption != null)
            {
                // 添加缓存拦截器
                services.AddMemoryCache();
                services.AddScoped<SaveChangeInterceptor<TDbContext>>();

                services.AddDbContext<TDbContext>((provider, option) =>
                {
                    option.UseDatabase(databaseOption);
                    option.AddInterceptors(provider.GetRequiredService<SaveChangeInterceptor<TDbContext>>());
                });

                return services;
            }
            throw new NoDatabaseOptionException();
        }

        public static IServiceCollection AddShardingDatabase<TDbContext>(this IServiceCollection services,
            IConfiguration configuration, string optionKey = "DatabaseOption")
            where TDbContext : ShardingDbContext
        {
            var databaseOption = configuration.GetSection(optionKey).Get<DatabaseOption>();
            if (databaseOption != null)
            {
                // 添加缓存拦截器
                services.AddMemoryCache();
                services.AddScoped<SaveChangeInterceptor<TDbContext>>();

                services.AddDbContext<TDbContext>((provider, option) =>
                {
                    option.UseShardingDatabase(databaseOption);
                    option.AddInterceptors(provider.GetRequiredService<SaveChangeInterceptor<TDbContext>>());
                });

                return services;
            }
            throw new NoDatabaseOptionException();
        }


        private static DbContextOptionsBuilder UseDatabase(this DbContextOptionsBuilder option, DatabaseOption databaseOption)
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

                // oracle版本
                "Oracle" => option.UseOracle(databaseOption.Connection, builder =>
                {
                    var vesionCompatibility = databaseOption.Version switch
                    {
                        "19" => OracleSQLCompatibility.DatabaseVersion19,
                        "21" => OracleSQLCompatibility.DatabaseVersion21,
                        "23" => OracleSQLCompatibility.DatabaseVersion23,
                        _ => OracleSQLCompatibility.DatabaseVersion19
                    };
                    builder.UseOracleSQLCompatibility(vesionCompatibility);
                    builder.UseRelationalNulls();
                }),
                _ => option
            };
            return option;
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
            option.ReplaceService<IModelCacheKeyFactory, ShardingCacheFactory>();
            return option;
        }
    }
}