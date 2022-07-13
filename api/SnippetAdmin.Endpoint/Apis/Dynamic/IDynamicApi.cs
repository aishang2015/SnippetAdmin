using SnippetAdmin.Endpoint.Models;
using SnippetAdmin.Endpoint.Models.Dynamic;

namespace SnippetAdmin.Controllers.Dynamic
{
    public interface IDynamicApi
    {
        /// <summary>
        /// 获取程序所有动态接口信息
        /// </summary>
        public Task<CommonResult<List<GetDynamicInfoOutputModel>>> GetDynamicInfo();

        /// <summary>
        /// 获取动态实体列信息
        /// </summary>
        public Task<CommonResult<List<GetColumnsOutputModel>>> GetColumns(GetColumnsInputModel inputModel);
    }
}
