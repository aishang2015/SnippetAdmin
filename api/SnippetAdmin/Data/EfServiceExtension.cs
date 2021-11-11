using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Data.Cache;
using SnippetAdmin.Data.Entity.RBAC;

namespace SnippetAdmin.Data
{
    public static class EfServiceExtension
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services,
            IConfiguration configuration, string optionKey = "DatabaseOption")
        {
            var databaseOption = configuration.GetSection(optionKey).Get<DatabaseOption>();
            if (databaseOption != null)
            {
                services.AddScoped<MemoryCacheInterceptor>();

                services.AddDbContext<SnippetAdminDbContext>((provider, option) =>
                {
                    option = databaseOption.Type switch
                    {
                        "SQLite" => option.UseSqlite(databaseOption.Connection, builder =>
                        {
                            builder.UseRelationalNulls();
                        }),

                        "SQLServer" => option.UseSqlServer(databaseOption.Connection, builder =>
                        {
                            builder.UseRelationalNulls();
                        }),

                        // mysql版本填写具体版本例如8.0.21
                        "MySQL" => option.UseMySql(databaseOption.Connection, new MySqlServerVersion(new Version(databaseOption.Version)), builder =>
                        {
                            builder.UseRelationalNulls();
                        }),

                        "PostgreSQL" => option.UseNpgsql(databaseOption.Connection, builder =>
                        {
                            builder.UseRelationalNulls();
                        }),

                        // oracle版本11或12
                        "Oracle" => option.UseOracle(databaseOption.Connection, builder =>
                        {
                            builder.UseOracleSQLCompatibility(databaseOption.Version);
                            builder.UseRelationalNulls();
                        }),
                        _ => option
                    };

                    option.AddInterceptors(provider.GetRequiredService<MemoryCacheInterceptor>());
                }).AddIdentity<SnippetAdminUser, SnippetAdminRole>(option =>
                {
                    // 密码强度设置
                    option.Password.RequireDigit = false;
                    option.Password.RequireLowercase = false;
                    option.Password.RequireUppercase = false;
                    option.Password.RequireNonAlphanumeric = false;
                    option.Password.RequiredLength = 4;
                })
                .AddEntityFrameworkStores<SnippetAdminDbContext>()
                .AddDefaultTokenProviders();

                return services;
            }
            throw new Exception("没有配置数据库，无法找到数据库配置片段！");
        }

        public static IServiceCollection AddDatabase<TDbContext>(this IServiceCollection services,
            IConfiguration configuration, string optionKey = "DatabaseOption") where TDbContext : DbContext
        {
            var databaseOption = configuration.GetSection(optionKey).Get<DatabaseOption>();
            if (databaseOption != null)
            {
                services.AddDbContext<TDbContext>(option =>
                {
                    option = databaseOption.Type switch
                    {
                        "SQLite" => option.UseSqlite(databaseOption.Connection, builder =>
                        {
                            builder.UseRelationalNulls();
                        }),

                        "SQLServer" => option.UseSqlServer(databaseOption.Connection, builder =>
                        {
                            builder.UseRelationalNulls();
                        }),

                        // mysql版本填写具体版本例如8.0.21
                        "MySQL" => option.UseMySql(databaseOption.Connection, new MySqlServerVersion(new Version(databaseOption.Version)), builder =>
                        {
                            builder.UseRelationalNulls();
                        }),

                        "PostgreSQL" => option.UseNpgsql(databaseOption.Connection, builder =>
                        {
                            builder.UseRelationalNulls();
                        }),

                        // oracle版本11或12
                        "Oracle" => option.UseOracle(databaseOption.Connection, builder =>
                        {
                            builder.UseOracleSQLCompatibility(databaseOption.Version);
                            builder.UseRelationalNulls();
                        }),
                        _ => option
                    };
                });

                return services;
            }
            throw new Exception("没有配置数据库，无法找到数据库配置片段！");
        }
    }
}