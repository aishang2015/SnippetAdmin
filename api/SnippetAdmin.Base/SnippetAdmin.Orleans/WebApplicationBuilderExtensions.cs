using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                builder.ConfigureApplicationParts(parts => {
                    types.ToList().ForEach(type =>
                    {
                        parts.AddApplicationPart(type.Assembly).WithReferences();
                    });
                });
            });

            return hostBuilder;
        }
    }
}
