using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;
using System.Linq;

namespace SnippetAdmin.Core.Middleware
{
    public static class SerilogRequestLogging
    {
        /// <summary>
        /// 输出所有请求的日志
        /// </summary>
        /// <param name="warningElapsed">超过这个时间的日志级别会变为warning否则为debug</param>
        /// <param name="ignorePathes">忽略的请求路径</param>
        public static IApplicationBuilder UseCustomSerilogRequestLogging(this IApplicationBuilder app,
            double warningElapsed = 500, params string[] ignorePathes)
        {
            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate = "Request: {Scheme} {Protocol} {Host} {RequestMethod} {RequestPath} Respond: {ContentType} {StatusCode} in {Elapsed:0.0000} ms";

                options.GetLevel = (httpContext, elapsed, exception) =>
                {
                    // 出现忽略的请求路径，将级别调整为debug
                    if (ignorePathes.Any(p => httpContext.Request.Path.Value.StartsWith(p)))
                    {
                        return LogEventLevel.Debug;
                    }

                    if (elapsed > warningElapsed)
                    {
                        return LogEventLevel.Warning;
                    }
                    else
                    {
                        return LogEventLevel.Debug;
                    }
                };

                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("Host", httpContext.Request.Host);
                    diagnosticContext.Set("Protocol", httpContext.Request.Protocol);
                    diagnosticContext.Set("Scheme", httpContext.Request.Scheme);

                    if (httpContext.Request.QueryString.HasValue)
                    {
                        diagnosticContext.Set("QueryString", httpContext.Request.QueryString.Value);
                    }

                    diagnosticContext.Set("ContentType", httpContext.Response.ContentType);
                    var endpoint = httpContext.GetEndpoint();
                    if (endpoint is object)
                    {
                        diagnosticContext.Set("EndpointName", endpoint.DisplayName);
                    }
                };
            });
            return app;
        }
    }
}