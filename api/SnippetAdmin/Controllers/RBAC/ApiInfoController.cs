using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using SnippetAdmin.Core.Attributes;
using SnippetAdmin.Endpoint.Apis.RBAC;
using SnippetAdmin.Endpoint.Models.RBAC.ApiInfo;

namespace SnippetAdmin.Controllers.RBAC
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[ApiExplorerSettings(GroupName = "v1")]
	public class ApiInfoController : ControllerBase, IApiInfoApi
	{
		private const string RequestMethodTemplate = """
    static {action}(param:{requestType}) {
        return Axios.instance.post<{responseType}>('api/Data/GetCsvDataType',param);
    }
""";

		private const string TsCodeTemplate = """
import { CommonResult } from "../common-result";
import { Axios } from "../request";

export class ExportService {

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
		[Authorize(Policy = "AccessApi")]
		public async Task<CommonResult<GetTsRequestCodeOutputModel>> GetTsRequestCode(GetTsRequestCodeInputModel inputModel)
		{
			var result = _apiDescriptionGroupCollectionProvider.ApiDescriptionGroups.Items
				.SelectMany(i => i.Items)
				.Where(i => i.ActionDescriptor.DisplayName.Contains(inputModel.ControllerName));

			return CommonResult.Success(new GetTsRequestCodeOutputModel()
			{
				RequestCode = TsCodeTemplate
			});

		}
	}
}