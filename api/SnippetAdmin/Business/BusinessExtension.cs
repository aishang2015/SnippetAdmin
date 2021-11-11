using SnippetAdmin.Business.Hubs;
using SnippetAdmin.Business.Jobs;
using SnippetAdmin.Business.Workers;
using SnippetAdmin.Core.HostedService;

namespace SnippetAdmin.Business
{
    public static class BusinessExtension
    {
        public static IServiceCollection AddBusinesses(this IServiceCollection services)
        {
            services.AddBackgroundService<BroadcastWorker>();

            services.AddScoped<TestJob>();
            return services;
        }

        public static IEndpointRouteBuilder MapHubs(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHub<BroadcastHub>("/broadcast");
            return endpoints;
        }
    }
}