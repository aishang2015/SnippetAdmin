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
	Log.Information("����ʼ����!");
	var builder = WebApplication.CreateBuilder(args);

	// �ֱ������ݿ� ���� �ڴ滺�� jwt automapper oauth �û�������
	builder.Services.AddSnippetAdminDbContext(builder.Configuration);
	builder.Services.AddMemoryCache();
	builder.Services.AddCustomAuthentication(builder.Configuration);
	builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
	builder.Services.AddOauth(builder.Configuration);

	// ʹ��serilog
	builder.Host.UseCustomSerilog();

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
			c.SwaggerEndpoint("/swagger/v1/swagger.json", "web��ҵ��ӿ�-v1");
			c.SwaggerEndpoint("/swagger/DynamicApi/swagger.json", "��̬�ӿ�");
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

    // ����������ʽ,Ϊaccesslog��¼
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
	Log.Error(e, "�����쳣������ֹͣ!");
}
finally
{
	Log.CloseAndFlush();
}