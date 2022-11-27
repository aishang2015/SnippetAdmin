using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Attributes;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Entity.System;
using SnippetAdmin.Endpoint.Models.System.Dic;

namespace SnippetAdmin.Controllers.System
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Policy = "AccessApi")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class DicController : ControllerBase, IDicApi
    {
        private readonly SnippetAdminDbContext _dbContext;

        private readonly IMapper _mapper;

        public DicController(SnippetAdminDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>
        /// 取得字典类型列表
        /// </summary>
        [HttpPost]
        [CommonResultResponseType<List<GetDicTypeListOutputModel>>]
        public async Task<CommonResult<List<GetDicTypeListOutputModel>>> GetDicTypeList()
        {
            var dicTypeList = await _dbContext.SysDicTypes.ToListAsync();
            var result = _mapper.Map<List<GetDicTypeListOutputModel>>(dicTypeList);
            return CommonResult.Success(result);
        }

        /// <summary>
        /// 添加字典类型
        /// </summary>
        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> AddDicType(AddDicTypeInputModel inputModel)
        {
            if (_dbContext.SysDicTypes.Any(t => t.Name == inputModel.Name))
            {
                return CommonResult.Fail(MessageConstant.DICTIONARY_ERROR_0001);
            }

            if (_dbContext.SysDicTypes.Any(t => t.Code == inputModel.Code))
            {
                return CommonResult.Fail(MessageConstant.DICTIONARY_ERROR_0002);
            }

            var dicType = _mapper.Map<SysDicType>(inputModel);
            _dbContext.Add(dicType);
            await _dbContext.SaveChangesAsync();

            return CommonResult.Success(MessageConstant.DICTIONARY_INFO_0001);
        }

        /// <summary>
        /// 更新字典类型
        /// </summary>
        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> UpdateDicType(UpdateDicTypeInputModel inputModel)
        {
            if (_dbContext.SysDicTypes.Any(t => t.Name == inputModel.Name && t.Id != inputModel.Id))
            {
                return CommonResult.Fail(MessageConstant.DICTIONARY_ERROR_0001);
            }

            if (_dbContext.SysDicTypes.Any(t => t.Code == inputModel.Code && t.Id != inputModel.Id))
            {
                return CommonResult.Fail(MessageConstant.DICTIONARY_ERROR_0002);
            }

            var dicType = _mapper.Map<SysDicType>(inputModel);
            _dbContext.Update(dicType);
            await _dbContext.SaveChangesAsync();

            return CommonResult.Success(MessageConstant.DICTIONARY_INFO_0002);
        }

        /// <summary>
        /// 删除字典类型
        /// </summary>
        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> DeleteDicType(IdInputModel<int> inputModel)
        {
            var dicType = _dbContext.SysDicTypes.Find(inputModel.Id);
            _dbContext.Remove(dicType);
            var dicValueList = _dbContext.SysDicValues.Where(v => v.TypeId == inputModel.Id);
            _dbContext.RemoveRange(dicValueList);
            await _dbContext.SaveChangesAsync();
            return CommonResult.Success(MessageConstant.DICTIONARY_INFO_0003);
        }

        /// <summary>
        /// 取得字典项目列表
        /// </summary>
        [HttpPost]
        [CommonResultResponseType<List<GetDicValueListOutputModel>>]
        public async Task<CommonResult<List<GetDicValueListOutputModel>>> GetDicValueList(IdInputModel<int> inputModel)
        {
            var dicValueList = await _dbContext.SysDicValues.Where(v => v.TypeId == inputModel.Id)
                .OrderBy(v => v.Sorting).ToListAsync();
            var result = _mapper.Map<List<GetDicValueListOutputModel>>(dicValueList);
            return CommonResult.Success(result);
        }

        /// <summary>
        /// 取得字典项目列表
        /// </summary>
        [HttpPost]
        [CommonResultResponseType<List<GetDicValueListOutputModel>>]
        public async Task<CommonResult<List<GetDicValueListOutputModel>>> GetDicValueListByCode(IdInputModel<string> inputModel)
        {
            var dicValueList = from dicType in _dbContext.SysDicTypes
                               join dicValue in _dbContext.SysDicValues on dicType.Id equals dicValue.TypeId
                               where dicType.Code == inputModel.Id
                               select dicValue;
            var result = _mapper.Map<List<GetDicValueListOutputModel>>(await dicValueList.ToListAsync());
            return CommonResult.Success(result);
        }

        /// <summary>
        /// 添加字典项目
        /// </summary>
        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> AddDicValueAsync(AddDicValueInputModel inputModel)
        {
            if (_dbContext.SysDicValues.Any(t => t.Name == inputModel.Name && t.TypeId == inputModel.TypeId))
            {
                return CommonResult.Fail(MessageConstant.DICTIONARY_ERROR_0003);
            }

            if (_dbContext.SysDicValues.Any(t => t.Code == inputModel.Code && t.TypeId == inputModel.TypeId))
            {
                return CommonResult.Fail(MessageConstant.DICTIONARY_ERROR_0004);
            }

            var dicValue = _mapper.Map<SysDicValue>(inputModel);
            _dbContext.Add(dicValue);
            await _dbContext.SaveChangesAsync();

            return CommonResult.Success(MessageConstant.DICTIONARY_INFO_0004);
        }

        /// <summary>
        /// 更新字典项目
        /// </summary>
        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> UpdateDicValueAsync(UpdateDicValueInputModel inputModel)
        {
            if (_dbContext.SysDicValues.Any(t => t.Name == inputModel.Name
                && t.TypeId == inputModel.TypeId
                && t.Id != inputModel.Id))
            {
                return CommonResult.Fail(MessageConstant.DICTIONARY_ERROR_0003);
            }

            if (_dbContext.SysDicValues.Any(t => t.Code == inputModel.Code
                && t.TypeId == inputModel.TypeId
                && t.Id != inputModel.Id))
            {
                return CommonResult.Fail(MessageConstant.DICTIONARY_ERROR_0004);
            }

            var dicValue = _mapper.Map<SysDicValue>(inputModel);
            _dbContext.Update(dicValue);
            await _dbContext.SaveChangesAsync();

            return CommonResult.Success(MessageConstant.DICTIONARY_INFO_0005);
        }

        /// <summary>
        /// 删除字典项目
        /// </summary>
        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> DeleteDicValue(IdInputModel<int> inputModel)
        {
            var dicType = _dbContext.SysDicValues.Find(inputModel.Id);
            _dbContext.Remove(dicType);
            await _dbContext.SaveChangesAsync();
            return CommonResult.Success(MessageConstant.DICTIONARY_INFO_0006);
        }
    }
}
