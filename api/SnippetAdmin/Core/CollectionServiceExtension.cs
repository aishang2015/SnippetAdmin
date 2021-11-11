using System.Collections.Concurrent;

namespace SnippetAdmin.Core
{
    /// <summary>
    /// 可以进行全局访问的线程安全集合
    /// </summary>
    public static class CollectionServiceExtension
    {
        public static IServiceCollection AddBlockingCollection<T>(this IServiceCollection services)
        {
            services.AddSingleton<BlockingCollection<T>>();
            return services;
        }

        public static IServiceCollection AddConcurrentDictionary<TKey, TValue>(this IServiceCollection services)
        {
            services.AddSingleton<ConcurrentDictionary<TKey, TValue>>();
            return services;
        }

        public static IServiceCollection AddConcurrentQueue<T>(this IServiceCollection services)
        {
            services.AddSingleton<ConcurrentQueue<T>>();
            return services;
        }

        public static IServiceCollection AddConcurrentStack<T>(this IServiceCollection services)
        {
            services.AddSingleton<ConcurrentStack<T>>();
            return services;
        }

        public static IServiceCollection AddConcurrentBag<T>(this IServiceCollection services)
        {
            services.AddSingleton<ConcurrentBag<T>>();
            return services;
        }
    }
}