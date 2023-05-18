using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

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
				builder.UseLocalhostClustering(gatewayPort: 30001);
				builder.AddMemoryGrainStorage("SnippetAdminSilo");
			});

			return hostBuilder;
		}
	}
}
