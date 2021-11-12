using SnippetAdmin.Business.Hubs;
using SnippetAdmin.Business.Jobs;
using SnippetAdmin.Business.Workers;
using SnippetAdmin.Core.HostedService;
using System.Reactive.Subjects;

namespace SnippetAdmin.Business
{
    public static class BusinessExtension
    {
        public static IServiceCollection AddBusiness(this IServiceCollection services)
        {
            services.AddWorkers();
            services.AddJobs();
            return services;
        }

        public static IServiceCollection AddWorkers(this IServiceCollection services)
        {
            services.AddBackgroundService<BroadcastWorker>();
            return services;
        }

        public static IServiceCollection AddJobs(this IServiceCollection services)
        {
            services.AddScoped<TestJob>();
            return services;
        }

        public static IServiceCollection AddSubject<T>(this IServiceCollection services)
        {
            services.AddSingleton<Subject<T>>();
            return services;
        }

        public static IEndpointRouteBuilder MapHubs(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHub<BroadcastHub>("/broadcast");
            return endpoints;
        }
    }
}