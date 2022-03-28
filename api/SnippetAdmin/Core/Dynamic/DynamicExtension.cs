﻿using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SnippetAdmin.Core.Dynamic.Attributes;
using SnippetAdmin.Core.Utils;
using System.Reflection;
using System.Text;

namespace SnippetAdmin.Core.Dynamic
{
    public static class DynamicExtension
    {
        public static IMvcBuilder AddDynamicController(this IMvcBuilder builder)
        {
            var controllerTemplate = @$"
using Microsoft.AspNetCore.Mvc;

using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

using SnippetAdmin.Constants;
using SnippetAdmin.Data;
using SnippetAdmin.Models;
using SnippetAdmin.Models.Common;
using SnippetAdmin.Core.Extensions;
using SnippetAdmin.Models.{{Entity}};

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
        public async Task<CommonResult> GetMany([FromBody] Get{{Entity}}InputModel inputModel)
        {{
            var dataQuery = _snippetAdminDbContext.Set<{{Entity}}>().AsQueryable();
            dataQuery = dataQuery{{QueryCondition}};
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

            var searchModelTemplate = @$"

using System;
using SnippetAdmin.Models.Common;

namespace SnippetAdmin.Models.{{Entity}}
{{    
    public class Get{{Entity}}InputModel: PagedInputModel
    {{
        {{Properties}}
    }}
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

                    // 查找字典相关的属性,生成一个获取字典的请求
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

                    // 生成查找模型
                    var propertyBuilder = new StringBuilder();
                    var conditinBuilder = new StringBuilder();
                    foreach (var property in classType.GetProperties())
                    {
                        if (property.Name == "Id")
                        {
                            continue;
                        }
                        if (property.PropertyType == typeof(short) ||
                            property.PropertyType == typeof(int) ||
                            property.PropertyType == typeof(long) ||
                            property.PropertyType == typeof(double) ||
                            property.PropertyType == typeof(float) ||
                            property.PropertyType == typeof(decimal) ||
                            property.PropertyType == typeof(DateTime))
                        {
                            propertyBuilder.Append($"public {property.PropertyType.Name}? Upper{property.Name}{{get;set;}}");
                            propertyBuilder.Append($"public {property.PropertyType.Name}? Lower{property.Name}{{get;set;}}");
                            propertyBuilder.Append($"public {property.PropertyType.Name}? Equal{property.Name}{{get;set;}}");

                            conditinBuilder.Append($".AndIfExist(inputModel.Upper{property.Name},d=>d.{property.Name}<=inputModel.Upper{property.Name})");
                            conditinBuilder.Append($".AndIfExist(inputModel.Lower{property.Name},d=>d.{property.Name}>=inputModel.Lower{property.Name})");
                            conditinBuilder.Append($".AndIfExist(inputModel.Equal{property.Name},d=>d.{property.Name}==inputModel.Equal{property.Name})");
                        }
                        if (property.PropertyType == typeof(short?) ||
                            property.PropertyType == typeof(int?) ||
                            property.PropertyType == typeof(long?) ||
                            property.PropertyType == typeof(double?) ||
                            property.PropertyType == typeof(float?) ||
                            property.PropertyType == typeof(decimal?) ||
                            property.PropertyType == typeof(DateTime?))
                        {
                            var propertyTypeName = property.PropertyType.GenericTypeArguments[0].Name;

                            propertyBuilder.Append($"public {propertyTypeName}? Upper{property.Name}{{get;set;}}");
                            propertyBuilder.Append($"public {propertyTypeName}? Lower{property.Name}{{get;set;}}");
                            propertyBuilder.Append($"public {propertyTypeName}? Equal{property.Name}{{get;set;}}");
                            propertyBuilder.Append($"public bool? NotNull{property.Name}{{get;set;}}");

                            conditinBuilder.Append($".AndIfExist(inputModel.Upper{property.Name},d=>d.{property.Name}<=inputModel.Upper{property.Name})");
                            conditinBuilder.Append($".AndIfExist(inputModel.Lower{property.Name},d=>d.{property.Name}>=inputModel.Lower{property.Name})");
                            conditinBuilder.Append($".AndIfExist(inputModel.Equal{property.Name},d=>d.{property.Name}==inputModel.Equal{property.Name})");
                            conditinBuilder.Append($".AndIf(inputModel.NotNull{property.Name}!=null && inputModel.NotNull{property.Name}.Value,d=>d.{property.Name}!=null)");
                            conditinBuilder.Append($".AndIf(inputModel.NotNull{property.Name}!=null && !inputModel.NotNull{property.Name}.Value,d=>d.{property.Name}==null)");
                        }
                        else if (property.PropertyType == typeof(string))
                        {
                            propertyBuilder.Append($"public string Contained{property.Name}{{get;set;}}");
                            propertyBuilder.Append($"public string Equal{property.Name}{{get;set;}}");

                            conditinBuilder.Append($".AndIfExist(inputModel.Contained{property.Name},d=>d.{property.Name}.Contains(inputModel.Contained{property.Name}))");
                            conditinBuilder.Append($".AndIfExist(inputModel.Equal{property.Name},d=>d.{property.Name}==inputModel.Equal{property.Name})");
                        }
                        else if (property.PropertyType == typeof(bool))
                        {
                            propertyBuilder.Append($"public bool? Equal{property.Name}{{get;set;}}");

                            conditinBuilder.Append($".AndIfExist(inputModel.Equal{property.Name},d=>d.{property.Name}==inputModel.Equal{property.Name})");
                        }
                    }
                    var searchModel = searchModelTemplate.Replace("{Properties}", propertyBuilder.ToString());
                    searchModel = searchModel.Replace("{Entity}", classType.Name);
                    syntaxTreeList.Add(SyntaxFactory.ParseSyntaxTree(searchModel));

                    source = source.Replace("{QueryCondition}", conditinBuilder.ToString());
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
