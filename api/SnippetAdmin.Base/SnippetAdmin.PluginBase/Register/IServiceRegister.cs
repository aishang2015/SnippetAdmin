using Microsoft.Extensions.DependencyInjection;

namespace SnippetAdmin.PluginBase.Register
{
    public interface IServiceRegister
    {
        public IServiceCollection RegisterPluginServices(IServiceCollection services);
    }
}
