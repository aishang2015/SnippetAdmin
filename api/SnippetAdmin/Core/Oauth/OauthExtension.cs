using Refit;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Oauth.Apis;

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
                    c.BaseAddress = new Uri(CommonConstant.GithubUri);
                });

            services.AddRefitClient<IGithubApi>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(CommonConstant.GithubApiUri);
                });

            services.AddRefitClient<IBaiduApi>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(CommonConstant.BaiduUri);
                });

            return services;
        }
    }
}