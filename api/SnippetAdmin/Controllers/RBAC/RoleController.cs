using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Attributes;
using SnippetAdmin.Core.Utils;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Auth;
using SnippetAdmin.Data.Entity.RBAC;
using SnippetAdmin.Models;
using SnippetAdmin.Models.Common;
using SnippetAdmin.Models.RBAC.Role;

namespace SnippetAdmin.Controllers.RBAC
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [SnippetAdminAuthorize]
    [ApiExplorerSettings(GroupName = "v1")]
    public class RoleController : ControllerBase
    {
        private readonly SnippetAdminDbContext _dbContext;

        private readonly IMapper _mapper;

        public RoleController(SnippetAdminDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> ActiveRole([FromBody] ActiveRoleInputModel inputModel)
        {
            var role = await _dbContext.Roles.FindAsync(inputModel.Id);
            role.IsActive = inputModel.IsActive;
            _dbContext.Roles.Update(role);
            await _dbContext.SaveChangesAsync();
            return this.SuccessCommonResult(MessageConstant.ROLE_INFO_0004);
        }

        [HttpPost]
        [CommonResultResponseType(typeof(GetRoleOutputModel))]
        public async Task<CommonResult> GetRole([FromBody] IntIdInputModel inputModel)
        {
            var role = await _dbContext.Roles.FindAsync(inputModel.Id);
            var result = _mapper.Map<GetRoleOutputModel>(role);
            result.Rights = _dbContext.RoleClaims.Where(r => r.RoleId == inputModel.Id && r.ClaimType == ClaimConstant.RoleRight)
                .Select(r => int.Parse(r.ClaimValue)).ToArray();
            return this.SuccessCommonResult(result);
        }

        [HttpPost]
        [CommonResultResponseType(typeof(PagedOutputModel<GetRoleOutputModel>))]
        public async Task<CommonResult> GetRolesAsync([FromBody] PagedInputModel inputModel)
        {
            var roles = await _dbContext.Roles.Skip(inputModel.SkipCount)
                .Take(inputModel.TakeCount).ToListAsync();

            var result = new PagedOutputModel<GetRoleOutputModel>()
            {
                Total = _dbContext.Roles.Count(),
                Data = _mapper.Map<List<GetRoleOutputModel>>(roles)
            };

            return this.SuccessCommonResult(result);
        }

        [HttpPost]
        [CommonResultResponseType(typeof(List<DicOutputModel>))]
        public async Task<CommonResult> GetRoleDic()
        {
            var result = await _dbContext.Roles.Select(r => new DicOutputModel
            {
                Key = r.Id,
                Value = r.Name
            }).ToListAsync();

            return this.SuccessCommonResult(result);
        }

        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> AddOrUpdateRoleAsync([FromBody] AddOrUpdateRoleInputModel inputModel)
        {
            // 校验名称和代码重复
            if (_dbContext.Roles.Any(r => r.Id != inputModel.Id && r.Name == inputModel.Name))
            {
                return this.FailCommonResult(MessageConstant.ROLE_ERROR_0007);
            }

            using var trans = await _dbContext.Database.BeginTransactionAsync();

            // 保存角色信息
            var role = await _dbContext.Roles.FindAsync(inputModel.Id);
            if (role != null)
            {
                _mapper.Map(inputModel, role);
                _dbContext.Entry(role).Property(r => r.Code).IsModified = false;
                _dbContext.Roles.Update(role);
            }
            else
            {
                role = _mapper.Map<SnippetAdminRole>(inputModel);
                role.Code = GuidUtil.NewSequentialGuid().ToString("N");
                role = _dbContext.Roles.Add(role).Entity;
            }
            await _dbContext.SaveChangesAsync();

            // 保存权限信息
            // 清理旧权限
            var roleClaims = _dbContext.RoleClaims
                .Where(rc => rc.RoleId == role.Id && rc.ClaimType == ClaimConstant.RoleRight)
                .ToList();
            _dbContext.RoleClaims.RemoveRange(roleClaims);

            // 保存新权限
            if (inputModel.Rights != null && inputModel.Rights.Length != 0)
            {
                inputModel.Rights.ToList().ForEach(r =>
                {
                    _dbContext.RoleClaims.Add(new SnippetAdminRoleClaim
                    {
                        RoleId = role.Id,
                        ClaimType = ClaimConstant.RoleRight,
                        ClaimValue = r.ToString(),
                    });
                });

                await _dbContext.SaveChangesAsync();
            }
            await trans.CommitAsync();

            return this.SuccessCommonResult(MessageConstant.ROLE_INFO_0001);
        }

        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> RemoveRoleAsync([FromBody] IntIdInputModel inputModel)
        {
            var role = await _dbContext.Roles.FindAsync(inputModel.Id);
            _dbContext.Roles.Remove(role);
            await _dbContext.SaveChangesAsync();
            return this.SuccessCommonResult(MessageConstant.ROLE_INFO_0002);
        }
    }
}