using Orleans;
using Orleans.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using SnippetAdmin.Business.BackgroundServices;
using SnippetAdmin.Business.Grains.Implements;
using SnippetAdmin.Business.Hubs;
using SnippetAdmin.Core;
using SnippetAdmin.Core.Authentication;
using SnippetAdmin.Core.Background;
using SnippetAdmin.Core.Dynamic;
using SnippetAdmin.Core.Middleware;
using SnippetAdmin.Core.Monitor;
using SnippetAdmin.Core.Oauth;
using SnippetAdmin.Core.Scheduler;
using SnippetAdmin.Core.TextJson;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Cache;
using SnippetAdmin.Models;
using System.Reflection;

Log.Logger = new LoggerConfiguration().CreateBootstrapLogger();

try
{
    Log.Information("Server start Runing!");
    var builder = WebApplication.CreateBuilder(args);

    // �ֱ������ݿ� ���� �ڴ滺�� jwt automapper oauth �û�������
    builder.Services.AddDatabase(builder.Configuration);
    builder.Services.AddMemoryCache();
    builder.Services.AddCustomAuthentication(builder.Configuration);
    builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
    builder.Services.AddOauth(builder.Configuration);

    // ����FluentValidation���ı�Ĭ��modelstate�ķ�����ʽ
    var mvcBuilder = builder.Services.AddControllers();
    mvcBuilder.AddFluentValidation().AddTextJsonOptions();
    mvcBuilder.AddDynamicController();

    builder.Services.ConfigureApiBehavior();

    // ���signalr
    builder.Services.AddSignalR();

    // ��Ӷ�ʱ���������
    builder.Services.AddJobScheduler();

    // ����swaggerȨ�޷���
    builder.Services.AddCustomSwaggerGen();

    // �������ã�����������վ����
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy",
            builder => builder
            .SetIsOriginAllowed(o => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
    });

    // httpcontext����
    builder.Services.AddHttpContextAccessor();

    // ���miniprofiler
    builder.Services.AddMiniProfiler(options =>
    {
        // ���ʵ�ַ http://localhost:29680/profiler/results-index
        // ��ʷ��Ĭ�ϱ���30����
        options.RouteBasePath = "/profiler";
    }).AddEntityFramework();

    // ���ϵͳָ�������
    builder.Services.AddMetricEventListener();

    // ���ָ��㲥
    builder.Services.AddBackgroundService<MetricsBackgroundService>();

    // ʹ��serilog
    builder.Host.UseSerilog((context, services, configuration) =>
    {
        // ��ȡappsetting�ڵ���־����
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

    // ʹ��orleans
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
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "SnippetAdmin v1");
        });

        app.UseMiniProfiler();
    }

    // serilog�ṩ��һ��������¼������Ϣ����־�м������������Ļ�����Ϣ�ᱻ�������־��
    app.UseCustomSerilogRequestLogging(500, "/broadcast");

    // �����쳣
    app.UseCustomExceptionHandler();

    // ʹ�ÿ�������
    app.UseCors("CorsPolicy");

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    // ����signalr·��
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapHubs();
        endpoints.MapControllers();
    });

    app.Initialize(DbContextInitializer.InitialSnippetAdminDbContext);
    app.Initialize(MemoryCacheInitializer.InitialCache);
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