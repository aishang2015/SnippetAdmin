namespace SnippetAdmin.Core
{
    /// <summary>
    /// 用于在程序启动时执行一些操作
    /// </summary>
    public static class InitializerHostExtension
    {
        /// <summary>
        /// 执行初始化操作(1个参数)
        /// </summary>
        public static void Initialize<T>(this IHost host, Action<T> action)
        {
            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                var service = services.GetRequiredService<T>();
                action(service);
            }
        }

        /// <summary>
        /// 执行初始化操作(2个参数)
        /// </summary>
        public static void Initialize<T1, T2>(this IHost host, Action<T1, T2> action)
        {
            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                var service1 = services.GetRequiredService<T1>();
                var service2 = services.GetRequiredService<T2>();
                action(service1, service2);
            }
        }

        /// <summary>
        /// 执行初始化操作(3个参数)
        /// </summary>
        public static void Initialize<T1, T2, T3>(this IHost host, Action<T1, T2, T3> action)
        {
            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                var service1 = services.GetRequiredService<T1>();
                var service2 = services.GetRequiredService<T2>();
                var service3 = services.GetRequiredService<T3>();
                action(service1, service2, service3);
            }
        }

        /// <summary>
        /// 执行初始化操作(4个参数)
        /// </summary>
        public static void Initialize<T1, T2, T3, T4>(this IHost host, Action<T1, T2, T3, T4> action)
        {
            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                var service1 = services.GetRequiredService<T1>();
                var service2 = services.GetRequiredService<T2>();
                var service3 = services.GetRequiredService<T3>();
                var service4 = services.GetRequiredService<T4>();
                action(service1, service2, service3, service4);
            }
        }

        /// <summary>
        /// 执行初始化操作(5个参数)
        /// </summary>
        public static void Initialize<T1, T2, T3, T4, T5>(
            this IHost host,
            Action<T1, T2, T3, T4, T5> action)
        {
            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                var service1 = services.GetRequiredService<T1>();
                var service2 = services.GetRequiredService<T2>();
                var service3 = services.GetRequiredService<T3>();
                var service4 = services.GetRequiredService<T4>();
                var service5 = services.GetRequiredService<T5>();
                action(service1, service2, service3, service4, service5);
            }
        }

        /// <summary>
        /// 执行初始化操作(6个参数)
        /// </summary>
        public static void Initialize<T1, T2, T3, T4, T5, T6>(
            this IHost host,
            Action<T1, T2, T3, T4, T5, T6> action)
        {
            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                var service1 = services.GetRequiredService<T1>();
                var service2 = services.GetRequiredService<T2>();
                var service3 = services.GetRequiredService<T3>();
                var service4 = services.GetRequiredService<T4>();
                var service5 = services.GetRequiredService<T5>();
                var service6 = services.GetRequiredService<T6>();
                action(service1, service2, service3, service4, service5, service6);
            }
        }

        /// <summary>
        /// 执行初始化操作(7个参数)
        /// </summary>
        public static void Initialize<T1, T2, T3, T4, T5, T6, T7>(
            this IHost host,
            Action<T1, T2, T3, T4, T5, T6, T7> action)
        {
            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                var service1 = services.GetRequiredService<T1>();
                var service2 = services.GetRequiredService<T2>();
                var service3 = services.GetRequiredService<T3>();
                var service4 = services.GetRequiredService<T4>();
                var service5 = services.GetRequiredService<T5>();
                var service6 = services.GetRequiredService<T6>();
                var service7 = services.GetRequiredService<T7>();
                action(service1, service2, service3, service4, service5, service6, service7);
            }
        }

        /// <summary>
        /// 执行初始化操作(8个参数)
        /// </summary>
        public static void Initialize<T1, T2, T3, T4, T5, T6, T7, T8>(
            this IHost host,
            Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
        {
            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                var service1 = services.GetRequiredService<T1>();
                var service2 = services.GetRequiredService<T2>();
                var service3 = services.GetRequiredService<T3>();
                var service4 = services.GetRequiredService<T4>();
                var service5 = services.GetRequiredService<T5>();
                var service6 = services.GetRequiredService<T6>();
                var service7 = services.GetRequiredService<T7>();
                var service8 = services.GetRequiredService<T8>();
                action(service1, service2, service3, service4, service5, service6, service7, service8);
            }
        }
    }
}