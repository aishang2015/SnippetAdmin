using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace SnippetAdmin.Quartz
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCustomerQuartz(this IServiceCollection services,
            IConfiguration configuration)
        {
            var section = configuration.GetSection("quartz");
            services.Configure<QuartzOptions>(section);
            services.Configure<QuartzOptions>(options =>
            {
                options.Scheduling.IgnoreDuplicates = true; // default: false
                options.Scheduling.OverWriteExistingData = true; // default: true
            });

            var quartzOptions = section.Get<QuartzOptions>();
            var connectionKey = quartzOptions.AllKeys.FirstOrDefault(k => k.Contains("connectionString"));
            var connectionString = quartzOptions.Get(connectionKey);
            DBInitializer.InitializeMySql(connectionString);

            //services.AddSingleton<QuartzJobRunner>();
            //services.AddSingleton<IJobFactory, ProviderJobFactory>();
            // 这里AddSingleton<ISchedulerFactory，StdSchedulerFactory>会覆盖option，导致无法记录到数据库
            //services.AddSingleton<ISchedulerFactory>(new StdSchedulerFactory(quartzOptions));
            services.AddSingleton<IQuartzService, QuartzService>();

            // quartz现在提供了内置的scope的job工厂，因此不需要quartzjobrunner和ijobfactory了
            services.AddQuartz(x => x.UseMicrosoftDependencyInjectionJobFactory());

            // ASP.NET Core hosting
            services.AddQuartzServer(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            });

            return services;
        }
    }
}
