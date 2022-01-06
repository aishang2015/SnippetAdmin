namespace SnippetAdmin.Business.Hubs
{
    public static class IEndpointRouteBuilderExtension
    {
        public static IEndpointRouteBuilder MapHubs(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHub<BroadcastHub>("/broadcast");
            return endpoints;
        }
    }
}
