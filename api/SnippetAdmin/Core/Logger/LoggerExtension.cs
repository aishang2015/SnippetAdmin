using Serilog;
using Serilog.Events;
using Serilog.Filters;

namespace SnippetAdmin.Core.Logger
{
    public static class LoggerExtension
    {
        public static IHostBuilder UseCustomSerilog(this IHostBuilder builder)
        {
            builder.UseSerilog((context, services, configuration) =>
             {
                 // 读取appsetting内的日志配置
                 string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs/all", "log-all-.txt");
                 string errorLogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs/error", "log-error-.txt");
                 string serilogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs/serilog", "log-serilog-.txt");
                 string logFormat = @"{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3} {SourceContext:l}] {Message:lj}{NewLine}{Exception}";
                 configuration.ReadFrom.Configuration(context.Configuration)
                         .ReadFrom.Services(services)
                        //.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                        //.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                        //.MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                        //.MinimumLevel.Override("System", LogEventLevel.Warning)
                        //.MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Warning)
                        //.MinimumLevel.Override("Microsoft.EntityFrameworkCore.Infrastructure", LogEventLevel.Warning)
                        //.MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database", LogEventLevel.Information)
                        .Enrich.FromLogContext()

                        .WriteTo.Logger(config =>
                        {
                            config.WriteTo.File(logPath,
                                restrictedToMinimumLevel: LogEventLevel.Debug,
                                outputTemplate: logFormat,
                                rollingInterval: RollingInterval.Day,
                                rollOnFileSizeLimit: true,
                                shared: true,
                                fileSizeLimitBytes: 10_000_000,
                                retainedFileCountLimit: 30);
                        })
                        .WriteTo.Logger(config =>
                        {
                            config.WriteTo.File(errorLogPath,
                                outputTemplate: logFormat,
                                restrictedToMinimumLevel: LogEventLevel.Error,
                                rollingInterval: RollingInterval.Day,
                                rollOnFileSizeLimit: true,
                                shared: true,
                                fileSizeLimitBytes: 10_000_000,
                                retainedFileCountLimit: 30);
                        })
                        .WriteTo.Logger(config =>
                        {
                            config.WriteTo.File(serilogPath,
                                outputTemplate: logFormat,
                                restrictedToMinimumLevel: LogEventLevel.Warning,
                                rollingInterval: RollingInterval.Day,
                                rollOnFileSizeLimit: true,
                                shared: true,
                                fileSizeLimitBytes: 10_000_000,
                                retainedFileCountLimit: 30);
                            config.Filter.ByIncludingOnly(Matching.FromSource("Serilog.AspNetCore"));
                        });
             });
            return builder;
        }
    }
}
