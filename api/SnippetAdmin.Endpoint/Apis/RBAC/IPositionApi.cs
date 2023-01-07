using SnippetAdmin.CommonModel;
using SnippetAdmin.Endpoint.Models.RBAC.Position;

namespace SnippetAdmin.Endpoint.Apis.RBAC
{
	public interface IPositionApi
	{

		public Task<CommonResult> AddOrUpdatePositionAsync(AddOrUpdatePositionInputModel inputModel);

		public Task<CommonResult> DeletePositionAsync(DeletePositionInputModel inputModel);

		public Task<CommonResult<GetPositionOutputModel>> GetPosition(IdInputModel<int> inputModel);

		public Task<CommonResult<PagedOutputModel<GetPositionsOutputModel>>> GetPositions(PagedInputModel inputModel);

		public Task<CommonResult<List<DicOutputModel<int>>>> GetPositionDic();
	}
}
