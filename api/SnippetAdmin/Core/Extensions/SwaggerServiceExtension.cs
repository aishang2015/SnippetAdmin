using Google.Protobuf.WellKnownTypes;
using Microsoft.OpenApi.Models;

namespace SnippetAdmin.Core.Extensions
{
    public static class SwaggerServiceExtension
    {
        public static IServiceCollection AddCustomSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SnippetAdmin", Version = "v1" });
                c.SwaggerDoc("DynamicApi", new OpenApiInfo { Title = "Roslyn动态接口", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "在下框中输入请求头中需要添加Jwt授权Token：Bearer Token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

				var filePath = Path.Combine(AppContext.BaseDirectory, "SnippetAdmin.xml");
				c.IncludeXmlComments(filePath);
			});
            return services;
        }
    }
}