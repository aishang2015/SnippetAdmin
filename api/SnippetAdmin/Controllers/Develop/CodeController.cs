﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using SnippetAdmin.Core.Attributes;
using SnippetAdmin.Endpoint.Models.Develop.Code;
using System.ComponentModel;
using System.Text;
using System.Linq;

namespace SnippetAdmin.Controllers.Develop
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(GroupName = "v1")]
    public class CodeController : ControllerBase
    {
        private const string RequestMethodTemplate = """
    static {action}(param:{requestType}) {
        return Axios.instance.post<{responseType}>('{requestPath}',param);
    }
""";

        private const string TsCodeTemplate = """
import { CommonResult,CommonResultNoData } from "../common-result";
import { Axios } from "../request";

export class {entity}Service {

{requests}}
""";

        private const string TsModelTemplate = """
/*
 * {modelName}
 */
export interface {modelName} {
{properties}}

""";

        private const string TsPropertyTemplate = "    {propertyName}?: null | {propertyType};";

        private readonly IApiDescriptionGroupCollectionProvider _apiDescriptionGroupCollectionProvider;

        public CodeController(IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider)
        {
            _apiDescriptionGroupCollectionProvider = apiDescriptionGroupCollectionProvider;
        }

        /// <summary>
        /// 查询所有控制器名称
        /// </summary>
        [HttpPost]
        [CommonResultResponseType<IEnumerable<string>>]
        [Description("查询所有控制器名称")]
        public CommonResult<IEnumerable<string>> GetControllers()
        {
            var result = _apiDescriptionGroupCollectionProvider.ApiDescriptionGroups.Items.SelectMany(i => i.Items)
                .Select(i => i.ActionDescriptor as ControllerActionDescriptor)
                .Select(i => i!.ControllerName)
                .Distinct();
            return CommonResult.Success(result);
        }

#pragma warning disable CS8604 // 引用类型参数可能为 null。
        /// <summary>
        /// 生成控制器的Axios请求代码
        /// </summary>
        [HttpPost]
        [CommonResultResponseType<GetTsRequestCodeOutputModel>]
        [Description("生成控制器的Axios请求代码")]
        public CommonResult<GetTsRequestCodeOutputModel> GetTsRequestCode(GetTsRequestCodeInputModel inputModel)
        {
            var apiDescriptions = _apiDescriptionGroupCollectionProvider.ApiDescriptionGroups.Items
                .SelectMany(i => i.Items)
                .Where(i => i.ActionDescriptor.DisplayName!.Contains(inputModel.ControllerName));

            var modelList = new List<string>();
            var controllerName = string.Empty;
            var stringBuilder = new StringBuilder();
            apiDescriptions.ToList().ForEach(desc =>
            {
                var actionDescriptor = desc.ActionDescriptor as ControllerActionDescriptor;
                controllerName = actionDescriptor!.ControllerName;

                var requestType = desc.ParameterDescriptions.FirstOrDefault()?.Type!;
                var paramTypeName = GetDataTypeName(requestType);
                if (paramTypeName != null)
                {
                    modelList.Add(GenerateTypeModel(requestType));
                }

                var responseType = desc.SupportedResponseTypes.FirstOrDefault()?.Type!;
                var responseTypeName = GetDataTypeName(responseType);
                if (responseTypeName != null)
                {
                    modelList.Add(GenerateTypeModel(responseType));
                }

                stringBuilder.AppendLine(
                    RequestMethodTemplate.Replace("{requestPath}", desc.RelativePath)
                        .Replace("{action}", actionDescriptor?.ActionName)
                        .Replace("{requestType}", paramTypeName ?? "any")
                        .Replace("{responseType}", responseTypeName)
                );
            });

            var result = TsCodeTemplate
                .Replace("{requests}", stringBuilder.ToString())
                .Replace("{entity}", controllerName);

            foreach (var model in modelList.Where(m => !string.IsNullOrEmpty(m)).Distinct())
            {
                result += model;
            }

            return CommonResult.Success(new GetTsRequestCodeOutputModel()
            {
                RequestCode = result
            });

        }
#pragma warning restore CS8604 // 引用类型参数可能为 null。

        private string? GetDataTypeName(Type? type)
        {
            if (type != null)
            {
                var typeName = type.Name.Replace("`1", string.Empty);
                if (typeName == "Int32")
                {
                    typeName = "number";
                }
                if (type.IsGenericType)
                {
                    var genericType = type.GetGenericArguments().FirstOrDefault();

                    var name = GetDataTypeName(genericType);

                    typeName = $"{typeName}<{name}>";
                }
                return typeName;
            }
            return null;
        }

        private string? GenerateTypeModel(Type? type)
        {
            var result = new StringBuilder();
            if (type == null)
            {
                return null;
            }

            if (type.Name == "String" || type.Name == "Int32" || type.Name == "IFormFile"
                || type.Name == "DateTime" || type.Name == "CommonResult")
            {
                return null;
            }

            if (type.IsGenericType)
            {
                if (!type.Name.StartsWith("Nullable") && !type.Name.StartsWith("IEnumerable") &&
                    !type.Name.StartsWith("List") && !type.Name.StartsWith("CommonResult"))
                {

                    var typeName = type.Name.Replace("`1", string.Empty);
                    var genericStr = string.Empty;
                    Type[] array = type.GetGenericArguments();
                    for (int i = 0; i < array.Length; i++)
                    {
                        genericStr += $"T{i + 1},";
                    }
                    typeName += "<" + genericStr.Substring(0, genericStr.Length - 1) + ">";

                    var sb = new StringBuilder();
                    var properties = type.GetProperties();
                    foreach (var property in properties)
                    {
                        if (property.PropertyType.IsGenericType)
                        {
                            result.Append(GenerateTypeModel(property.PropertyType));
                        }

                        sb.AppendLine(TsPropertyTemplate
                            .Replace("{propertyName}", LowerFistChar(property.Name))
                            .Replace("{propertyType}", GetTsType(property.PropertyType)));
                    }

                    var modelCode = TsModelTemplate
                            .Replace("{modelName}", typeName)
                            .Replace("{properties}", sb.ToString());
                    result.AppendLine();
                    result.Append(modelCode);

                }

                var genericType = type.GetGenericArguments().FirstOrDefault();
                result.AppendLine();
                result.Append(GenerateTypeModel(genericType));
            }
            else
            {
                var sb = new StringBuilder();
                var properties = type.GetProperties();
                foreach (var property in properties)
                {
                    if (property.PropertyType.IsGenericType)
                    {
                        result.Append(GenerateTypeModel(property.PropertyType));
                    }

                    sb.AppendLine(TsPropertyTemplate
                        .Replace("{propertyName}", LowerFistChar(property.Name))
                        .Replace("{propertyType}", GetTsType(property.PropertyType)));
                }
                var modelCode = TsModelTemplate
                        .Replace("{modelName}", type.Name)
                        .Replace("{properties}", sb.ToString());

                result.AppendLine();
                result.Append(modelCode);
            }
            return result.ToString();
        }

        private string? GetTsType(Type type)
        {
            var typeName = string.Empty;
            if (type.IsGenericType && typeof(Nullable<>) == type.GetGenericTypeDefinition())
            {
                type = type.GetGenericArguments().FirstOrDefault()!;
                typeName = type.Name;
            }
            else if (type.IsGenericType && typeof(IEnumerable<>) == type.GetGenericTypeDefinition())
            {
                type = type.GetGenericArguments().FirstOrDefault()!;
                typeName = type.Name + "[]";
            }
            else if (type.IsGenericType)
            {
                typeName = type.Name.Replace("`1", string.Empty) + "<T>";
            }
            else
            {
                typeName = type.Name;
            }

            return typeName switch
            {
                "DateTime" => "Date",
                "Boolean" => "boolean",
                "String" => "string",
                "Int32" => "number",
                "DateTime[]" => "Date[]",
                "Boolean[]" => "boolean[]",
                "String[]" => "string[]",
                "Int32[]" => "number[]",
                _ => typeName
            };
        }

        private string LowerFistChar(string word)
        {
            if (!string.IsNullOrEmpty(word))
            {
                if (word.Length == 1)
                {
                    return word.ToLower();
                }
                return word.Substring(0, 1).ToLower() + word.Substring(1);
            }
            return word;
        }
    }
}
