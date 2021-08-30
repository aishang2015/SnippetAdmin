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
            // �ֱ������ݿ� ���� �ڴ滺�� jwt automapper oauth �û�������
            services.AddDatabase(Configuration);
            services.AddDistributeCache(Configuration);
            services.AddMemoryCache();
            services.AddCustomAuthentication(Configuration);
            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddOauth(Configuration);
            services.AddUserAccessor();

            // ����FluentValidation���ı�Ĭ��modelstate�ķ�����ʽ
            services.AddControllers().AddFluentValidation().AddTextJsonOptions();
            services.ConfigureApiBehavior();

            // ����signalr
            services.AddSignalR();

            // ���Ӻ�̨����
            services.AddWorks();

            // ����swaggerȨ�޷���
            services.AddCustomSwaggerGen();

            // �������ã�����������վ����
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .SetIsOriginAllowed(o => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            // httpcontext����
            services.AddHttpContextAccessor();

            // ����miniprofiler
            services.AddMiniProfiler(options =>
            {
                // ���ʵ�ַ http://localhost:29680/profiler/results-index
                // ��ʷ��Ĭ�ϱ���30����
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
        }
    }
}