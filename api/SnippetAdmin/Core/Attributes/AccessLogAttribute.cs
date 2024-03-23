
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SnippetAdmin.Core.Extensions;
using SnippetAdmin.Core.Helpers;
using SnippetAdmin.Data.Entity.System;
using System.Diagnostics;
using System.Text.Json;

namespace SnippetAdmin.Core.Attributes
{
    public class AccessLogAttribute : ActionFilterAttribute
    {
        private Stopwatch _stopwatch;

        private SysAccessLog _accessLog;

        public string Module { get; }

        public string Method { get; }

        private bool IsRecordArguments { get; }

        private bool IsRecordResult { get; }

        public AccessLogAttribute(string module, string method, bool isRecordArguments = true,
            bool isRecordResult = true)
        {
            Module = module;
            Method = method;
            IsRecordArguments = isRecordArguments;
            IsRecordResult = isRecordResult;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            var request = context.HttpContext.Request;

            _accessLog = new SysAccessLog();
            _stopwatch = new Stopwatch();

            _accessLog.TraceIdentifier = context.HttpContext.TraceIdentifier;
            _accessLog.Path = context.HttpContext.Request.Path;
            _accessLog.Module = Module;
            _accessLog.Method = Method;
            _accessLog.AccessedTime = DateTime.Now;
            _accessLog.UserId = context.HttpContext.User.GetUserId();
            _accessLog.RemoteIp = context.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

            if (IsRecordArguments)
            {
                request.Body.Position = 0;
                using var requestReader = new StreamReader(request.Body);
                _accessLog.RequestBody = requestReader.ReadToEndAsync().Result;
                request.Body.Position = 0;
            }

            _stopwatch.Start();

        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);

            _stopwatch.Stop();
            _accessLog.ElapsedTime = _stopwatch.ElapsedMilliseconds;
            _accessLog.StatusCode = context.HttpContext.Response.StatusCode;

            if (IsRecordResult && context.Result is ObjectResult objectResult)
            {
                _accessLog.ResponseBody = JsonSerializer.Serialize(objectResult.Value);
            }

            ChannelHelper<SysAccessLog>.Instance.Writer.WriteAsync(_accessLog).ConfigureAwait(false);
        }


    }
}
