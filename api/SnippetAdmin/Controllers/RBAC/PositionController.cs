using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Attributes;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Auth;
using SnippetAdmin.Data.Entity.RBAC;
using SnippetAdmin.Models;
using SnippetAdmin.Models.Common;
using SnippetAdmin.Models.RBAC.Position;

namespace SnippetAdmin.Controllers.RBAC
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [SnippetAdminAuthorize]
    [ApiExplorerSettings(GroupName = "v1")]
    public class PositionController : ControllerBase
    {
        private readonly SnippetAdminDbContext _dbContext;

        private readonly IMapper _mapper;

        public PositionController(SnippetAdminDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> AddOrUpdatePositionAsync(AddOrUpdatePositionInputModel inputModel)
        {
            // validate
            if (_dbContext.Positions.Any(p => p.Id != inputModel.Id && p.Name == inputModel.Name))
            {
                return this.FailCommonResult(MessageConstant.POSITION_ERROR_0001);
            }
            if (_dbContext.Positions.Any(p => p.Id != inputModel.Id && p.Code == inputModel.Code))
            {
                return this.FailCommonResult(MessageConstant.POSITION_ERROR_0002);
            }

            var position = _dbContext.Positions.Find(inputModel.Id);
            if (position == null)
            {
                _dbContext.Positions.Add(new Position()
                {
                    Name = inputModel.Name,
                    Code = inputModel.Code
                });
            }
            else
            {
                position.Name = inputModel.Name;
                position.Code = inputModel.Code;
                _dbContext.Positions.Update(position);
            }
            await _dbContext.SaveChangesAsync();

            return this.SuccessCommonResult(MessageConstant.POSITION_INFO_0001);
        }

        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> DeletePositionAsync(DeletePositionInputModel inputModel)
        {
            var position = _dbContext.Positions.Find(inputModel.Id);
            var userClaims = _dbContext.UserClaims.Where(uc => uc.ClaimValue == inputModel.Id.ToString() &&
                uc.ClaimType == ClaimConstant.UserPosition);

            _dbContext.Remove(position);
            _dbContext.RemoveRange(userClaims);
            await _dbContext.SaveChangesAsync();

            return this.SuccessCommonResult(MessageConstant.POSITION_INFO_0002);
        }

        [HttpPost]
        [CommonResultResponseType(typeof(GetPositionOutputModel))]
        public CommonResult GetPosition([FromBody] IntIdInputModel inputModel)
        {
            var positoin = _dbContext.Positions.Find(inputModel.Id);
            return this.SuccessCommonResult(new GetPositionOutputModel
            {
                Id = positoin.Id,
                Name = positoin.Name,
                Code = positoin.Code
            });
        }

        [HttpPost]
        [CommonResultResponseType(typeof(PagedOutputModel<GetPositionsOutputModel>))]
        public async Task<CommonResult> GetPositions([FromBody] PagedInputModel inputModel)
        {
            var query = _dbContext.Positions.AsQueryable();
            query = query.Sort(inputModel.Sorts);

            var result = new PagedOutputModel<GetPositionsOutputModel>()
            {
                Total = query.Count(),
                Data = await query.Select(p => new GetPositionsOutputModel()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Code = p.Code,
                }).Skip(inputModel.SkipCount).Take(inputModel.TakeCount).ToListAsync()
            };
            return this.SuccessCommonResult(result);
        }

        [HttpPost]
        [CommonResultResponseType(typeof(List<DicOutputModel>))]
        public async Task<CommonResult> GetPositionDic()
        {
            var result = await _dbContext.Positions.Select(r => new DicOutputModel
            {
                Key = r.Id.ToString(),
                Value = r.Name
            }).ToListAsync();

            return this.SuccessCommonResult(result);
        }
    }
}
