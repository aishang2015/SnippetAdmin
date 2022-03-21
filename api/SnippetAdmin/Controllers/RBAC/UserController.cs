using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Attributes;
using SnippetAdmin.Core.Method;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Auth;
using SnippetAdmin.Data.Entity.RBAC;
using SnippetAdmin.Models;
using SnippetAdmin.Models.Common;
using SnippetAdmin.Models.RBAC.User;

namespace SnippetAdmin.Controllers.RBAC
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [SnippetAdminAuthorize]
    [ApiExplorerSettings(GroupName = "v1")]
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
            result.Organizations = (from uc in _dbContext.UserClaims
                                    where uc.UserId == inputModel.Id &&
                                        uc.ClaimType == ClaimConstant.UserOrganization
                                    select uc.ClaimValue).ToArray().Select(d => int.Parse(d)).ToArray();
            result.Positions = (from uc in _dbContext.UserClaims
                                where uc.UserId == inputModel.Id &&
                                    uc.ClaimType == ClaimConstant.UserPosition
                                select uc.ClaimValue).ToArray().Select(d => int.Parse(d)).ToArray();
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
                        where _dbContext.UserClaims.Any(userClaim =>
                            userClaim.UserId == u.Id && userClaim.ClaimValue == inputModel.Org.ToString() &&
                            userClaim.ClaimType == ClaimConstant.UserOrganization)
                        select u;
            }

            // 组织过滤
            if (inputModel.Position != null)
            {
                query = from u in query
                        where _dbContext.UserClaims.Any(userClaim =>
                            userClaim.UserId == u.Id && userClaim.ClaimValue == inputModel.Position.ToString() &&
                            userClaim.ClaimType == ClaimConstant.UserPosition)
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
                                  Organizations = (from uc in _dbContext.UserClaims
                                                   join org in _dbContext.Organizations on uc.ClaimValue equals org.Id.ToString()
                                                   where uc.UserId == u.Id && uc.ClaimType == ClaimConstant.UserOrganization
                                                   select org.Name).ToArray(),
                                  Positions = (from uc in _dbContext.UserClaims
                                               join pos in _dbContext.Positions on uc.ClaimValue equals pos.Id.ToString()
                                               where uc.UserId == u.Id && uc.ClaimType == ClaimConstant.UserPosition
                                               select pos.Name).ToArray()
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
            var user = _dbContext.Users.Find(inputModel.Id);

            if (user != null)
            {
                _mapper.Map(inputModel, user);
                _dbContext.Users.Update(user);

                var ur = _dbContext.UserRoles.Where(ur => ur.UserId == user.Id).ToList();
                _dbContext.UserRoles.RemoveRange(ur);

                var ups = _dbContext.UserClaims.Where(uc => uc.UserId == user.Id &&
                    (uc.ClaimType == ClaimConstant.UserPosition || uc.ClaimType == ClaimConstant.UserOrganization)).ToList();
                _dbContext.UserClaims.RemoveRange(ups);

            }
            else
            {
                user = _mapper.Map<SnippetAdminUser>(inputModel);
                await userManager.CreateAsync(user);
                await userManager.AddPasswordAsync(user, "123456");
            }

            inputModel.Roles?.ToList().ForEach(role =>
                _dbContext.UserRoles.Add(new SnippetAdminUserRole { UserId = user.Id, RoleId = role })
            );

            inputModel.Organizations?.ToList().ForEach(organization =>
                _dbContext.UserClaims.Add(new SnippetAdminUserClaim
                {
                    UserId = user.Id,
                    ClaimValue = organization.ToString(),
                    ClaimType = ClaimConstant.UserOrganization
                }));
            inputModel.Positions?.ToList().ForEach(position =>
                _dbContext.UserClaims.Add(new SnippetAdminUserClaim
                {
                    UserId = user.Id,
                    ClaimValue = position.ToString(),
                    ClaimType = ClaimConstant.UserPosition
                }));

            await _dbContext.SaveChangesAsync();
            await trans.CommitAsync();
            return this.SuccessCommonResult(MessageConstant.USER_INFO_0001);
        }

        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> RemoveUserAsync([FromBody] IntIdInputModel inputModel)
        {
            var user = _dbContext.Users.Find(inputModel.Id);
            var uops = _dbContext.UserClaims.Where(u => u.UserId == inputModel.Id).ToList();
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
            foreach (var userId in inputModel.UserIds)
            {
                _dbContext.UserClaims.Add(new SnippetAdminUserClaim
                {
                    UserId = userId,
                    ClaimType = ClaimConstant.UserOrganization,
                    ClaimValue = inputModel.OrgId.ToString(),
                });
            }
            await _dbContext.SaveChangesAsync();
            return this.SuccessCommonResult(MessageConstant.USER_INFO_0004);
        }

        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> RemoveOrgMemberAsync([FromBody] RemoveOrgMemberInputModel inputModel)
        {
            var uops = _dbContext.UserClaims.Where(uop => uop.ClaimValue == inputModel.OrgId.ToString() &&
                uop.UserId == inputModel.UserId && uop.ClaimType == ClaimConstant.UserOrganization).ToList();
            _dbContext.RemoveRange(uops);
            await _dbContext.SaveChangesAsync();

            return this.SuccessCommonResult(MessageConstant.USER_INFO_0004);
        }
    }
}