using Microsoft.Extensions.DependencyInjection;

namespace SnippetAdmin.Core.UserAccessor
{
    public static class UserAccessorServiceExtension
    {
        /// <summary>
        /// 添加用户访问服务
        /// </summary>
        public static IServiceCollection AddUserAccessor(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IUserAccessor, UserAccessor>();
            return services;
        }
    }
}