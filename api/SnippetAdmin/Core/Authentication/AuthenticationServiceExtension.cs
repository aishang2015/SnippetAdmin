﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace SnippetAdmin.Core.Authentication
{
	public static class AuthenticationServiceExtension
	{
		public static IServiceCollection AddCustomAuthentication(this IServiceCollection services,
				IConfiguration configuration)
		{
			// 取得并绑定jwt配置
			var config = configuration.GetSection("JwtOption");
			services.Configure<JwtOption>(config);
			var jwtOption = config.Get<JwtOption>();

			var validationParameters = new TokenValidationParameters
			{
				ValidAudience = jwtOption.Audience,
				ValidateAudience = false,

				ValidIssuer = jwtOption.Issuer,
				ValidateIssuer = false,

				IssuerSigningKey = jwtOption.SecurityKey,
				ValidateIssuerSigningKey = true,

				ValidateLifetime = true
			};

			services.AddSingleton(validationParameters);

			// 配置认证规则
			services.AddAuthentication(option =>
			{
				option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(option =>
			{
				// 认证的一些参数
				option.TokenValidationParameters = validationParameters;

				// signalr的token配置
				option.Events = new JwtBearerEvents
				{
					OnMessageReceived = context =>
					{
						var accessToken = context.Request.Query["access_token"];
						var path = context.HttpContext.Request.Path;
						if (!string.IsNullOrEmpty(accessToken) &&
							(path.StartsWithSegments("/broadcast")))
						{
							context.Token = accessToken;
						}
						return Task.CompletedTask;
					}
				};
			});

			// 注入token工厂
			services.AddScoped<IJwtFactory, JwtFactory>();
			services.AddHttpContextAccessor();
			return services;
		}
	}
}