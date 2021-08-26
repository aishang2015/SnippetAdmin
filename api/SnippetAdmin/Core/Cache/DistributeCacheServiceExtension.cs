using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SnippetAdmin.Core.Cache
{
    public static class DistributeCacheServiceExtension
    {
        public static IServiceCollection AddDistributeCache(this IServiceCollection services,
                IConfiguration configuration, string optionKey = "DistributeCacheOption")
        {
            var cacheOption = configuration.GetSection(optionKey).Get<DistributeCacheOption>();

            switch (cacheOption.Type)
            {
                case "Memory":
                    services.AddDistributedMemoryCache();
                    break;

                case "SQLServer":
                    services.AddDistributedSqlServerCache(options =>
                    {
                        options.ConnectionString = cacheOption.Connection;
                        options.TableName = cacheOption.Store;
                    });
                    break;

                case "Redis":
                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = cacheOption.Connection;
                        options.InstanceName = cacheOption.Store;
                    });
                    break;

                default:
                    services.AddDistributedMemoryCache();
                    break;
            }

            return services;
        }
    }
}