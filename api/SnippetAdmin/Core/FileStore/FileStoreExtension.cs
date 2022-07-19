using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace SnippetAdmin.Core.FileStore
{
    public static class FileStoreExtension
    {
        public static IServiceCollection AddFileStorage(
            this IServiceCollection services,
            IConfiguration configuration,
            string sectionName = "FileStoreOption")
        {
            var config = configuration.GetSection(sectionName);
            services.Configure<FileStoreOption>(config);
            var option = config.Get<FileStoreOption>();
            ConfigureService(services, option);
            return services;
        }

        public static IServiceCollection AddFileStorage(
            this IServiceCollection services,
            Action<FileStoreOption> action)
        {
            services.Configure(action);

            var config = new FileStoreOption();
            action(config);
            ConfigureService(services, config);
            return services;
        }

        private static void ConfigureService(IServiceCollection services, FileStoreOption option)
        {
            long maxBodySize = option.MaxSize * 1024 * 1024;

            // kestrel 
            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = maxBodySize;
            });

            // iis
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = maxBodySize;
            });

            services.AddScoped<IFileStoreService, LocalFileStoreService>();
        }
    }
}
