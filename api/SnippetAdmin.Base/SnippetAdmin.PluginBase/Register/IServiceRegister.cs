using Microsoft.Extensions.DependencyInjection;

namespace SnippetAdmin.PluginBase.Register
{
	public interface IServiceRegister
	{
		public List<string> StyleUrlList { get; }

		public List<string> ScriptUrlList { get; }

		public IServiceCollection RegisterPluginServices(IServiceCollection services);
	}
}
