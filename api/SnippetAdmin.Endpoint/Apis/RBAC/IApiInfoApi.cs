using SnippetAdmin.CommonModel;

namespace SnippetAdmin.Endpoint.Apis.RBAC
{
	public interface IApiInfoApi
	{
		/// <summary>
		/// 获取程序所有API信息
		/// </summary>
		public Task<CommonResult<List<string?>>> GetApiPaths();
	}
}