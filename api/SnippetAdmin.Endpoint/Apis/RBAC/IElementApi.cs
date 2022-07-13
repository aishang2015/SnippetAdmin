using SnippetAdmin.Endpoint.Models;
using SnippetAdmin.Endpoint.Models.Common;
using SnippetAdmin.Endpoint.Models.RBAC.Element;

namespace SnippetAdmin.Endpoint.Apis.RBAC
{
    public interface IElementApi
    {
        /// <summary>
        /// 获取元素详细信息
        /// </summary>      
        public Task<CommonResult<GetElementOutputModel>> GetElement(IdInputModel<int> inputModel);

        /// <summary>
        /// 获取元素树信息
        /// </summary>
        public Task<CommonResult<List<GetElementTreeOutputModel>>> GetElementTree();

        /// <summary>
        /// 创建页面元素
        /// </summary>
        public Task<CommonResult> CreateElement(CreateElementInputModel inputModel);

        /// <summary>
        /// 删除页面元素
        /// </summary>
        public Task<CommonResult> DeleteElement(IdInputModel<int> inputModel);

        /// <summary>
        /// 修改页面元素
        /// </summary>
        public Task<CommonResult> UpdateElement(UpdateElementInputModel inputModel);
    }
}