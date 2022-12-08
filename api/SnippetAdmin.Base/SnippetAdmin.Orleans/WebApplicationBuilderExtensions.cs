using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;

namespace SnippetAdmin.Orleans
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder UseDevelopOrleans(this WebApplicationBuilder hostBuilder,
            params Type[] types)
        {
            // use orleans
            hostBuilder.Host.UseOrleans((ctx, builder) =>
            {
                builder.UseLocalhostClustering();
                builder.AddMemoryGrainStorage("SnippetAdminSilo");
            });

            return hostBuilder;
        }
    }
}
