using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Attribute;
using SnippetAdmin.Core.Method;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Auth;
using SnippetAdmin.Data.Entity.RBAC;
using SnippetAdmin.Models;
using SnippetAdmin.Models.Common;
using SnippetAdmin.Models.RBAC.User;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnippetAdmin.Controllers.RBAC
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [SnippetAdminAuthorize]
    public class UserController : ControllerBase
    {
        private readonly SnippetAdminDbContext _dbContext;

        private readonly IMapper _mapper;

        public UserController(SnippetAdminDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> ActiveUserAsync([FromBody] ActiveUserInputModel inputModel)
        {
            var user = await _dbContext.Users.FindAsync(inputModel.Id);
            user.IsActive = inputModel.IsActive;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            return this.SuccessCommonResult(MessageConstant.USER_INFO_0001);
        }

        [HttpPost]
        [CommonResultResponseType(typeof(GetUserOutputModel))]
        public async Task<CommonResult> GetUserAsync([FromBody] IntIdInputModel inputModel)
        {
            var user = await _dbContext.Users.FindAsync(inputModel.Id);
            var result = _mapper.Map<GetUserOutputModel>(user);
            result.Roles = (from ur in _dbContext.UserRoles
                            where ur.UserId == inputModel.Id
                            select ur.RoleId).ToArray();
            return this.SuccessCommonResult(result);
        }

        [HttpPost]
        [CommonResultResponseType(typeof(PagedOutputModel<SearchUserOutputModel>))]
        public CommonResult SearchUser([FromBody] SearchUserInputModel inputModel)
        {
            // 普通条件
            var query = _dbContext.Users
                .AndIfExist(inputModel.UserName, u => u.UserName.Contains(inputModel.UserName))
                .AndIfExist(inputModel.RealName, u => u.RealName.Contains(inputModel.RealName))
                .AndIfExist(inputModel.Phone, u => u.PhoneNumber.Contains(inputModel.Phone));

            // 角色过滤
            if (inputModel.Role != null)
            {
                query = from u in query
                        join ur in _dbContext.UserRoles on u.Id equals ur.UserId
                        where ur.RoleId == inputModel.Role
                        select u;
            }

            // 组织过滤
            if (inputModel.Org != null)
            {
                query = from u in query
                        where _dbContext.UserOrganizationPositions.Any(uop =>
                            uop.UserId == u.Id && uop.OrganizationId == inputModel.Org)
                        select u;
            }

            // 查询数据
            var resultQuery = from u in query
                              select new SearchUserOutputModel
                              {
                                  Id = u.Id,
                                  Avatar = u.Avatar,
                                  Gender = (int)u.Gender,
                                  IsActive = u.IsActive,
                                  PhoneNumber = u.PhoneNumber,
                                  RealName = u.RealName,
                                  UserName = u.UserName,
                                  Roles = (from ur in _dbContext.UserRoles
                                           join r in _dbContext.Roles on ur.RoleId equals r.Id
                                           where ur.UserId == u.Id
                                           select new RoleInfo
                                           {
                                               RoleName = r.Name,
                                               IsActive = r.IsActive
                                           }).ToArray(),
                                  OrgPositions = (from uop in _dbContext.UserOrganizationPositions
                                                  join o in _dbContext.Organizations on uop.OrganizationId equals o.Id
                                                  join p in _dbContext.Positions on uop.PositionId equals p.Id
                                                  where uop.UserId == u.Id
                                                  select new OrgPositionOutputModel
                                                  {
                                                      Org = o.Name,
                                                      Position = p.Name
                                                  }).ToArray()
                              };

            var result = new PagedOutputModel<SearchUserOutputModel>
            {
                Total = resultQuery.Count(),
                Data = resultQuery.Skip(inputModel.SkipCount).Take(inputModel.TakeCount)
            };

            return this.SuccessCommonResult(result);
        }

        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> AddOrUpdateUserAsync([FromBody] AddOrUpdateUserInputModel inputModel,
            [FromServices] UserManager<SnippetAdminUser> userManager)
        {
            if (_dbContext.Users.Any(u => u.UserName == inputModel.UserName && u.Id != inputModel.Id))
            {
                return this.FailCommonResult(MessageConstant.USER_ERROR_0012);
            }

            using var trans = await _dbContext.Database.BeginTransactionAsync();

            if (inputModel.Id != null)
            {
                var user = _dbContext.Users.Find(inputModel.Id);
                _mapper.Map(inputModel, user);
                _dbContext.Users.Update(user);

                var ur = _dbContext.UserRoles.Where(ur => ur.UserId == user.Id).ToList();
                _dbContext.UserRoles.RemoveRange(ur);
                foreach (var role in inputModel.Roles)
                {
                    _dbContext.UserRoles.Add(new IdentityUserRole<int> { UserId = user.Id, RoleId = role });
                }
            }
            else
            {
                var user = _mapper.Map<SnippetAdminUser>(inputModel);
                await userManager.CreateAsync(user);
                await userManager.AddPasswordAsync(user, "123456");
                if (inputModel.Roles != null)
                {
                    foreach (var role in inputModel.Roles)
                    {
                        _dbContext.UserRoles.Add(new IdentityUserRole<int> { UserId = user.Id, RoleId = role });
                    }
                }
            }
            await _dbContext.SaveChangesAsync();
            await trans.CommitAsync();
            return this.SuccessCommonResult(MessageConstant.USER_INFO_0001);
        }

        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> RemoveUserAsync([FromBody] IntIdInputModel inputModel)
        {
            var user = _dbContext.Users.Find(inputModel.Id);
            var uops = _dbContext.UserOrganizationPositions.Where(u => u.UserId == inputModel.Id).ToList();
            _dbContext.Remove(user);
            _dbContext.RemoveRange(uops);
            await _dbContext.SaveChangesAsync();
            return this.SuccessCommonResult(MessageConstant.USER_INFO_0001);
        }

        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> SetUserPasswordAsync([FromBody] SetUserPasswordInputModel inputModel,
            [FromServices] UserManager<SnippetAdminUser> userManager)
        {
            if (inputModel.Password != inputModel.ConfirmPassword)
            {
                return this.FailCommonResult(MessageConstant.USER_ERROR_0011);
            }

            var user = _dbContext.Users.Find(inputModel.Id);
            await userManager.RemovePasswordAsync(user);
            await userManager.AddPasswordAsync(user, inputModel.Password);
            return this.SuccessCommonResult(MessageConstant.USER_INFO_0003);
        }

        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> AddOrgMemberAsync([FromBody] AddOrgMemberInputModel inputModel)
        {
            foreach (var user in inputModel.UserIds)
            {
                if (inputModel.Positions != null)
                {
                    foreach (var position in inputModel.Positions)
                    {
                        _dbContext.UserOrganizationPositions.Add(new UserOrganizationPosition
                        {
                            UserId = user,
                            OrganizationId = inputModel.OrgId,
                            PositionId = position
                        });
                    }
                }
                else
                {
                    _dbContext.UserOrganizationPositions.Add(new UserOrganizationPosition
                    {
                        UserId = user,
                        OrganizationId = inputModel.OrgId,
                        PositionId = null
                    });
                }
            }
            await _dbContext.SaveChangesAsync();
            return this.SuccessCommonResult(MessageConstant.USER_INFO_0004);
        }

        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> RemoveOrgMemberAsync([FromBody] RemoveOrgMemberInputModel inputModel)
        {
            var uops = _dbContext.UserOrganizationPositions.Where(uop => uop.OrganizationId == inputModel.OrgId &&
                uop.UserId == inputModel.UserId).ToList(); ;
            _dbContext.UserOrganizationPositions.RemoveRange(uops);
            await _dbContext.SaveChangesAsync();

            return this.SuccessCommonResult(MessageConstant.USER_INFO_0004);
        }
    }
}