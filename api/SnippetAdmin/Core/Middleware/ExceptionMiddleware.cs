using Microsoft.AspNetCore.Diagnostics;
using SnippetAdmin.CommonModel.Extensions;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Extensions;
using SnippetAdmin.Core.Helpers;
using SnippetAdmin.Data.Entity.System;
using System.Text.Json;

namespace SnippetAdmin.Core.Middleware
{
    public static class ExceptionMiddleware
    {
        public static IApplicationBuilder UseCustomExceptionRecorder(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(configure =>
            {
                configure.Run(async httpContext =>
                {
                    var exceptionHandlerPathFeature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
                    var ex = exceptionHandlerPathFeature?.Error;
                    if (ex != null)
                    {
                        var log = new SysExceptionLog
                        {
                            Type = ex.GetType().FullName,
                            Message = ex.Message,
                            Source = ex.Source,
                            StackTrace = ex.StackTrace,
                            Username = httpContext.User.GetUserName(),
                            Path = httpContext.Request.Path,
                            HappenedTime = DateTime.Now
                        };
                        await ChannelHelper<SysExceptionLog>.Instance.Writer.WriteAsync(log);

                        httpContext.Response.StatusCode = StatusCodes.Status200OK;
                        httpContext.Response.ContentType = "application/json; charset=utf-8";
                        var result = new CommonResult
                        {
                            IsSuccess = false,
                        };
                        if (ex is ErrorSortPropertyException)
                        {
                            result.Code = MessageConstant.SYSTEM_ERROR_004.Item1;
                            result.Message = MessageConstant.SYSTEM_ERROR_004.Item2;
                        }
                        else
                        {
                            result.Code = MessageConstant.SYSTEM_ERROR_001.Item1;
                            result.Message = MessageConstant.SYSTEM_ERROR_001.Item2;
                        }

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