using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace SnippetAdmin.Core.HostedService
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddSchedulerService(this IServiceCollection services)
        {
            services.AddBackgroundService<JobSchedulerService>();

            return services;
        }

        public static IServiceCollection AddBackgroundService<T>(this IServiceCollection services)
            where T : BackgroundService
        {
            services.AddHostedService<T>();
            services.AddSingleton(serviceProvider =>
            {
                var hostedServices = serviceProvider.GetServices<IHostedService>();
                return hostedServices.First(s => s.GetType() == typeof(T)) as T;
            });
            return services;
        }
    }
}
