namespace SnippetAdmin.Core.Monitor
{
    public static class MonitorExtension
    {
        public static IServiceCollection AddMetricEventListener(this IServiceCollection services)
        {
            services.AddSingleton<MetricEventListener>();
            services.BuildServiceProvider().GetService<MetricEventListener>();
            return services;
        }
    }
}
