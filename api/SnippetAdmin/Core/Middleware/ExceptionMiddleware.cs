using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using SnippetAdmin.Constants;
using SnippetAdmin.Models;
using System.Text.Json;

namespace SnippetAdmin.Core.Middleware
{
    public static class ExceptionMiddleware
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(configure =>
            {
                configure.Run(async httpContext =>
                {
                    var exceptionHandlerPathFeature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
                    var ex = exceptionHandlerPathFeature?.Error;
                    if (ex != null)
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status200OK;
                        httpContext.Response.ContentType = "application/json; charset=utf-8";
                        var result = new CommonResult
                        {
                            IsSuccess = false,
                            Code = MessageConstant.SYSTEM_ERROR_001.Item1,
                            Message = MessageConstant.SYSTEM_ERROR_001.Item2,
                        };
                        await JsonSerializer.SerializeAsync(httpContext.Response.Body, result, new JsonSerializerOptions
                        {
                            // 首字母小写
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        });
                    }
                });
            });
            return app;
        }
    }
}