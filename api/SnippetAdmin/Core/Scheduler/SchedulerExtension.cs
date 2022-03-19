using SnippetAdmin.Core.Utils;

namespace SnippetAdmin.Core.Scheduler
{
    public static class SchedulerExtension
    {

        /// <summary>
        /// 将backgroundservice注册为后端服务，并可以直接通过类型获得注入实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>k
        public static IServiceCollection AddJobScheduler(this IServiceCollection services)
        {
            // 启动定时服务
            services.AddHostedService<JobSchedulerService>();
            services.AddSingleton(serviceProvider =>
            {
                var hostedServices = serviceProvider.GetServices<IHostedService>();
                return hostedServices.First(s => s.GetType() == typeof(JobSchedulerService)) as JobSchedulerService;
            });

            // 查找全局的ijob定义,并将其注入
            var types = ReflectionUtil.GetAssemblyTypes()
                 .Where(t => typeof(IJob).IsAssignableFrom(t) && t.IsClass)
                 .ToList();
            types.ForEach(j => services.AddScoped(j));
            return services;
        }
    }
}
