namespace SnippetAdmin.Core.Background
{
    public static class IServiceCollectionExtension
    {
        /// <summary>
        /// 将backgroundservice注册为后端服务，并可以直接通过类型获得注入实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
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
