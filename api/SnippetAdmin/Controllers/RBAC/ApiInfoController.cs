using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using SnippetAdmin.Core.Attributes;
using SnippetAdmin.Endpoint.Apis.RBAC;
using System.ComponentModel;

namespace SnippetAdmin.Controllers.RBAC
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[ApiExplorerSettings(GroupName = "v1")]
	public class ApiInfoController : ControllerBase, IApiInfoApi
	{

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
		[Description("获取程序所有API信息")]
		public Task<CommonResult<List<string>>> GetApiPaths()
		{
			var result = _apiDescriptionGroupCollectionProvider.ApiDescriptionGroups.Items
				.SelectMany(i => i.Items).Select(i => i.RelativePath).ToList();
			return Task.FromResult(CommonResult.Success(result));
		}
	}
}