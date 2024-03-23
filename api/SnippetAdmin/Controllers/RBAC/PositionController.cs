using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.CommonModel.Extensions;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Attributes;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Entity.Rbac;
using SnippetAdmin.Endpoint.Apis.RBAC;
using SnippetAdmin.Endpoint.Models.RBAC.Position;
using System.ComponentModel;

namespace SnippetAdmin.Controllers.RBAC
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize(Policy = "AccessApi")]
	[ApiExplorerSettings(GroupName = "v1")]
	public class PositionController : ControllerBase, IPositionApi
	{
		private readonly SnippetAdminDbContext _dbContext;

		private readonly IMapper _mapper;

		public PositionController(SnippetAdminDbContext dbContext, IMapper mapper)
		{
			_dbContext = dbContext;
			_mapper = mapper;
		}

		/// <summary>
		/// 创建或添加一个职位
		/// </summary>
		[HttpPost]
		[CommonResultResponseType]
		[Description("创建或添加一个职位")]
        [AccessLog("职位管理", "创建或添加一个职位")]
        public async Task<CommonResult> AddOrUpdatePositionAsync(AddOrUpdatePositionInputModel inputModel)
		{
			// validate
			if (_dbContext.RbacPositions.Any(p => p.Id != inputModel.Id && p.Name == inputModel.Name))
			{
				return CommonResult.Fail(MessageConstant.POSITION_ERROR_0001);
			}
			if (_dbContext.RbacPositions.Any(p => p.Id != inputModel.Id && p.Code == inputModel.Code))
			{
				return CommonResult.Fail(MessageConstant.POSITION_ERROR_0002);
			}

			var position = _dbContext.RbacPositions.Find(inputModel.Id);
			if (position == null)
			{
				_dbContext.RbacPositions.Add(new RbacPosition()
				{
					Name = inputModel.Name,
					Code = inputModel.Code,
					Sorting = inputModel.Sorting
				});
			}
			else
			{
				position.Name = inputModel.Name;
				position.Code = inputModel.Code;
				position.Sorting = inputModel.Sorting;
			}
			await _dbContext.AuditSaveChangesAsync();

			return CommonResult.Success(MessageConstant.POSITION_INFO_0001);
		}

		/// <summary>
		/// 删除一个职位
		/// </summary>
		[HttpPost]
		[CommonResultResponseType]
		[Description("删除一个职位")]
        [AccessLog("职位管理", "删除一个职位")]
        public async Task<CommonResult> DeletePositionAsync(DeletePositionInputModel inputModel)
		{
			var position = _dbContext.RbacPositions.Find(inputModel.Id);
			var userClaims = _dbContext.UserClaims.Where(uc => uc.ClaimValue == inputModel.Id.ToString() &&
				uc.ClaimType == ClaimConstant.UserPosition);

			_dbContext.Remove(position);
			_dbContext.RemoveRange(userClaims);
			await _dbContext.AuditSaveChangesAsync();

			return CommonResult.Success(MessageConstant.POSITION_INFO_0002);
		}

		/// <summary>
		/// 取得职位信息
		/// </summary>
		[HttpPost]
		[CommonResultResponseType<GetPositionOutputModel>]
		[Description("取得职位信息")]
        public async Task<CommonResult<GetPositionOutputModel>> GetPosition([FromBody] IdInputModel<int> inputModel)
		{
			var positoin = await _dbContext.RbacPositions.FindAsync(inputModel.Id);
			return CommonResult.Success(new GetPositionOutputModel
			{
				Id = positoin.Id,
				Name = positoin.Name,
				Code = positoin.Code,
				Sorting = positoin.Sorting,
			});
		}

		/// <summary>
		/// 查询职位列表
		/// </summary>
		[HttpPost]
		[CommonResultResponseType<PagedOutputModel<GetPositionsOutputModel>>]
		[Description("查询职位列表")]
		public async Task<CommonResult<PagedOutputModel<GetPositionsOutputModel>>> GetPositions([FromBody] PagedInputModel inputModel)
		{
			var query = _dbContext.RbacPositions.OrderBy(p => p.Sorting).AsQueryable();
			query = query.Sort(inputModel.Sorts);

			var result = new PagedOutputModel<GetPositionsOutputModel>()
			{
				Total = query.Count(),
				Data = await query.Select(p => new GetPositionsOutputModel()
				{
					Id = p.Id,
					Name = p.Name,
					Code = p.Code,
					Sorting = p.Sorting
				}).Skip(inputModel.SkipCount).Take(inputModel.TakeCount).ToListAsync()
			};
			return CommonResult.Success(result);
		}

		/// <summary>
		/// 取得职位字典
		/// </summary>
		[HttpPost]
		[CommonResultResponseType<List<DicOutputModel<int>>>]
		[Description("取得职位字典")]
		public async Task<CommonResult<List<DicOutputModel<int>>>> GetPositionDic()
		{
			var result = await _dbContext.RbacPositions.Select(r => new DicOutputModel<int>
			{
				Key = r.Id,
				Value = r.Name
			}).ToListAsync();

			return CommonResult.Success(result);
		}
	}
}
