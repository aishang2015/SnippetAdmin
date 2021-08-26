using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SnippetAdmin.Business;
using SnippetAdmin.Core;
using SnippetAdmin.Core.Authentication;
using SnippetAdmin.Core.Cache;
using SnippetAdmin.Core.Middleware;
using SnippetAdmin.Core.Oauth;
using SnippetAdmin.Core.TextJson;
using SnippetAdmin.Core.UserAccessor;
using SnippetAdmin.Data;
using SnippetAdmin.Models;
using System.Reflection;

namespace SnippetAdmin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 分别是数据库 缓存 内存缓存 jwt automapper oauth 用户访问器
            services.AddDatabase(Configuration);
            services.AddDistributeCache(Configuration);
            services.AddMemoryCache();
            services.AddCustomAuthentication(Configuration);
            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddOauth(Configuration);
            services.AddUserAccessor();

            // 配置FluentValidation并改变默认modelstate的返回形式
            services.AddControllers().AddFluentValidation().AddTextJsonOptions();
            services.ConfigureApiBehavior();

            // 添加signalr
            services.AddSignalR();

            // 添加后台服务
            services.AddWorks();

            // 配置swagger权限访问
            services.AddCustomSwaggerGen();

            // 跨域配置，允许所有网站访问
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .SetIsOriginAllowed(o => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            // httpcontext服务
            services.AddHttpContextAccessor();

            // 添加miniprofiler
            services.AddMiniProfiler(options =>
            {
                // 访问地址 http://localhost:29680/profiler/results-index
                // 历史会默认保存30分钟
                options.RouteBasePath = "/profiler";
            }).AddEntityFramework();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.IndexStream = () => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("SnippetAdmin.Swagger.index.html");
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SnippetAdmin v1");
                });

                app.UseMiniProfiler();
            }

            // serilog提供的一个用来记录请求信息的日志中间件，所有请求的基本信息会被输出到日志中
            app.UseCustomSerilogRequestLogging(500, "/broadcast");

            // 处理异常
            app.UseCustomExceptionHandler();

            // 使用跨域配置
            app.UseCors("CorsPolicy");

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // 配置signalr路径
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHubs();
                endpoints.MapControllers();
            });
        }
    }
}