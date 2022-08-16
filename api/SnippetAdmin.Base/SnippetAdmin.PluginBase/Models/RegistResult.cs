using System.Reflection;

namespace SnippetAdmin.PluginBase.Models
{
    public class RegistResult
    {
        /// <summary>
        /// 设置路由扫描的包，否则即使加载了也无法访问到指定页面
        /// <Router AppAssembly="@typeof(App).Assembly"AdditionalAssemblies="PluginData.Assemblies">
        /// </summary>
        public HashSet<Assembly> AssemblySet { get; set; }

        /// <summary>
        /// 在宿主程序的_Host.cshtml中加入
        /// @foreach (var style in PluginData.StyleList)
        /// @foreach(var script in PluginData.ScriptList)
        /// {
        ///     < script src = "@script" ></ script >
        /// }
        /// </summary>
        public HashSet<string> ScriptUrlSet { get; set; }

        /// <summary>
        /// 在宿主程序的_Host.cshtml中加入
        /// @foreach (var style in PluginData.StyleList)
        /// {    
        /// 	<link href = "@style" rel="stylesheet" />
        /// }
        /// </summary>
        public HashSet<string> StyleUrlSet { get; set; }
    }
}
