using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Caching.Memory;
using SnippetAdmin.CommonModel.Extensions;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Attributes;
using SnippetAdmin.Core.Extensions;
using SnippetAdmin.Core.Helpers;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Entity.System;
using SnippetAdmin.DynamicApi.Models;
using SnippetAdmin.DynamicApi.Templates;
using SnippetAdmin.Endpoint.Models.System.AccessLog;
using SnippetAdmin.Models.SysAccessLog;
using System.Reflection;

namespace SnippetAdmin.Controllers.System
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class SysAccessLogController : ControllerBase
    {

        private readonly SnippetAdminDbContext _snippetadmindbcontext;

        private readonly IMapper _mapper;

        public SysAccessLogController(SnippetAdminDbContext _snippetadmindbcontext, IMapper mapper)
        {
            this._snippetadmindbcontext = _snippetadmindbcontext;
            _mapper = mapper;
        }

        [HttpPost("FindOne")]
        public CommonResult FindOne([FromBody] IdInputModel<int> inputModel)
        {
            var result = _snippetadmindbcontext.Set<SysAccessLog>().Find(inputModel.Id);
            return CommonResult.Success(result);
        }

        [HttpPost("GetMany")]
        public CommonResult GetMany([FromBody] DynamicSearchInputModel inputModel)
        {
            var dataQuery = _snippetadmindbcontext.SysAccessLogs.AsQueryable();
            if (dataQuery == null)
            {
                return CommonResult.Fail(MessageConstant.SYSTEM_ERROR_005);
            }
            dataQuery = inputModel.GetFilterExpression(dataQuery);
            dataQuery = dataQuery.Sort(inputModel.Sorts);
            var result = new PagedOutputModel<SysAccessLog>
            {
                Total = dataQuery.Count(),
                Data = dataQuery.Skip(inputModel.SkipCount).Take(inputModel.TakeCount).ToList()
            };
            return CommonResult.Success(result);
        }

        [HttpPost("GetMany2")]
        public CommonResult GetMany2([FromBody] GetSysAccessLogInputModel inputModel)
        {
            var dataQuery = _snippetadmindbcontext.SysAccessLogs.AsQueryable();
            dataQuery = dataQuery
               .AndIfExist(inputModel.ContainedModule, d => d.Module.Contains(inputModel.ContainedModule))
               .AndIfExist(inputModel.EqualModule, d => d.Module == inputModel.EqualModule)
               .AndIfExist(inputModel.ContainedMethod, d => d.Method.Contains(inputModel.ContainedMethod))
               .AndIfExist(inputModel.EqualMethod, d => d.Method == inputModel.EqualMethod)
               .AndIfExist(inputModel.ContainedPath, d => d.Path.Contains(inputModel.ContainedPath))
               .AndIfExist(inputModel.EqualPath, d => d.Path == inputModel.EqualPath)
               .AndIfExist(inputModel.UpperUserId, d => d.UserId <= inputModel.UpperUserId)
               .AndIfExist(inputModel.LowerUserId, d => d.UserId >= inputModel.LowerUserId)
               .AndIfExist(inputModel.EqualUserId, d => d.UserId == inputModel.EqualUserId)
               .AndIfExist(inputModel.ContainedRemoteIp, d => d.RemoteIp.Contains(inputModel.ContainedRemoteIp))
               .AndIfExist(inputModel.EqualRemoteIp, d => d.RemoteIp == inputModel.EqualRemoteIp)
               .AndIfExist(inputModel.UpperAccessedTime, d => d.AccessedTime <= inputModel.UpperAccessedTime)
               .AndIfExist(inputModel.LowerAccessedTime, d => d.AccessedTime >= inputModel.LowerAccessedTime)
               .AndIfExist(inputModel.EqualAccessedTime, d => d.AccessedTime == inputModel.EqualAccessedTime)
               .AndIfExist(inputModel.UpperElapsedTime, d => d.ElapsedTime <= inputModel.UpperElapsedTime)
               .AndIfExist(inputModel.LowerElapsedTime, d => d.ElapsedTime >= inputModel.LowerElapsedTime)
               .AndIfExist(inputModel.EqualElapsedTime, d => d.ElapsedTime == inputModel.EqualElapsedTime)
               .AndIfExist(inputModel.ContainedRequestBody, d => d.RequestBody.Contains(inputModel.ContainedRequestBody))
               .AndIfExist(inputModel.EqualRequestBody, d => d.RequestBody == inputModel.EqualRequestBody)
               .AndIfExist(inputModel.UpperStatusCode, d => d.StatusCode <= inputModel.UpperStatusCode)
               .AndIfExist(inputModel.LowerStatusCode, d => d.StatusCode >= inputModel.LowerStatusCode)
               .AndIfExist(inputModel.EqualStatusCode, d => d.StatusCode == inputModel.EqualStatusCode)
               .AndIfExist(inputModel.ContainedResponseBody, d => d.ResponseBody.Contains(inputModel.ContainedResponseBody))
               .AndIfExist(inputModel.EqualResponseBody, d => d.ResponseBody == inputModel.EqualResponseBody);
            dataQuery = dataQuery.Sort(inputModel.Sorts);

            var dataResult = dataQuery.Skip(inputModel.SkipCount).Take(inputModel.TakeCount).ToList();
            var mapperedData = _mapper.Map<List<GetSysAccessLogOutputModel>>(dataResult);

            var userSet = _snippetadmindbcontext.Users;
            mapperedData.ForEach(u => u.RealName = userSet.FirstOrDefault(user => user.Id == u.UserId)?.RealName);

            var result = new PagedOutputModel<GetSysAccessLogOutputModel>
            {
                Total = dataQuery.Count(),
                Data = mapperedData
            };
            return CommonResult.Success(result);
        }

        [HttpPost("AddOne")]
        public async Task<CommonResult> AddOne([FromBody] SysAccessLog entity)
        {
            _snippetadmindbcontext.Set<SysAccessLog>().Add(entity);
            await _snippetadmindbcontext.SaveChangesAsync();
            return CommonResult.Success(DynamicMessageConstants.DYNAMIC_INFO_003);
        }

        [HttpPost("AddMany")]
        public async Task<CommonResult> AddOne([FromBody] IEnumerable<SysAccessLog> data)
        {
            _snippetadmindbcontext.Set<SysAccessLog>().AddRange(data);
            await _snippetadmindbcontext.SaveChangesAsync();
            return CommonResult.Success(DynamicMessageConstants.DYNAMIC_INFO_003);
        }

        [HttpPost("UpdateOne")]
        public async Task<CommonResult> UpdateOne([FromBody] SysAccessLog entity)
        {
            _snippetadmindbcontext.Set<SysAccessLog>().Update(entity);
            await _snippetadmindbcontext.SaveChangesAsync();
            return CommonResult.Success(DynamicMessageConstants.DYNAMIC_INFO_002);
        }

        [HttpPost("DeleteOne")]
        public async Task<CommonResult> DeleteOne([FromBody] IdInputModel<int> inputModel)
        {
            var result = _snippetadmindbcontext.Set<SysAccessLog>().Find(inputModel.Id);
            _snippetadmindbcontext.Remove(result);
            await _snippetadmindbcontext.SaveChangesAsync();
            return CommonResult.Success(DynamicMessageConstants.DYNAMIC_INFO_001);
        }


        [HttpPost("GetModuleMethodDic")]
        [CommonResultResponseType<Dictionary<string, IEnumerable<string>>>]
        public CommonResult GetModuleMethodDic([FromServices] IMemoryCache cache)
        {
            var result = cache.GetOrCreate(nameof(AccessLogAttribute), cacheEntry =>
            {
                var attributes = ReflectionHelper.GetSubClass<ControllerBase>()
                    .SelectMany(c => c.GetMethods())
                    .Where(m => m.IsPublic)
                    .Select(c => c.GetCustomAttribute<AccessLogAttribute>()).Where(a => a != null)
                    .GroupBy(a => a.Module)
                    .Select(g => new KeyValuePair<string, IEnumerable<string>>(g.Key, g.Select(ga => ga.Method).Distinct()));
                return new Dictionary<string, IEnumerable<string>>(attributes);
            });

            return CommonResult.Success(result);
        }

        [HttpPost("GetDataDetailLogs")]
        [CommonResultResponseType<List<GetDataDetailLogsOutputModel>>]
        public CommonResult GetDataDetailLogs(GetDataDetailLogsInputModel inputModel)
        {
            var designModel = _snippetadmindbcontext.GetService<IDesignTimeModel>().Model;

            var dataLogs = _snippetadmindbcontext.SysDataLogs.Where(l => l.TraceIdentifier == inputModel.TraceIdentifier)
                 .ToList();
            var dataLogIdList = dataLogs.Select(l => l.Id).ToList();
            var dataDetailLogs = _snippetadmindbcontext.SysDataLogDetails.Where(l => dataLogIdList.Contains(l.DataLogId))
                .ToList();

            var result = new List<GetDataDetailLogsOutputModel>();
            foreach (var dataLog in dataLogs)
            {
                var designEntity = designModel.FindEntityType(dataLog.EntityName);

                var detail = new GetDataDetailLogsOutputModel();
                detail.OperateTime = dataLog.OperateTime;
                detail.Operation = dataLog.Operation switch
                {
                    2 => "删除数据",
                    3 => "修改数据",
                    4 => "添加数据"
                };
                detail.EntityName = designEntity?.GetComment();
                detail.DataDetailList = new();

                foreach (var dataDetailLog in dataDetailLogs.Where(l => l.DataLogId == dataLog.Id))
                {
                    var propertyComment = designEntity!.FindDeclaredProperty(dataDetailLog.PropertyName).GetComment();
                    if (propertyComment == null)
                    {
                        continue;
                    }

                    detail.DataDetailList.Add(new DataDetailModel
                    {
                        OldValue = dataDetailLog.OldValue,
                        NewValue = dataDetailLog.NewValue,
                        PropertyName = propertyComment
                    });
                }
                result.Add(detail);
            }

            return CommonResult.Success(result);
        }
    }
}