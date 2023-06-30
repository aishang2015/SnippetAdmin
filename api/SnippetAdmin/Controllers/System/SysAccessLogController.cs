using Microsoft.AspNetCore.Mvc;
using SnippetAdmin.CommonModel.Extensions;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Extensions;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Entity.System;
using SnippetAdmin.DynamicApi.Models;
using SnippetAdmin.DynamicApi.Templates;
using SnippetAdmin.Models.SysAccessLog;

namespace SnippetAdmin.Controllers.System
{
	[Route("api/[controller]")]
	[ApiController]
	[ApiExplorerSettings(GroupName = "v1")]
	public class SysAccessLogController : ControllerBase
	{

		private readonly SnippetAdminDbContext _snippetadmindbcontext;

		public SysAccessLogController(SnippetAdminDbContext _snippetadmindbcontext)
		{
			this._snippetadmindbcontext = _snippetadmindbcontext;
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
			if (dataQuery == null)
			{
				return CommonResult.Fail(MessageConstant.SHARDING_ERROR_001);
			}

			dataQuery = dataQuery.AndIfExist(inputModel.ContainedMethod, d => d.Method.Contains(inputModel.ContainedMethod))
				.AndIfExist(inputModel.EqualMethod, d => d.Method == inputModel.EqualMethod)
				.AndIfExist(inputModel.ContainedDescription, d => d.Description.Contains(inputModel.ContainedDescription))
				.AndIfExist(inputModel.EqualDescription, d => d.Description == inputModel.EqualDescription)
				.AndIfExist(inputModel.ContainedPath, d => d.Path.Contains(inputModel.ContainedPath))
				.AndIfExist(inputModel.EqualPath, d => d.Path == inputModel.EqualPath)
				.AndIfExist(inputModel.ContainedUsername, d => d.Username.Contains(inputModel.ContainedUsername))
				.AndIfExist(inputModel.EqualUsername, d => d.Username == inputModel.EqualUsername)
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
			var result = new PagedOutputModel<SysAccessLog>
			{
				Total = dataQuery.Count(),
				Data = dataQuery.Skip(inputModel.SkipCount).Take(inputModel.TakeCount).ToList()
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


	}
}