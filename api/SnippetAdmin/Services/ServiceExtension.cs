using SnippetAdmin.Services.Impl;

namespace SnippetAdmin.Services
{
	public static class ServiceExtension
	{
		public static IServiceCollection AddServices(this IServiceCollection services)
		{
			services.AddScoped<IAccessService, AccessService>();

			return services;
		}
	}
}
