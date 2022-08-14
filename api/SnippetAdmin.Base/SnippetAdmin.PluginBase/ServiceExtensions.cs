using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using SnippetAdmin.PluginBase.Messages;
using SnippetAdmin.PluginBase.Messages.Memory;
using SnippetAdmin.PluginBase.Models;
using SnippetAdmin.PluginBase.Register;
using System.Reflection;
using System.Runtime.Loader;

namespace SnippetAdmin.PluginBase
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// 将插件内的服务注册到子容器
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        public static RegistResult RegistPlugin(this IMvcBuilder builder, params string[] dllPaths)
        {
            var result = new RegistResult();
            var assemblyList = new List<Assembly>();
            var scriptList = new List<string>();
            var styleList = new List<string>();

            var messageObserver = new MemoryMessageObserver();
            builder.Services.AddSingleton(messageObserver);
            builder.Services.AddSingleton<IMessageObserver>(messageObserver);
            foreach (var dllPath in dllPaths)
            {
                var pluginAssembly = Assembly.LoadFrom(dllPath);

                assemblyList.Add(pluginAssembly);

                // 将插件内注册的服务注入到主容器
                var type = pluginAssembly.GetTypes()?
                    .Where(t => typeof(IServiceRegister).IsAssignableFrom(t) && t.IsClass).FirstOrDefault();
                if (type != null)
                {
                    var instance = Activator.CreateInstance(type) as IServiceRegister;
                    instance?.RegisterPluginServices(builder.Services);

                    scriptList.AddRange(instance.ScriptUrlList);
                    styleList.AddRange(instance.StyleUrlList);
                }

                var dllDirectory = Path.GetDirectoryName(dllPath);
                if (string.IsNullOrEmpty(dllDirectory))
                {
                    continue;
                }

                // 引用的第三方路径
                var pluginReferenceDllPathes = Directory.GetFiles(dllDirectory)
                    .Where(f => !f.StartsWith("System"))
                    .Where(f => !f.StartsWith("Microsoft"))
                    .Where(f => f.EndsWith(".dll"))
                    .ToArray();

                builder.ConfigureApplicationPartManager(manager =>
                {
                    var assemblyPart = new AssemblyPart(pluginAssembly);
                    manager.ApplicationParts.Add(assemblyPart);
                    pluginReferenceDllPathes.ToList()
                        .ForEach((path) =>
                        {
                            var context = AssemblyLoadContext.GetLoadContext(pluginAssembly);
                            context?.LoadFromAssemblyPath(path);
                        });
                });

            }

            return new RegistResult
            {
                AssemblySet = assemblyList.ToHashSet(),
                ScriptUrlSet = scriptList.ToHashSet(),
                StyleUrlSet = styleList.ToHashSet()
            };
        }
    }
}
