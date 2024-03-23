global using SnippetAdmin.CommonModel;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
using SnippetAdmin.DynamicApi;
using SnippetAdmin.Grains;
using SnippetAdmin.Jobs;
using SnippetAdmin.Models;
using SnippetAdmin.Orleans;
using SnippetAdmin.Quartz;
using System.Reflection;

Log.Logger = new LoggerConfiguration()
	.Enrich.FromLogContext()
	.WriteToFile()
	.CreateBootstrapLogger();

try
{
	Log.Information("服务开始启动!");
	var builder = WebApplication.CreateBuilder(args);

	// 分别是数据库 缓存 内存缓存 jwt automapper oauth 用户访问器
	builder.Services.AddSnippetAdminDbContext(builder.Configuration);
	builder.Services.AddMemoryCache();
	builder.Services.AddCustomAuthentication(builder.Configuration);
	builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
	builder.Services.AddOauth(builder.Configuration);

	// 使用serilog
	builder.Host.UseCustomSerilog();

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
	//builder.Services.AddJobScheduler();
	builder.Services.AddScoped<HelloJob>();
	builder.Services.AddCustomerQuartz(builder.Configuration);

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

	// add file storage
	builder.Services.AddFileStorage((option) =>
	{
		option.BasePath = "LocalFileFolder";
		option.IsAbsolute = false;
		option.MaxSize = 1024;
	});

	// record some log
	//builder.Services.AddBackgroundService<ShardingInitialBackgroundService>();
	builder.Services.AddBackgroundService<LoginLogBackgroundService>();
	builder.Services.AddBackgroundService<AccessedLogBackgroundService>();
	builder.Services.AddBackgroundService<ExceptionLogBackgroundService>();

	var app = builder.Build();

	if (builder.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI(c =>
		{
			c.IndexStream = () => Assembly.GetExecutingAssembly().GetManifestResourceStream("SnippetAdmin.Swagger.index.html");
			c.SwaggerEndpoint("/swagger/v1/swagger.json", "web端业务接口-v1");
			c.SwaggerEndpoint("/swagger/DynamicApi/swagger.json", "动态接口");
		});

		app.UseMiniProfiler();
	}

	// static file access
	app.UseFileStorageAccess();

	// record some information
	app.UseLoginLogRecorder();
	app.UseCustomExceptionRecorder();

	// use cors config
	app.UseCors("CorsPolicy");

	app.UseRouting();

	app.UseAuthentication();
	app.UseAuthorization();

	app.MapMetricHub();

    // 启动倒带方式,为accesslog记录
    app.Use(next => context =>
    {
        context.Request.EnableBuffering();
        return next(context);
    });

    app.MapControllers();

	app.Initialize(DbContextInitializer.InitialSnippetAdminDbContext);
	app.Run();
}
catch (Exception e)
{
	Log.Error(e, "发生异常，服务停止!");
}
finally
{
	Log.CloseAndFlush();
}