using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Attributes;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Entity.Rbac;
using SnippetAdmin.Endpoint.Apis.RBAC;
using SnippetAdmin.Endpoint.Models.RBAC.Role;

namespace SnippetAdmin.Controllers.RBAC
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Policy = "AccessApi")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class RoleController : ControllerBase, IRoleApi
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
            return CommonResult.Success(MessageConstant.ROLE_INFO_0004);
        }

        [HttpPost]
        [CommonResultResponseType(typeof(GetRoleOutputModel))]
        public async Task<CommonResult<GetRoleOutputModel>> GetRole([FromBody] IdInputModel<int> inputModel)
        {
            var role = await _dbContext.Roles.FindAsync(inputModel.Id);
            var result = _mapper.Map<GetRoleOutputModel>(role);
            result.Rights = _dbContext.RoleClaims.Where(r => r.RoleId == inputModel.Id && r.ClaimType == ClaimConstant.RoleRight)
                .Select(r => int.Parse(r.ClaimValue)).ToArray();
            return CommonResult.Success(result);
        }

        [HttpPost]
        [CommonResultResponseType(typeof(PagedOutputModel<GetRoleOutputModel>))]
        public async Task<CommonResult<PagedOutputModel<GetRoleOutputModel>>> GetRolesAsync([FromBody] PagedInputModel inputModel)
        {
            var roles = await _dbContext.Roles.Skip(inputModel.SkipCount)
                .Take(inputModel.TakeCount).ToListAsync();

            var result = new PagedOutputModel<GetRoleOutputModel>()
            {
                Total = _dbContext.Roles.Count(),
                Data = _mapper.Map<List<GetRoleOutputModel>>(roles)
            };

            return CommonResult.Success(result);
        }

        [HttpPost]
        [CommonResultResponseType(typeof(List<DicOutputModel<int>>))]
        public async Task<CommonResult<List<DicOutputModel<int>>>> GetRoleDic()
        {
            var result = await _dbContext.Roles.Select(r => new DicOutputModel<int>
            {
                Key = r.Id,
                Value = r.Name
            }).ToListAsync();

            return CommonResult.Success(result);
        }

        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> AddOrUpdateRoleAsync([FromBody] AddOrUpdateRoleInputModel inputModel)
        {
            // 校验名称和代码重复
            if (_dbContext.Roles.Any(r => r.Id != inputModel.Id && r.Name == inputModel.Name))
            {
                return CommonResult.Fail(MessageConstant.ROLE_ERROR_0007);
            }
            if (_dbContext.Roles.Any(r => r.Id != inputModel.Id && r.Code == inputModel.Code))
            {
                return CommonResult.Fail(MessageConstant.ROLE_ERROR_0008);
            }

            using var trans = await _dbContext.Database.BeginTransactionAsync();

            // 保存角色信息
            var role = await _dbContext.Roles.FindAsync(inputModel.Id);
            if (role != null)
            {
                _mapper.Map(inputModel, role);
                _dbContext.Roles.Update(role);
            }
            else
            {
                role = _mapper.Map<RbacRole>(inputModel);
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
                    _dbContext.RoleClaims.Add(new RbacRoleClaim
                    {
                        RoleId = role.Id,
                        ClaimType = ClaimConstant.RoleRight,
                        ClaimValue = r.ToString(),
                    });
                });

                await _dbContext.SaveChangesAsync();
            }
            await trans.CommitAsync();

            return CommonResult.Success(MessageConstant.ROLE_INFO_0001);
        }

        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> RemoveRoleAsync([FromBody] IdInputModel<int> inputModel)
        {
            var role = await _dbContext.Roles.FindAsync(inputModel.Id);
            _dbContext.Roles.Remove(role);
            await _dbContext.SaveChangesAsync();
            return CommonResult.Success(MessageConstant.ROLE_INFO_0002);
        }
    }
}