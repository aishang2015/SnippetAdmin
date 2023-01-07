using SnippetAdmin.Core.Helpers;
using SnippetAdmin.Data.Entity.System;
using SnippetAdmin.Endpoint.Models.Account;
using System.Text.Json;

namespace SnippetAdmin.Core.Middleware
{
	public static class LoginLogMiddleware
	{
		public static IApplicationBuilder UseLoginLogRecorder(this IApplicationBuilder app)
		{
			app.Use(async (context, next) =>
			{
				if (context.Request.Method != "OPTIONS" &&
					context.Request.Path.StartsWithSegments("/api/account/login"))
				{
					var log = new SysLoginLog();
					context.Request.EnableBuffering();

					// read request body 
					using var requestStream = new StreamReader(context.Request.Body);
					var requestBody = await requestStream.ReadToEndAsync();
					context.Request.Body.Position = 0;

					var requestObj = JsonSerializer.Deserialize<LoginInputModel>(requestBody, new JsonSerializerOptions
					{
						// 首字母小写
						PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
					});
					log.Username = requestObj.UserName;
					log.RemoteIp = context.Connection.RemoteIpAddress.MapToIPv4().ToString();

					// read original response body
					var responseOriginalBody = context.Response.Body;
					using var memStream = new MemoryStream();
					context.Response.Body = memStream;

					await next.Invoke();

					// reset response body
					memStream.Position = 0;
					using var responseReader = new StreamReader(memStream);
					var responseBody = await responseReader.ReadToEndAsync();
					memStream.Position = 0;
					await memStream.CopyToAsync(responseOriginalBody);
					context.Response.Body = responseOriginalBody;

					var responseObj = JsonSerializer.Deserialize<CommonResult<LoginOutputModel>>(responseBody,
						new JsonSerializerOptions
						{
							// 首字母小写
							PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
						});
					log.AccessedTime = DateTime.Now;
					log.IsSucceed = responseObj.IsSuccess;

					await ChannelHelper<SysLoginLog>.Instance.Writer.WriteAsync(log);
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
