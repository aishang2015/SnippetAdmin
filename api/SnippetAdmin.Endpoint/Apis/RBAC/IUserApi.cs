using SnippetAdmin.CommonModel;
using SnippetAdmin.Endpoint.Models.RBAC.User;

namespace SnippetAdmin.Endpoint.Apis.RBAC
{
	public interface IUserApi
	{
		public Task<CommonResult> ActiveUserAsync(ActiveUserInputModel inputModel);

		public Task<CommonResult<GetUserOutputModel>> GetUserAsync(IdInputModel<int> inputModel);

		public Task<CommonResult<PagedOutputModel<SearchUserOutputModel>>> SearchUser(SearchUserInputModel inputModel);

		public Task<CommonResult> AddOrUpdateUserAsync(AddOrUpdateUserInputModel inputModel);

		public Task<CommonResult> RemoveUserAsync(IdInputModel<int> inputModel);

		public Task<CommonResult> SetUserPasswordAsync(SetUserPasswordInputModel inputModel);

		public Task<CommonResult> AddOrgMemberAsync(AddOrgMemberInputModel inputModel);

		public Task<CommonResult> RemoveOrgMemberAsync(RemoveOrgMemberInputModel inputModel);
	}
}