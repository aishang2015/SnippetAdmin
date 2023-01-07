using SnippetAdmin.CommonModel;
using SnippetAdmin.Endpoint.Models.RBAC.Role;

namespace SnippetAdmin.Endpoint.Apis.RBAC
{
	public interface IRoleApi
	{
		public Task<CommonResult> ActiveRole(ActiveRoleInputModel inputModel);

		public Task<CommonResult<GetRoleOutputModel>> GetRole(IdInputModel<int> inputModel);

		public Task<CommonResult<PagedOutputModel<GetRoleOutputModel>>> GetRolesAsync(PagedInputModel inputModel);

		public Task<CommonResult<List<DicOutputModel<int>>>> GetRoleDic();

		public Task<CommonResult> AddOrUpdateRoleAsync(AddOrUpdateRoleInputModel inputModel);

		public Task<CommonResult> RemoveRoleAsync(IdInputModel<int> inputModel);
	}
}