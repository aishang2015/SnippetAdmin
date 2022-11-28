using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using SnippetAdmin.Core.Attributes;
using SnippetAdmin.Endpoint.Apis.RBAC;
using SnippetAdmin.Endpoint.Models.RBAC.ApiInfo;
using System.Text;

namespace SnippetAdmin.Controllers.RBAC
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[ApiExplorerSettings(GroupName = "v1")]
	public class ApiInfoController : ControllerBase, IApiInfoApi
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

{requests}

}
""";

		private readonly IApiDescriptionGroupCollectionProvider _apiDescriptionGroupCollectionProvider;

		public ApiInfoController(IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider)
		{
			_apiDescriptionGroupCollectionProvider = apiDescriptionGroupCollectionProvider;
		}

		/// <summary>
		/// 获取程序所有API信息
		/// </summary>
		[HttpPost]
		[CommonResultResponseType<List<string>>]
		[Authorize(Policy = "AccessApi")]
		public Task<CommonResult<List<string>>> GetApiPaths()
		{
			var result = _apiDescriptionGroupCollectionProvider.ApiDescriptionGroups.Items
				.SelectMany(i => i.Items).Select(i => i.RelativePath).ToList();
			return Task.FromResult(CommonResult.Success(result));
		}

		[HttpPost]
		[CommonResultResponseType<GetTsRequestCodeOutputModel>]
		[AllowAnonymous]
		public async Task<CommonResult<GetTsRequestCodeOutputModel>> GetTsRequestCode(GetTsRequestCodeInputModel inputModel)
		{
			var apiDescriptions = _apiDescriptionGroupCollectionProvider.ApiDescriptionGroups.Items
				.SelectMany(i => i.Items)
				.Where(i => i.ActionDescriptor.DisplayName.Contains(inputModel.ControllerName));

			var controllerName = string.Empty;
			var stringBuilder = new StringBuilder();
			apiDescriptions.ToList().ForEach(desc =>
			{
				var actionDescriptor = desc.ActionDescriptor as ControllerActionDescriptor;

				var paramTypeName = GetDataTypeName(desc.ParameterDescriptions.FirstOrDefault()?.Type);
				var responseTypeName = GetDataTypeName(desc.SupportedResponseTypes.FirstOrDefault()?.Type);

				stringBuilder.AppendLine(
					RequestMethodTemplate.Replace("{requestPath}", desc.RelativePath)
						.Replace("{action}", actionDescriptor?.ActionName)
						.Replace("{requestType}", paramTypeName)
						.Replace("{responseType}", responseTypeName)
				);
			});

			var result = TsCodeTemplate
				.Replace("{requests}", stringBuilder.ToString())
				.Replace("{entity}", inputModel.ControllerName);


			return CommonResult.Success(new GetTsRequestCodeOutputModel()
			{
				RequestCode = result
			});

		}

		private string GetDataTypeName(Type type)
		{
			if (type != null)
			{
				var typeName = type.Name.Replace("`1", string.Empty);
				if (type.IsGenericType)
				{
					var genericType = type.GetGenericArguments().FirstOrDefault();

					var name = GetDataTypeName(genericType);

					typeName = $"{typeName}<{name}>";
				}
				return typeName;
			}
			return string.Empty;
		}
	}
}