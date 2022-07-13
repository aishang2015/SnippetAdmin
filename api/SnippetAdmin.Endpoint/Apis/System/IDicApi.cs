using SnippetAdmin.Endpoint.Models;
using SnippetAdmin.Endpoint.Models.Common;
using SnippetAdmin.Endpoint.Models.System.Dic;

namespace SnippetAdmin.Controllers.System
{

    public interface IDicApi
    {
        /// <summary>
        /// 取得字典类型列表
        /// </summary>
        public Task<CommonResult<List<GetDicTypeListOutputModel>>> GetDicTypeList();

        /// <summary>
        /// 添加字典类型
        /// </summary>
        public Task<CommonResult> AddDicType(AddDicTypeInputModel inputModel);

        /// <summary>
        /// 更新字典类型
        /// </summary>
        public Task<CommonResult> UpdateDicType(UpdateDicTypeInputModel inputModel);

        /// <summary>
        /// 删除字典类型
        /// </summary>
        public Task<CommonResult> DeleteDicType(IdInputModel<int> inputModel);

        /// <summary>
        /// 取得字典项目列表
        /// </summary>
        public Task<CommonResult<List<GetDicValueListOutputModel>>> GetDicValueList(IdInputModel<int> inputModel);

        /// <summary>
        /// 取得字典项目列表
        /// </summary>
        public Task<CommonResult<List<GetDicValueListOutputModel>>> GetDicValueListByCode(IdInputModel<string> inputModel);

        /// <summary>
        /// 添加字典项目
        /// </summary>
        public Task<CommonResult> AddDicValueAsync(AddDicValueInputModel inputModel);

        /// <summary>
        /// 更新字典项目
        /// </summary>
        public Task<CommonResult> UpdateDicValueAsync(UpdateDicValueInputModel inputModel);

        /// <summary>
        /// 删除字典项目
        /// </summary>
        public Task<CommonResult> DeleteDicValue(IdInputModel<int> inputModel);
    }
}
