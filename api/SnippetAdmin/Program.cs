global using SnippetAdmin.CommonModel;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Orleans;
using Orleans.Hosting;
using Serilog;
using SnippetAdmin.Background;
using SnippetAdmin.Core.Authentication;
using SnippetAdmin.Core.Extensions;
using SnippetAdmin.Core.FileStore;
using SnippetAdmin.Core.Logger;
using SnippetAdmin.Core.Middleware;
using SnippetAdmin.Core.Monitor;
using SnippetAdmin.Core.Oauth;
using SnippetAdmin.Core.TextJson;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Auth;
using SnippetAdmin.Data.Entity.Rbac;
using SnippetAdmin.DynamicApi;
using SnippetAdmin.Grains;
using SnippetAdmin.Jobs;
using SnippetAdmin.Models;
using SnippetAdmin.Quartz;
using System.Reflection;

Log.Logger = new LoggerConfiguration().CreateBootstrapLogger();

try
{
    Log.Information("Server start Runing!");
    var builder = WebApplication.CreateBuilder(args);

    // �ֱ������ݿ� ���� �ڴ滺�� jwt automapper oauth �û�������
    builder.Services.AddDatabase<SnippetAdminDbContext, RbacUser, RbacRole>(builder.Configuration);
    builder.Services.AddMemoryCache();
    builder.Services.AddCustomAuthentication(builder.Configuration);
    builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
    builder.Services.AddOauth(builder.Configuration);

    // �Զ�����Ȩ����
    builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IAuthorizationHandler, AccessApiAuthorizationHandler>());
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AccessApi", policy => policy.Requirements.Add(new AccessApiRequirement()));
    });

    // ����FluentValidation���ı�Ĭ��modelstate�ķ�����ʽ
    var mvcBuilder = builder.Services.AddControllers();
    mvcBuilder.AddFluentValidation().AddTextJsonOptions();
    mvcBuilder.AddDynamicController();

    builder.Services.ConfigureApiBehavior();

    // ���signalr
    builder.Services.AddSignalR();

    // ��Ӷ�ʱ���������
    //builder.Services.AddJobScheduler();
    builder.Services.AddScoped<HelloJob>();
    builder.Services.AddCustomerQuartz(builder.Configuration);

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

    // add miniprofiler
    builder.Services.AddMiniProfiler(options =>
    {
        // ���ʵ�ַ http://localhost:29680/profiler/results-index
        // ��ʷ��Ĭ�ϱ���30����
        options.RouteBasePath = "/profiler";
    }).AddEntityFramework();

    // add metric listener
    builder.Services.AddMetricEventListener();

    // add file storage
    builder.Services.AddFileStorage((option) =>
    {
        option.BasePath = "LocalFileFolder";
        option.IsAbsolute = false;
        option.MaxSize = 1024;
    });

    // record some log
    builder.Services.AddBackgroundService<LoginLogBackgroundService>();
    builder.Services.AddBackgroundService<AccessedLogBackgroundService>();
    builder.Services.AddBackgroundService<ExceptionLogBackgroundService>();

    // ʹ��serilog
    builder.Host.UseCustomSerilog();

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
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "web��ҵ��ӿ�-v1");
            c.SwaggerEndpoint("/swagger/DynamicApi/swagger.json", "��̬�ӿ�");
        });

        app.UseMiniProfiler();
    }

    // static file access
    app.UseFileStorageAccess();

    // record some information
    app.UseLoginLogRecorder();
    app.UseAccessedLogRecord("/api/ApiInfo/*",
        "/api/Element/*",
        "/api/Organization/*",
        "/api/Position/*",
        "/api/Role/*",
        "/api/User/*",
        "/api/Job/*",
        "/api/JobRecord/*",
        "/api/Dic/*");
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