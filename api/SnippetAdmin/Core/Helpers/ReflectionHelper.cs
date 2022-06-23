using Microsoft.Extensions.DependencyModel;
using System.Reflection;

namespace SnippetAdmin.Core.Helpers
{
    public static class ReflectionHelper
    {
        /// <summary>
        /// 获取当前应用的所有程序集
        /// </summary>
        public static IEnumerable<Assembly> GetAssemblies()
        {
            return DependencyContext.Default.CompileLibraries
                        .Where(lib => !lib.Serviceable && lib.Type != "referenceassembly")
                        .Select(lib => Assembly.Load(lib.Name));
        }

        /// <summary>
        /// 获取程序集中所有类型
        /// </summary>
        public static IEnumerable<Type> GetAssemblyTypes()
        {
            return GetAssemblies()
                .SelectMany(a => a.GetTypes());
        }

        /// <summary>
        /// 获取子类
        /// </summary>
        public static IEnumerable<Type> GetSubClass<T>()
        {
            return GetAssemblyTypes()
                .Where(t => typeof(T).IsAssignableFrom(t));
        }
    }
}