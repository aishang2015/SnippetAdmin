using SnippetAdmin.Core.Extensions;

namespace SnippetAdmin.Core.Monitor
{
    public static class MonitorExtension
    {
        /// <summary>
        /// inject event listener
        /// </summary>
        public static IServiceCollection AddMetricEventListener(this IServiceCollection services)
        {
            // inject and start the listner
            var listener = new MetricEventListener();
            services.AddSingleton(listener);

            // add a background service to broadcast all metric through the signalr hub
            services.AddBackgroundService<MetricsBackgroundService>();
            return services;
        }

        /// <summary>
        /// map to metrics hub
        /// </summary>
        public static WebApplication MapMetricHub(this WebApplication application)
        {
			application.MapHub<MetricsHub>("/metrics");
            return application;
        }
    }
}
