using SnippetAdmin.Core.Extensions;
using SnippetAdmin.Core.Helpers;
using SnippetAdmin.Data.Entity.System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SnippetAdmin.Core.Middleware
{
    public static class AccessedLogMiddleware
    {
        public static IApplicationBuilder UseAccessedLogRecord(this IApplicationBuilder app,
            params string[] pathMatches)
        {
            Func<string, bool> isMatchAny = inputStr => pathMatches.Any(m => Regex.IsMatch(inputStr, m, RegexOptions.IgnoreCase));

            app.Use(async (context, next) =>
            {
                if ((pathMatches.Length != 0 && isMatchAny(context.Request.Path)) || pathMatches.Length == 0)
                {
                    if (context.Request.Method == "OPTIONS")
                    {
                        await next.Invoke();
                    }
                    else
                    {
                        var log = new SysAccessLog();
                        context.Request.EnableBuffering();

                        var stopWatch = new Stopwatch();

                        // read request body 
                        using var requestStream = new StreamReader(context.Request.Body);
                        log.RequestBody = await requestStream.ReadToEndAsync();
                        context.Request.Body.Position = 0;

                        log.Method = context.Request.Method;
                        log.Path = context.Request.Path;
                        log.RemoteIp = context.Connection.RemoteIpAddress.MapToIPv4().ToString();

                        // read original response body
                        var responseOriginalBody = context.Response.Body;
                        using var memStream = new MemoryStream();
                        context.Response.Body = memStream;

                        stopWatch.Start();
                        await next.Invoke();
                        stopWatch.Stop();

                        // reset response body
                        memStream.Position = 0;
                        using var responseReader = new StreamReader(memStream);
                        log.ResponseBody = await responseReader.ReadToEndAsync();
                        memStream.Position = 0;
                        await memStream.CopyToAsync(responseOriginalBody);
                        context.Response.Body = responseOriginalBody;

                        log.ElapsedTime = stopWatch.ElapsedMilliseconds;
                        log.StatusCode = context.Response.StatusCode;
                        log.Username = context.User.GetUserName();
                        log.AccessedTime = DateTime.Now;

                        await ChannelHelper<SysAccessLog>.Instance.Writer.WriteAsync(log);
                    }
                }
                else
                {
                    await next.Invoke();
                }
            });

            return app;
        }
    }
}
