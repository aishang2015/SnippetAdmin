using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Attributes;
using SnippetAdmin.Core.Authentication;
using SnippetAdmin.Core.Extensions;
using SnippetAdmin.Core.FileStore;
using SnippetAdmin.Core.Helpers;
using SnippetAdmin.Core.Oauth;
using SnippetAdmin.Core.Oauth.Models;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Entity.Rbac;
using SnippetAdmin.Endpoint.Models.Account;
using System.Security.Claims;

namespace SnippetAdmin.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[ApiExplorerSettings(GroupName = "v1")]
	public class AccountController : ControllerBase, IAccountApi
	{
		private readonly UserManager<RbacUser> _userManager;

		private readonly IJwtFactory _jwtFactory;

		private readonly IMapper _mapper;

		private readonly JwtOption _jwtOption;

		private readonly IMemoryCache _memoryCache;

		private readonly SnippetAdminDbContext _dbContext;

		private readonly OauthHelper _oauthHelper;

		public AccountController(
			UserManager<RbacUser> userManager,
			IJwtFactory jwtFactory,
			IMapper mapper,
			IOptions<JwtOption> options,
			IMemoryCache memoryCache,
			SnippetAdminDbContext dbContext,
			OauthHelper oauthHelper)
		{
			_userManager = userManager;
			_jwtFactory = jwtFactory;
			_mapper = mapper;
			_jwtOption = options.Value;
			_memoryCache = memoryCache;
			_dbContext = dbContext;
			_oauthHelper = oauthHelper;
		}

		/// <summary>
		/// 登录操作
		/// </summary>
		[HttpPost]
		[CommonResultResponseType<LoginOutputModel>]
		public async Task<CommonResult> Login([FromBody] LoginInputModel inputModel)
		{
			// 取得用户
			var user = await _userManager.FindByNameAsync(inputModel.UserName);
			if (user == null)
			{
				return CommonResult.Fail(MessageConstant.ACCOUNT_ERROR_0001);
			}

			// 检查密码
			var isValidPassword = await _userManager.CheckPasswordAsync(user, inputModel.Password);
			if (!isValidPassword)
			{
				return CommonResult.Fail(MessageConstant.ACCOUNT_ERROR_0001);
			}

			if (!user.IsActive)
			{
				return CommonResult.Fail(MessageConstant.ACCOUNT_ERROR_0012);
			}
			return await MakeLoginResultAsync(user);
		}

		/// <summary>
		/// 获取当前用户的信息
		/// </summary>
		[Authorize]
		[HttpPost]
		[CommonResultResponseType<UserInfoOutputModel>]
		public async Task<CommonResult> GetCurrentUserInfo()
		{
			// 查找自己的信息
			var user = await _userManager.FindByNameAsync(User.GetUserName());

			// 返回结果
			return CommonResult.Success(MessageConstant.EMPTYTUPLE,
				_mapper.Map<UserInfoOutputModel>(user));
		}

		/// <summary>
		/// 第三方登录
		/// </summary>
		[HttpPost]
		public async Task<CommonResult> ThirdPartyLogin(ThirdPartyLoginInputModel model)
		{
			switch (model.Source)
			{
				case CommonConstant.Github:
					var githubUserInfo = await _oauthHelper.GetGithubUserInfoAsync(model.Code);
					var findUser = _userManager.Users.FirstOrDefault(u => u.GithubId == githubUserInfo.id);

					// 没有发现用户，需要绑定信息
					if (findUser == null)
					{
						// 将第三方用户信息存入缓存
						var key = Guid.NewGuid().ToString("N");
						_memoryCache.Set(key, githubUserInfo, TimeSpan.FromMinutes(5));

						// 让前端进入账号绑定页面
						return CommonResult.Success(MessageConstant.ACCOUNT_INFO_0002,
							new ThirdPartyLoginOutputModel(null, null, CommonConstant.Github, githubUserInfo.name, key));
					}

					// 用户未激活
					if (!findUser.IsActive)
					{
						return CommonResult.Fail(MessageConstant.ACCOUNT_ERROR_0012);
					}

					// 根据找到的用户信息生成登录token
					return await MakeLoginResultAsync(findUser);

				case CommonConstant.Baidu:
					var baiduUserInfo = await _oauthHelper.GetBaiduUserInfoAsync(model.Code);
					findUser = _userManager.Users.FirstOrDefault(u => u.BaiduId == baiduUserInfo.openid);

					// 没有发现用户，需要绑定信息
					if (findUser == null)
					{
						// 将第三方用户信息存入缓存
						var key = Guid.NewGuid().ToString("N");
						_memoryCache.Set(key, baiduUserInfo, TimeSpan.FromMinutes(5));

						// 让前端进入账号绑定页面
						return CommonResult.Success(MessageConstant.ACCOUNT_INFO_0002,
							new ThirdPartyLoginOutputModel(null, null, CommonConstant.Baidu, baiduUserInfo.uname, key));
					}

					// 用户未激活
					if (!findUser.IsActive)
					{
						return CommonResult.Fail(MessageConstant.ACCOUNT_ERROR_0012);
					}

					// 根据找到的用户信息生成登录token
					return await MakeLoginResultAsync(findUser);

				default:
					return CommonResult.Fail(MessageConstant.ACCOUNT_ERROR_0004);
			}
		}

		/// <summary>
		/// 绑定第三方账号
		/// </summary>
		[HttpPost]
		[CommonResultResponseType<LoginOutputModel>]
		public async Task<CommonResult> BindingThirdPartyAccount(BindingThirdPartyAccountInputModel inputModel)
		{
			// 检查用户信息
			var user = await _userManager.FindByNameAsync(inputModel.UserName);

			// 用户不存在
			if (user == null)
			{
				return CommonResult.Fail(MessageConstant.ACCOUNT_ERROR_0001);
			}

			// 检查密码
			var isValidPassword = await _userManager.CheckPasswordAsync(user, inputModel.Password);
			if (isValidPassword)
			{
				return CommonResult.Fail(MessageConstant.ACCOUNT_ERROR_0001);
			}

			// 用户未激活
			if (!user.IsActive)
			{
				return CommonResult.Fail(MessageConstant.ACCOUNT_ERROR_0012);
			}

			// 账号验证通过则绑定用户的第三方账号信息
			switch (inputModel.ThirdPartyType)
			{
				case CommonConstant.Github:
					var githubUserInfo = _memoryCache.Get<GithubUserInfo>(inputModel.ThirdPartyInfoCacheKey);
					if (githubUserInfo != null)
					{
						user.GithubId = githubUserInfo.id;
						await _userManager.UpdateAsync(user);
						return await MakeLoginResultAsync(user);
					}
					else
					{
						return CommonResult.Fail(MessageConstant.ACCOUNT_ERROR_0007);
					}

				case CommonConstant.Baidu:
					var baiduUserInfo = _memoryCache.Get<BaiduUserInfo>(inputModel.ThirdPartyInfoCacheKey);
					if (baiduUserInfo != null)
					{
						user.BaiduId = baiduUserInfo.openid;
						await _userManager.UpdateAsync(user);
						return await MakeLoginResultAsync(user);
					}
					else
					{
						return CommonResult.Fail(MessageConstant.ACCOUNT_ERROR_0007);
					}

				default:
					return CommonResult.Fail(MessageConstant.ACCOUNT_ERROR_0004);
			}
		}

		/// <summary>
		/// 刷新token
		/// </summary>
		[HttpPost]
		[CommonResultResponseType<LoginOutputModel>]
		public async Task<CommonResult> Refresh([FromBody] RefreshInputModel inputModel)
		{
			// 验证token
			var userInfo = _jwtFactory.ValidToken(inputModel.JwtToken);
			if (userInfo == null)
			{
				return CommonResult.Fail(MessageConstant.ACCOUNT_ERROR_0010);
			}

			// 取得用户
			var user = await _userManager.FindByNameAsync(inputModel.UserName);
			if (user != null)
			{
				// 生成jwttoken
				var token = _jwtFactory.GenerateJwtToken(
					new List<(string, string)> {
						(ClaimTypes.Name, user.UserName),
						(ClaimTypes.NameIdentifier, user.Id.ToString())
					});
				var identifies = await GetUserFrontRightsAsync(user);
				return CommonResult.Success(
					string.Empty, string.Empty,
					new LoginOutputModel(token, user.UserName, _jwtOption.Expires, identifies)
				);
			}
			return CommonResult.Fail(MessageConstant.ACCOUNT_ERROR_0001);
		}



		/// <summary>
		/// 上传用户头像
		/// </summary>
		[Authorize]
		[HttpPost]
		public async Task<CommonResult> UploadAvatar([FromForm] IFormFile avatar,
			[FromServices] IFileStoreService fileStoreService,
			[FromServices] SnippetAdminDbContext db)
		{
			var user = db.Users.FirstOrDefault(u => u.UserName == User.GetUserName());

			if (!string.IsNullOrEmpty(user.Avatar))
			{
				await fileStoreService.DeleteFileAsync(user.Avatar);
			}

			var newFileName = GuidHelper.NewSequentialGuid().ToString("N") + "." + avatar.FileName.Split('.').Last();
			await fileStoreService.SaveFromStreamAsync(avatar.OpenReadStream(), newFileName);

			user.Avatar = newFileName;
			db.Users.Update(user);
			await db.SaveChangesAsync();

			return CommonResult.Success(newFileName);
		}

		/// <summary>
		/// 更新用户信息
		/// </summary>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		public async Task<CommonResult> UpdateUserInfo([FromBody] UpdateUserInfoInputModel model,
			[FromServices] SnippetAdminDbContext db)
		{
			var user = db.Users.FirstOrDefault(u => u.UserName == User.GetUserName());
			user.PhoneNumber = model.PhoneNumber;
			db.Update(user);
			await db.SaveChangesAsync();
			return CommonResult.Success(MessageConstant.SYSTEM_INFO_002);
		}

		/// <summary>
		/// 修改密码
		/// </summary>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		public async Task<CommonResult> ModifyPassword([FromBody] ModifyPasswordInputModel model,
			[FromServices] SnippetAdminDbContext db,
			[FromServices] UserManager<RbacUser> userManager)
		{
			var user = db.Users.FirstOrDefault(u => u.UserName == User.GetUserName());
			var isValidPwd = await userManager.CheckPasswordAsync(user, model.OldPassword);
			if (!isValidPwd)
			{
				return CommonResult.Fail(MessageConstant.ACCOUNT_ERROR_0013);
			}
			await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
			return CommonResult.Success(MessageConstant.ACCOUNT_INFO_0003);
		}

		/// <summary>
		/// 生成token返回结果
		/// </summary>
		private async Task<CommonResult> MakeLoginResultAsync(RbacUser user)
		{
			// 生成jwttoken
			var token = _jwtFactory.GenerateJwtToken(
					new List<(string, string)> {
						(ClaimTypes.Name, user.UserName),
						(ClaimTypes.NameIdentifier, user.Id.ToString())
					});
			var identifies = await GetUserFrontRightsAsync(user);
			return CommonResult.Success(
				MessageConstant.ACCOUNT_INFO_0001,
				new LoginOutputModel(token, user.UserName, _jwtOption.Expires, identifies)
			);
		}

		private async Task<string[]> GetUserFrontRightsAsync(RbacUser user)
		{
			// 取得前端页面元素权限
			var roles = await _userManager.GetRolesAsync(user);
			var elementIds = (from role in _dbContext.Roles
							  from element in _dbContext.RbacElements
							  from rc in _dbContext.RoleClaims
							  where
								 role.IsActive &&
								 element.Id.ToString() == rc.ClaimValue &&
								 rc.ClaimType == ClaimConstant.RoleRight &&
								 rc.RoleId == role.Id &&
								 roles.Contains(role.Name)
							  select element.Id).Distinct().ToList();

			return (from element in _dbContext.RbacElements
					from elementTree in _dbContext.RbacElementTrees
					where element.Id == elementTree.Ancestor &&
						  elementIds.Contains(elementTree.Descendant)
					select element.Identity).Distinct().ToArray();
		}
	}
}