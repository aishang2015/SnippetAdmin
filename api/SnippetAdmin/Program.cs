using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Orleans;
using Orleans.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using SnippetAdmin.Background;
using SnippetAdmin.Core.Authentication;
using SnippetAdmin.Core.Dynamic;
using SnippetAdmin.Core.Extensions;
using SnippetAdmin.Core.Middleware;
using SnippetAdmin.Core.Monitor;
using SnippetAdmin.Core.Oauth;
using SnippetAdmin.Core.Scheduler;
using SnippetAdmin.Core.TextJson;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Auth;
using SnippetAdmin.Grains;
using SnippetAdmin.Models;
using System.Reflection;

Log.Logger = new LoggerConfiguration().CreateBootstrapLogger();

try
{
    Log.Information("Server start Runing!");
    var builder = WebApplication.CreateBuilder(args);

    // 分别是数据库 缓存 内存缓存 jwt automapper oauth 用户访问器
    builder.Services.AddDatabase(builder.Configuration);
    builder.Services.AddMemoryCache();
    builder.Services.AddCustomAuthentication(builder.Configuration);
    builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
    builder.Services.AddOauth(builder.Configuration);

    // 自定义授权策略
    builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IAuthorizationHandler, AccessApiAuthorizationHandler>());
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AccessApi", policy => policy.Requirements.Add(new AccessApiRequirement()));
    });

    // 配置FluentValidation并改变默认modelstate的返回形式
    var mvcBuilder = builder.Services.AddControllers();
    mvcBuilder.AddFluentValidation().AddTextJsonOptions();
    mvcBuilder.AddDynamicController();

    builder.Services.ConfigureApiBehavior();

    // 添加signalr
    builder.Services.AddSignalR();

    // 添加定时任务调度器
    builder.Services.AddJobScheduler();

    // 配置swagger权限访问
    builder.Services.AddCustomSwaggerGen();

    // 跨域配置，允许所有网站访问
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy",
            builder => builder
            .SetIsOriginAllowed(o => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
    });

    // httpcontext服务
    builder.Services.AddHttpContextAccessor();

    // add miniprofiler
    builder.Services.AddMiniProfiler(options =>
    {
        // 访问地址 http://localhost:29680/profiler/results-index
        // 历史会默认保存30分钟
        options.RouteBasePath = "/profiler";
    }).AddEntityFramework();

    // add metric listener
    builder.Services.AddMetricEventListener();

    // record some log
    builder.Services.AddBackgroundService<LoginLogBackgroundService>();
    builder.Services.AddBackgroundService<AccessedLogBackgroundService>();
    builder.Services.AddBackgroundService<ExceptionLogBackgroundService>();

    // 使用serilog
    builder.Host.UseSerilog((context, services, configuration) =>
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

    // use orleans
    builder.Host.UseOrleans((ctx, builder) =>
    {
        builder.UseLocalhostClustering();
        builder.AddMemoryGrainStorage("SnippetAdminSilo");
        builder.ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(TestGrain).Assembly).WithReferences());
    });

    var app = builder.Build();

    if (builder.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.IndexStream = () => Assembly.GetExecutingAssembly().GetManifestResourceStream("SnippetAdmin.Swagger.index.html");
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "web端业务接口-v1");
            c.SwaggerEndpoint("/swagger/dynamic-v1/swagger.json", "动态接口-v1");
        });

        app.UseMiniProfiler();
    }

    // record some information
    app.UseLoginLogRecorder();
    app.UseAccessedLogRecord("/api/*");
    app.UseCustomExceptionRecorder();

    // use cors config
    app.UseCors("CorsPolicy");

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    // map signalr path
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapMetricHub();
        endpoints.MapControllers();
    });

    app.Initialize(DbContextInitializer.InitialSnippetAdminDbContext);
    app.Initialize(JobInitializer.InitialJob);
    app.Run();
}
catch (Exception e)
{
    Log.Error(e, "Exception happened!");
}
finally
{
    Log.CloseAndFlush();
}