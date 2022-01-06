using SnippetAdmin.Business.BackgroundServices;
using SnippetAdmin.Core.HostedService;

namespace SnippetAdmin.Business.Jobs
{
    public static class IServiceCollectionExtension
    {
        /// <summary>
        /// 将backgroundservice注册为后端服务，并可以直接通过类型获得注入实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddJobScheduler(this IServiceCollection services)
        {
            // 启动定时服务
            services.AddBackgroundService<JobSchedulerService>();

            // 将所有定时任务注入到
            services.AddScoped<TestJob>();
            return services;
        }
    }
}
