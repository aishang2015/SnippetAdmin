using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using SnippetAdmin.DynamicApi.Attributes;
using SnippetAdmin.DynamicApi.Templates;
using System.Reflection;
using System.Text;

namespace SnippetAdmin.DynamicApi
{
    public static class DynamicExtension
    {
        public static IMvcBuilder AddDynamicController(this IMvcBuilder builder)
        {
            var classes = DependencyContext.Default.CompileLibraries
                .Where(lib => !lib.Serviceable && lib.Type != "referenceassembly")
                .Select(lib => Assembly.Load(lib.Name))
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetCustomAttribute(typeof(DynamicApiAttribute)) != null).ToList();

            var syntaxTreeList = new List<SyntaxTree>();

            if (classes.Any())
            {
                foreach (var classType in classes)
                {
                    var controllerSource = DbContextTemplateConstant.ControllerTemplate;

                    // 特性
                    var attribute = classType.GetCustomAttribute(typeof(DynamicApiAttribute))
                        as DynamicApiAttribute;
                    controllerSource = controllerSource.Replace("{DbContextType}", attribute?.DbContextType.Name);
                    controllerSource = controllerSource.Replace("{DbContextName}", "_" + attribute?.DbContextType.Name.ToLower());
                    controllerSource = controllerSource.Replace("{DbContextNamespace}", attribute?.DbContextType.Namespace);

                    controllerSource = controllerSource.Replace("{GroupName}", attribute?.ApiGroup ?? "DynamicApi");

                    // 查找字典相关的属性,生成一个获取字典的请求
                    var dicKeyProperty = classType.GetProperties().FirstOrDefault(p => p.GetCustomAttribute(typeof(DynamicDicKeyAttribute)) != null);
                    var dicValueProperty = classType.GetProperties().FirstOrDefault(p => p.GetCustomAttribute(typeof(DynamicDicValueAttribute)) != null);
                    if (dicKeyProperty != null && dicValueProperty != null)
                    {
                        var dicAction = DbContextTemplateConstant.DicActionTemplate
                            .Replace("{KeyProperty}", dicKeyProperty.Name)
                            .Replace("{ValueProperty}", dicValueProperty.Name);
                        controllerSource = controllerSource.Replace("{dicAction}", dicAction);
                    }
                    else
                    {
                        controllerSource = controllerSource.Replace("{dicAction}", string.Empty);
                    }

                    #region 生成getmany2接口用

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
                            propertyBuilder.Append($"public {property.PropertyType.Name}? Upper{property.Name}{{get;set;}}\n        ");
                            propertyBuilder.Append($"public {property.PropertyType.Name}? Lower{property.Name}{{get;set;}}\n        ");
                            propertyBuilder.Append($"public {property.PropertyType.Name}? Equal{property.Name}{{get;set;}}\n        ");

                            conditinBuilder.Append($".AndIfExist(inputModel.Upper{property.Name},d=>d.{property.Name}<=inputModel.Upper{property.Name})\n                ");
                            conditinBuilder.Append($".AndIfExist(inputModel.Lower{property.Name},d=>d.{property.Name}>=inputModel.Lower{property.Name})\n                ");
                            conditinBuilder.Append($".AndIfExist(inputModel.Equal{property.Name},d=>d.{property.Name}==inputModel.Equal{property.Name})\n                ");
                        }
                        else if (property.PropertyType == typeof(short?) ||
                                property.PropertyType == typeof(int?) ||
                                property.PropertyType == typeof(long?) ||
                                property.PropertyType == typeof(double?) ||
                                property.PropertyType == typeof(float?) ||
                                property.PropertyType == typeof(decimal?) ||
                                property.PropertyType == typeof(DateTime?))
                        {
                            var propertyTypeName = property.PropertyType.GenericTypeArguments[0].Name;

                            propertyBuilder.Append($"public {propertyTypeName}? Upper{property.Name}{{get;set;}}\n        ");
                            propertyBuilder.Append($"public {propertyTypeName}? Lower{property.Name}{{get;set;}}\n        ");
                            propertyBuilder.Append($"public {propertyTypeName}? Equal{property.Name}{{get;set;}}\n        ");
                            propertyBuilder.Append($"public bool? NotNull{property.Name}{{get;set;}}\n        ");

                            conditinBuilder.Append($".AndIfExist(inputModel.Upper{property.Name},d=>d.{property.Name}<=inputModel.Upper{property.Name})\n                ");
                            conditinBuilder.Append($".AndIfExist(inputModel.Lower{property.Name},d=>d.{property.Name}>=inputModel.Lower{property.Name})\n                ");
                            conditinBuilder.Append($".AndIfExist(inputModel.Equal{property.Name},d=>d.{property.Name}==inputModel.Equal{property.Name})\n                ");
                            conditinBuilder.Append($".AndIf(inputModel.NotNull{property.Name}!=null && inputModel.NotNull{property.Name}.Value,d=>d.{property.Name}!=null)\n                ");
                            conditinBuilder.Append($".AndIf(inputModel.NotNull{property.Name}!=null && !inputModel.NotNull{property.Name}.Value,d=>d.{property.Name}==null)\n                ");
                        }
                        else if (property.PropertyType == typeof(string))
                        {
                            propertyBuilder.Append($"public string Contained{property.Name}{{get;set;}}\n        ");
                            propertyBuilder.Append($"public string Equal{property.Name}{{get;set;}}\n        ");

                            conditinBuilder.Append($".AndIfExist(inputModel.Contained{property.Name},d=>d.{property.Name}.Contains(inputModel.Contained{property.Name}))\n                ");
                            conditinBuilder.Append($".AndIfExist(inputModel.Equal{property.Name},d=>d.{property.Name}==inputModel.Equal{property.Name})\n                ");
                        }
                        else if (property.PropertyType == typeof(bool))
                        {
                            propertyBuilder.Append($"public bool? Equal{property.Name}{{get;set;}}\n        ");

                            conditinBuilder.Append($".AndIfExist(inputModel.Equal{property.Name},d=>d.{property.Name}==inputModel.Equal{property.Name})\n                ");
                        }
                        else if (property.PropertyType.IsEnum)
                        {
                            propertyBuilder.Append($"public {property.PropertyType}? Equal{property.Name}{{get;set;}}\n        ");

                            conditinBuilder.Append($".AndIfExist(inputModel.Equal{property.Name},d=>d.{property.Name}==inputModel.Equal{property.Name})\n                ");
                        }
                    }
                    var searchModelSource = DbContextTemplateConstant.SearchModelTemplate.Replace("{Properties}", propertyBuilder.ToString());
                    searchModelSource = searchModelSource.Replace("{Entity}", classType.Name);
                    syntaxTreeList.Add(SyntaxFactory.ParseSyntaxTree(searchModelSource));
                    controllerSource = controllerSource.Replace("{QueryCondition}", conditinBuilder.ToString());
                    #endregion

                    controllerSource = controllerSource.Replace("{Namespace}", classType.Namespace);
                    controllerSource = controllerSource.Replace("{Entity}", classType.Name);
                    syntaxTreeList.Add(SyntaxFactory.ParseSyntaxTree(controllerSource));

                    WriteToFile("Dynamic", $"{classType.Name}Controller.cs", controllerSource);
                    WriteToFile("Dynamic", $"Get{classType.Name}InputModel.cs", searchModelSource);
                }

                var compilation = CSharpCompilation.Create(
                     syntaxTrees: syntaxTreeList,
                     assemblyName: $"dynamicController.dll",
                     options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                     references: AppDomain.CurrentDomain.GetAssemblies().Select(x => MetadataReference.CreateFromFile(x.Location)));

                Assembly compiledAssembly;
				using var stream = new MemoryStream();

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

        public static void WriteToFile(string directory, string fileName, string fileContent, string filePath = "")
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = AppContext.BaseDirectory;
            }
            filePath = Path.Combine(filePath, directory);

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            var fullpath = Path.Combine(filePath, fileName);

			using var file = new StreamWriter(File.Create(fullpath));
			file.Write(fileContent);
		}
    }
}
