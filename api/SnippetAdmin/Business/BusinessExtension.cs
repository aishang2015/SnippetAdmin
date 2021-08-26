using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using SnippetAdmin.Business.Hubs;
using SnippetAdmin.Business.Workers;
using SnippetAdmin.Core.HostedService;

namespace SnippetAdmin.Business
{
    public static class BusinessExtension
    {
        public static IServiceCollection AddWorks(this IServiceCollection services)
        {
            services.AddBackgroundService<BroadcastWorker>();
            return services;
        }

        public static IEndpointRouteBuilder MapHubs(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHub<BroadcastHub>("/broadcast");
            return endpoints;
        }
    }
}