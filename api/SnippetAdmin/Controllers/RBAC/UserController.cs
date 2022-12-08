using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Attributes;
using SnippetAdmin.Core.Extensions;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Entity.Rbac;
using SnippetAdmin.Endpoint.Apis.RBAC;
using SnippetAdmin.Endpoint.Models.RBAC.User;
using System.ComponentModel;

namespace SnippetAdmin.Controllers.RBAC
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize(Policy = "AccessApi")]
	[ApiExplorerSettings(GroupName = "v1")]
	public class UserController : ControllerBase, IUserApi
	{
		private readonly SnippetAdminDbContext _dbContext;

		private readonly IMapper _mapper;

		private readonly UserManager<RbacUser> _userManager;

		public UserController(SnippetAdminDbContext dbContext, IMapper mapper, UserManager<RbacUser> userManager)
		{
			_dbContext = dbContext;
			_mapper = mapper;
			_userManager = userManager;
		}

		/// <summary>
		/// 用户激活
		/// </summary>
		[HttpPost]
		[CommonResultResponseType]
		[Description("用户激活")]
		public async Task<CommonResult> ActiveUserAsync([FromBody] ActiveUserInputModel inputModel)
		{
			var user = await _dbContext.Users.FindAsync(inputModel.Id);
			user.IsActive = inputModel.IsActive;
			_dbContext.Users.Update(user);
			await _dbContext.SaveChangesAsync();
			return CommonResult.Success(MessageConstant.USER_INFO_0001);
		}

		/// <summary>
		/// 取得用户详细信息
		/// </summary>
		[HttpPost]
		[CommonResultResponseType<GetUserOutputModel>]
		[Description("取得用户详细信息")]
		public async Task<CommonResult<GetUserOutputModel>> GetUserAsync([FromBody] IdInputModel<int> inputModel)
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
			return CommonResult.Success(result);
		}

		/// <summary>
		/// 查询用户信息
		/// </summary>
		[HttpPost]
		[CommonResultResponseType<PagedOutputModel<SearchUserOutputModel>>]
		[Description("查询用户信息")]
		public Task<CommonResult<PagedOutputModel<SearchUserOutputModel>>> SearchUser([FromBody] SearchUserInputModel inputModel)
		{
			var userQuery = _dbContext.CacheSet<RbacUser>().AsQueryable().OrderBy(u => u.Id);
			var roleQuery = _dbContext.CacheSet<RbacRole>().AsQueryable().OrderBy(u => u.Id);
			var userRoleQuery = _dbContext.CacheSet<RbacUserRole>().AsQueryable();
			var userClaimQuery = _dbContext.CacheSet<RbacUserClaim>().AsQueryable();
			var organizationQuery = _dbContext.CacheSet<RbacOrganization>().AsQueryable();
			var positionQuery = _dbContext.CacheSet<RbacPosition>().AsQueryable();

			// 普通条件
			var query = userQuery
				.AndIfExist(inputModel.UserName, u => u.UserName.Contains(inputModel.UserName))
				.AndIfExist(inputModel.RealName, u => u.RealName.Contains(inputModel.RealName))
				.AndIfExist(inputModel.Phone, u => u.PhoneNumber.Contains(inputModel.Phone));

			// 角色过滤
			if (inputModel.Role != null)
			{
				query = from u in query
						join ur in userRoleQuery on u.Id equals ur.UserId
						where ur.RoleId == inputModel.Role
						select u;
			}

			// 组织过滤
			if (inputModel.Org != null)
			{
				query = from u in query
						where userClaimQuery.Any(userClaim =>
							userClaim.UserId == u.Id && userClaim.ClaimValue == inputModel.Org.ToString() &&
							userClaim.ClaimType == ClaimConstant.UserOrganization)
						select u;
			}

			// 组织过滤
			if (inputModel.Position != null)
			{
				query = from u in query
						where userClaimQuery.Any(userClaim =>
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
								  Roles = (from ur in userRoleQuery
										   join r in roleQuery on ur.RoleId equals r.Id
										   where ur.UserId == u.Id
										   select new RoleInfo
										   {
											   RoleName = r.Name,
											   IsActive = r.IsActive
										   }).ToArray(),
								  Organizations = (from uc in userClaimQuery
												   join org in organizationQuery on uc.ClaimValue equals org.Id.ToString()
												   where uc.UserId == u.Id && uc.ClaimType == ClaimConstant.UserOrganization
												   select org.Name).ToArray(),
								  Positions = (from uc in userClaimQuery
											   join pos in positionQuery on uc.ClaimValue equals pos.Id.ToString()
											   where uc.UserId == u.Id && uc.ClaimType == ClaimConstant.UserPosition
											   select pos.Name).ToArray()
							  };

			var result = new PagedOutputModel<SearchUserOutputModel>
			{
				Total = resultQuery.Count(),
				Data = resultQuery.Skip(inputModel.SkipCount).Take(inputModel.TakeCount).ToList()
			};

			return Task.FromResult(CommonResult.Success(result));
		}

		/// <summary>
		/// 添加或删除用户
		/// </summary>
		[HttpPost]
		[CommonResultResponseType]
		[Description("添加或删除用户")]
		public async Task<CommonResult> AddOrUpdateUserAsync([FromBody] AddOrUpdateUserInputModel inputModel)
		{
			if (_dbContext.Users.Any(u => u.UserName == inputModel.UserName && u.Id != inputModel.Id))
			{
				return CommonResult.Fail(MessageConstant.USER_ERROR_0012);
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
				await _dbContext.SaveChangesAsync();
			}
			else
			{
				user = _mapper.Map<RbacUser>(inputModel);
				await _userManager.CreateAsync(user);
				await _userManager.AddPasswordAsync(user, "123456");
			}

			inputModel.Roles?.ToList().ForEach(role =>
				_dbContext.UserRoles.Add(new RbacUserRole { UserId = user.Id, RoleId = role })
			);

			inputModel.Organizations?.ToList().ForEach(organization =>
				_dbContext.UserClaims.Add(new RbacUserClaim
				{
					UserId = user.Id,
					ClaimValue = organization.ToString(),
					ClaimType = ClaimConstant.UserOrganization
				}));
			inputModel.Positions?.ToList().ForEach(position =>
				_dbContext.UserClaims.Add(new RbacUserClaim
				{
					UserId = user.Id,
					ClaimValue = position.ToString(),
					ClaimType = ClaimConstant.UserPosition
				}));

			await _dbContext.SaveChangesAsync();
			await trans.CommitAsync();
			return CommonResult.Success(MessageConstant.USER_INFO_0001);
		}

		/// <summary>
		/// 删除一个用户
		/// </summary>
		[HttpPost]
		[CommonResultResponseType]
		[Description("删除一个用户")]
		public async Task<CommonResult> RemoveUserAsync([FromBody] IdInputModel<int> inputModel)
		{
			var user = _dbContext.Users.Find(inputModel.Id);
			var uops = _dbContext.UserClaims.Where(u => u.UserId == inputModel.Id).ToList();
			_dbContext.Remove(user);
			_dbContext.RemoveRange(uops);
			await _dbContext.SaveChangesAsync();
			return CommonResult.Success(MessageConstant.USER_INFO_0001);
		}

		/// <summary>
		/// 设置用户密码
		/// </summary>
		[HttpPost]
		[CommonResultResponseType]
		[Description("设置用户密码")]
		public async Task<CommonResult> SetUserPasswordAsync([FromBody] SetUserPasswordInputModel inputModel)
		{
			if (inputModel.Password != inputModel.ConfirmPassword)
			{
				return CommonResult.Fail(MessageConstant.USER_ERROR_0011);
			}

			var user = _dbContext.Users.Find(inputModel.Id);
			await _userManager.RemovePasswordAsync(user);
			await _userManager.AddPasswordAsync(user, inputModel.Password);
			return CommonResult.Success(MessageConstant.USER_INFO_0003);
		}

		/// <summary>
		/// 添加组织成员
		/// </summary>
		[HttpPost]
		[CommonResultResponseType]
		[Description("添加组织成员")]
		public async Task<CommonResult> AddOrgMemberAsync([FromBody] AddOrgMemberInputModel inputModel)
		{
			foreach (var userId in inputModel.UserIds)
			{
				_dbContext.UserClaims.Add(new RbacUserClaim
				{
					UserId = userId,
					ClaimType = ClaimConstant.UserOrganization,
					ClaimValue = inputModel.OrgId.ToString(),
				});
			}
			await _dbContext.SaveChangesAsync();
			return CommonResult.Success(MessageConstant.USER_INFO_0004);
		}

		/// <summary>
		/// 删除组织成员
		/// </summary>
		[HttpPost]
		[CommonResultResponseType]
		[Description("删除组织成员")]
		public async Task<CommonResult> RemoveOrgMemberAsync([FromBody] RemoveOrgMemberInputModel inputModel)
		{
			var uops = _dbContext.UserClaims.Where(uop => uop.ClaimValue == inputModel.OrgId.ToString() &&
				uop.UserId == inputModel.UserId && uop.ClaimType == ClaimConstant.UserOrganization).ToList();
			_dbContext.RemoveRange(uops);
			await _dbContext.SaveChangesAsync();

			return CommonResult.Success(MessageConstant.USER_INFO_0004);
		}
	}
}