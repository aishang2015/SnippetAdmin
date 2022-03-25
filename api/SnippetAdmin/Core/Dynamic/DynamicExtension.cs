using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SnippetAdmin.Core.Dynamic.Attributes;
using SnippetAdmin.Core.Utils;
using System.Reflection;

namespace SnippetAdmin.Core.Dynamic
{
    public static class DynamicExtension
    {
        public static IMvcBuilder AddDynamicController(this IMvcBuilder builder)
        {
            var controllerTemplate = @$"
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

using SnippetAdmin.Constants;
using SnippetAdmin.Data;
using SnippetAdmin.Models;
using SnippetAdmin.Models.Common;

using {{Namespace}};

namespace SnippetAdmin.Controllers
{{
    [Route(""api/[controller]"")]
    [ApiController]
    [ApiExplorerSettings(GroupName = ""dynamic-v1"")]
    public class {{Entity}}Controller: ControllerBase
    {{

        private readonly SnippetAdminDbContext _snippetAdminDbContext;

        public {{Entity}}Controller(SnippetAdminDbContext snippetAdminDbContext)
        {{
            _snippetAdminDbContext = snippetAdminDbContext;
        }}
        
        [HttpPost(""FindOne"")]
        public async Task<CommonResult> FindOne([FromBody] IntIdInputModel inputModel)
        {{
            var result = _snippetAdminDbContext.Set<{{Entity}}>().Find(inputModel.Id);
            return this.SuccessCommonResult(result);
        }}

        [HttpPost(""GetMany"")]
        public async Task<CommonResult> GetMany([FromBody] PagedInputModel inputModel)
        {{
            var dataQuery = _snippetAdminDbContext.Set<{{Entity}}>().AsQueryable();
            dataQuery = inputModel.GetSortExpression(dataQuery);
            var result = new PagedOutputModel<{{Entity}}>
            {{
                Total = dataQuery.Count(),
                Data = dataQuery.Skip(inputModel.SkipCount).Take(inputModel.TakeCount).ToList()
            }};            
            return this.SuccessCommonResult(result);
        }}

        [HttpPost(""AddOne"")]
        public async Task<CommonResult> AddOne([FromBody] {{Entity}} entity)
        {{
            _snippetAdminDbContext.Set<{{Entity}}>().Add(entity);
            await _snippetAdminDbContext.SaveChangesAsync();
            return this.SuccessCommonResult(MessageConstant.SYSTEM_INFO_003);
        }}

        [HttpPost(""AddMany"")]
        public async Task<CommonResult> AddOne([FromBody] IEnumerable<{{Entity}}> data)
        {{
            _snippetAdminDbContext.Set<{{Entity}}>().AddRange(data);
            await _snippetAdminDbContext.SaveChangesAsync();
            return this.SuccessCommonResult(MessageConstant.SYSTEM_INFO_003);
        }}

        [HttpPost(""UpdateOne"")]
        public async Task<CommonResult> UpdateOne([FromBody] {{Entity}} entity)
        {{
            _snippetAdminDbContext.Set<{{Entity}}>().Update(entity);
            await _snippetAdminDbContext.SaveChangesAsync();
            return this.SuccessCommonResult(MessageConstant.SYSTEM_INFO_002);
        }}

        [HttpPost(""DeleteOne"")]
        public async Task<CommonResult> DeleteOne([FromBody] IntIdInputModel inputModel)
        {{
            var result = _snippetAdminDbContext.Set<{{Entity}}>().Find(inputModel.Id);
            _snippetAdminDbContext.Remove(result);
            await _snippetAdminDbContext.SaveChangesAsync();
            return this.SuccessCommonResult(MessageConstant.SYSTEM_INFO_001);
        }}

        {{dicAction}}
    }}
}}
";
            var dicActionTemplate = @$"

        [HttpPost(""GetDic"")]
        public async Task<CommonResult> GetDic()
        {{
            var query = _snippetAdminDbContext.Set<{{Entity}}>().Select(d=> new DicOutputModel{{
                Key = d.{{KeyProperty}}.ToString(),
                Value = d.{{ValueProperty}}.ToString()
            }}).Distinct();
            return this.SuccessCommonResult(query.ToList());
        }}
";


            var classes = ReflectionUtil.GetAssemblyTypes()
                .Where(t => t.GetCustomAttribute(typeof(DynamicApiAttribute)) != null).ToList();

            var syntaxTreeList = new List<SyntaxTree>();

            if (classes.Any())
            {
                foreach (var classType in classes)
                {
                    var source = controllerTemplate;

                    // 查找字典,生成字典请求
                    var dicKeyProperty = classType.GetProperties().FirstOrDefault(p => p.GetCustomAttribute(typeof(DynamicDicKeyAttribute)) != null);
                    var dicValueProperty = classType.GetProperties().FirstOrDefault(p => p.GetCustomAttribute(typeof(DynamicDicValueAttribute)) != null);
                    if (dicKeyProperty != null && dicValueProperty != null)
                    {
                        var dicAction = dicActionTemplate
                            .Replace("{KeyProperty}", dicKeyProperty.Name)
                            .Replace("{ValueProperty}", dicValueProperty.Name);
                        source = source.Replace("{dicAction}", dicAction);
                    }
                    else
                    {
                        source = source.Replace("{dicAction}", string.Empty);
                    }

                    source = source.Replace("{Namespace}", classType.Namespace);
                    source = source.Replace("{Entity}", classType.Name);
                    syntaxTreeList.Add(SyntaxFactory.ParseSyntaxTree(source));
                }

                var compilation = CSharpCompilation.Create(
                     syntaxTrees: syntaxTreeList,
                     assemblyName: $"dynamicController.dll",
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
            }
            return builder;
        }
    }
}
