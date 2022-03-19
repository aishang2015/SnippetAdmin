using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace SnippetAdmin.Core.Dynamic
{
    public static class DynamicExtension
    {
        public static IMvcBuilder AddDynamicController(this IMvcBuilder builder)
        {
            var controllerTemplate = @$"
using Microsoft.AspNetCore.Mvc;
using SnippetAdmin.Models;
using System.Threading.Tasks;

namespace SnippetAdmin.Controllers
{{
    [Route(""api /[controller]"")]
    [ApiController]
    public class NetController: ControllerBase
    {{
        [HttpGet]
        public async Task<CommonResult> GetNet()
        {{
            return this.SuccessCommonResult(""Ok"");
        }}
    }}
}}
";
            var tree = SyntaxFactory.ParseSyntaxTree(controllerTemplate);
            var compilation = CSharpCompilation.Create(
                 syntaxTrees: new[] { tree },
                 assemblyName: $"assemblytest.dll",
                 options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                 references: AppDomain.CurrentDomain.GetAssemblies().Select(x => MetadataReference.CreateFromFile(x.Location)));

            Assembly compiledAssembly;
            using (var stream = new MemoryStream())
            {
                // 检测脚本代码是否有误
                var compileResult = compilation.Emit(stream);
                if (compileResult.Success)
                {
                    compiledAssembly = Assembly.Load(stream.GetBuffer());

                    builder.ConfigureApplicationPartManager(apm =>
                    {
                        var assemblyPart = new AssemblyPart(compiledAssembly);
                        apm.ApplicationParts.Add(assemblyPart);
                    });
                }
            }
            return builder;
        }
    }
}
