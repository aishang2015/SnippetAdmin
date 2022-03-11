namespace SnippetAdmin.Business.Hubs
{
    public static class IEndpointRouteBuilderExtension
    {
        public static IEndpointRouteBuilder MapHubs(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHub<BroadcastHub>("/broadcast");
            endpoints.MapHub<MetricsHub>("/metrics");
            return endpoints;
        }
    }
}
