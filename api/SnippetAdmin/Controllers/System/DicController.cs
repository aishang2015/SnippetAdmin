using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Attributes;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Entity.System;
using SnippetAdmin.Models;
using SnippetAdmin.Models.Common;
using SnippetAdmin.Models.System.Dic;

namespace SnippetAdmin.Controllers.System
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class DicController : ControllerBase
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
        [CommonResultResponseType(typeof(List<GetDicTypeListOutputModel>))]
        public CommonResult GetDicTypeList()
        {
            var dicTypeList = _dbContext.SysDicTypes.ToList();
            var result = _mapper.Map<List<GetDicTypeListOutputModel>>(dicTypeList);
            return this.SuccessCommonResult(result);
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
                return this.FailCommonResult(MessageConstant.DICTIONARY_ERROR_0001);
            }

            if (_dbContext.SysDicTypes.Any(t => t.Code == inputModel.Code))
            {
                return this.FailCommonResult(MessageConstant.DICTIONARY_ERROR_0002);
            }

            var dicType = _mapper.Map<SysDicType>(inputModel);
            _dbContext.Add(dicType);
            await _dbContext.SaveChangesAsync();

            return this.SuccessCommonResult(MessageConstant.DICTIONARY_INFO_0001);
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
                return this.FailCommonResult(MessageConstant.DICTIONARY_ERROR_0001);
            }

            if (_dbContext.SysDicTypes.Any(t => t.Code == inputModel.Code && t.Id != inputModel.Id))
            {
                return this.FailCommonResult(MessageConstant.DICTIONARY_ERROR_0002);
            }

            var dicType = _mapper.Map<SysDicType>(inputModel);
            _dbContext.Update(dicType);
            await _dbContext.SaveChangesAsync();

            return this.SuccessCommonResult(MessageConstant.DICTIONARY_INFO_0002);
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
            return this.SuccessCommonResult(MessageConstant.DICTIONARY_INFO_0003);
        }

        /// <summary>
        /// 取得字典项目列表
        /// </summary>
        [HttpPost]
        [CommonResultResponseType(typeof(List<GetDicValueListOutputModel>))]
        public CommonResult GetDicValueList(IdInputModel<int> inputModel)
        {
            var dicValueList = _dbContext.SysDicValues.Where(v => v.TypeId == inputModel.Id)
                .OrderBy(v => v.Sorting).ToList();
            var result = _mapper.Map<List<GetDicValueListOutputModel>>(dicValueList);
            return this.SuccessCommonResult(result);
        }

        /// <summary>
        /// 取得字典项目列表
        /// </summary>
        [HttpPost]
        [CommonResultResponseType(typeof(List<GetDicValueListOutputModel>))]
        public CommonResult GetDicValueListByCode(IdInputModel<string> inputModel)
        {
            var dicValueList = from dicType in _dbContext.SysDicTypes
                               join dicValue in _dbContext.SysDicValues on dicType.Id equals dicValue.TypeId
                               where dicType.Code == inputModel.Id
                               select dicValue;
            var result = _mapper.Map<List<GetDicValueListOutputModel>>(dicValueList);
            return this.SuccessCommonResult(result);
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
                return this.FailCommonResult(MessageConstant.DICTIONARY_ERROR_0003);
            }

            if (_dbContext.SysDicValues.Any(t => t.Code == inputModel.Code && t.TypeId == inputModel.TypeId))
            {
                return this.FailCommonResult(MessageConstant.DICTIONARY_ERROR_0004);
            }

            var dicValue = _mapper.Map<SysDicValue>(inputModel);
            _dbContext.Add(dicValue);
            await _dbContext.SaveChangesAsync();

            return this.SuccessCommonResult(MessageConstant.DICTIONARY_INFO_0004);
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
                return this.FailCommonResult(MessageConstant.DICTIONARY_ERROR_0003);
            }

            if (_dbContext.SysDicValues.Any(t => t.Code == inputModel.Code
                && t.TypeId == inputModel.TypeId
                && t.Id != inputModel.Id))
            {
                return this.FailCommonResult(MessageConstant.DICTIONARY_ERROR_0004);
            }

            var dicValue = _mapper.Map<SysDicValue>(inputModel);
            _dbContext.Update(dicValue);
            await _dbContext.SaveChangesAsync();

            return this.SuccessCommonResult(MessageConstant.DICTIONARY_INFO_0005);
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
            return this.SuccessCommonResult(MessageConstant.DICTIONARY_INFO_0006);
        }
    }
}
