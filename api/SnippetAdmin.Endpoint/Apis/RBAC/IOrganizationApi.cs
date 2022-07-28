using SnippetAdmin.CommonModel;
using SnippetAdmin.Endpoint.Models.RBAC.Organization;

namespace SnippetAdmin.Controllers.RBAC
{

    public interface IOrganizationApi
    {
        /// <summary>
        /// 获取组织机构详细信息
        /// </summary>
        public Task<CommonResult<GetOrganizationOutputModel>> GetOrganization(IdInputModel<int> inputModel);

        /// <summary>
        /// 获取组织树信息
        /// </summary>
        public Task<CommonResult<List<GetOrganizationTreeOutputModel>>> GetOrganizationTree();

        /// <summary>
        /// 创建组织
        /// </summary>
        public Task<CommonResult> CreateOrganization(CreateOrganizationInputModel inputModel);

        /// <summary>
        /// 删除组织
        /// </summary>
        public Task<CommonResult> DeleteOrganization(IdInputModel<int> inputModel);

        /// <summary>
        /// 修改组织
        /// </summary>
        public Task<CommonResult> UpdateOrganization(UpdateOrganizationInputModel inputModel);

        /// <summary>
        /// 查询组织类型列表
        /// </summary>
        public Task<CommonResult<List<GetOrganizationTypesOutputModel>>> GetOrganizationTypes();

        /// <summary>
        /// 添加或更新组织类型
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public Task<CommonResult> AddOrUpdateOrganizationType(
            AddOrUpdateOrganizationTypeInputModel inputModel);

        /// <summary>
        /// 删除组织类型
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public Task<CommonResult> RemoveOrganizationType(
            RemoveOrganizationTypeInputModel inputModel);
    }
}