using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using SnippetAdmin.Core.Oauth.Apis;
using System;

namespace SnippetAdmin.Core.Oauth
{
    public static class OauthExtension
    {
        public static IServiceCollection AddOauth(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OauthOption>(configuration.GetSection("OauthOption"));
            services.AddScoped<OauthHelper>();

            services.AddRefitClient<IGithubAuthApi>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri("https://github.com");
                });

            services.AddRefitClient<IGithubApi>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri("https://api.github.com");
                });

            services.AddRefitClient<IBaiduApi>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri("https://openapi.baidu.com");
                });

            return services;
        }
    }
}