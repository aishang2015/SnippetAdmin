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
using SnippetAdmin.Core.Oauth;
using SnippetAdmin.Core.Oauth.Models;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Entity.RBAC;
using SnippetAdmin.Data.Entity.System;
using SnippetAdmin.Models;
using SnippetAdmin.Models.Account;

namespace SnippetAdmin.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<SnippetAdminUser> _userManager;

        private readonly IJwtFactory _jwtFactory;

        private readonly IMapper _mapper;

        private readonly JwtOption _jwtOption;

        private readonly IMemoryCache _memoryCache;

        private readonly SnippetAdminDbContext _dbContext;

        public AccountController(
            UserManager<SnippetAdminUser> userManager,
            IJwtFactory jwtFactory,
            IMapper mapper,
            IOptions<JwtOption> options,
            IMemoryCache memoryCache,
            SnippetAdminDbContext dbContext)
        {
            _userManager = userManager;
            _jwtFactory = jwtFactory;
            _mapper = mapper;
            _jwtOption = options.Value;
            _memoryCache = memoryCache;
            _dbContext = dbContext;
        }

        /// <summary>
        /// 登录操作
        /// </summary>
        [HttpPost]
        [CommonResultResponseType(typeof(CommonResult<LoginOutputModel>))]
        public async Task<CommonResult> Login([FromBody] LoginInputModel inputModel)
        {
            // 取得用户
            var user = await _userManager.FindByNameAsync(inputModel.UserName);
            if (user == null)
            {
                return this.FailCommonResult(MessageConstant.ACCOUNT_ERROR_0001);
            }

            // 检查密码
            var isValidPassword = await _userManager.CheckPasswordAsync(user, inputModel.Password);
            if (!isValidPassword)
            {
                return this.FailCommonResult(MessageConstant.ACCOUNT_ERROR_0001);
            }

            if (!user.IsActive)
            {
                return this.FailCommonResult(MessageConstant.ACCOUNT_ERROR_0012);
            }
            return await MakeLoginResultAsync(user);
        }

        /// <summary>
        /// 获取当前用户的信息
        /// </summary>
        [Authorize]
        [HttpPost]
        [CommonResultResponseType(typeof(CommonResult<UserInfoOutputModel>))]
        public async Task<CommonResult> GetCurrentUserInfo()
        {
            // 查找自己的信息
            var user = await _userManager.FindByNameAsync(User.UserName());

            // 返回结果
            return this.SuccessCommonResult(MessageConstant.EMPTYTUPLE,
                _mapper.Map<UserInfoOutputModel>(user));
        }

        /// <summary>
        /// 第三方登录
        /// </summary>
        [HttpPost]
        public async Task<CommonResult> ThirdPartyLogin(ThirdPartyLoginInputModel model,
            [FromServices] OauthHelper _oauthHelper)
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
                        return this.SuccessCommonResult(MessageConstant.ACCOUNT_INFO_0002,
                            new ThirdPartyLoginOutputModel(null, null, CommonConstant.Github, githubUserInfo.name, key));
                    }

                    // 用户未激活
                    if (!findUser.IsActive)
                    {
                        return this.FailCommonResult(MessageConstant.ACCOUNT_ERROR_0012);
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
                        return this.SuccessCommonResult(MessageConstant.ACCOUNT_INFO_0002,
                            new ThirdPartyLoginOutputModel(null, null, CommonConstant.Baidu, baiduUserInfo.uname, key));
                    }

                    // 用户未激活
                    if (!findUser.IsActive)
                    {
                        return this.FailCommonResult(MessageConstant.ACCOUNT_ERROR_0012);
                    }

                    // 根据找到的用户信息生成登录token
                    return await MakeLoginResultAsync(findUser);

                default:
                    return this.FailCommonResult(MessageConstant.ACCOUNT_ERROR_0004);
            }
        }

        /// <summary>
        /// 绑定第三方账号
        /// </summary>
        [HttpPost]
        [CommonResultResponseType(typeof(CommonResult<LoginOutputModel>))]
        public async Task<CommonResult> BindingThirdPartyAccount(BindingThirdPartyAccountInputModel inputModel)
        {
            // 检查用户信息
            var user = await _userManager.FindByNameAsync(inputModel.UserName);

            // 用户不存在
            if (user == null)
            {
                return this.FailCommonResult(MessageConstant.ACCOUNT_ERROR_0001);
            }

            // 检查密码
            var isValidPassword = await _userManager.CheckPasswordAsync(user, inputModel.Password);
            if (isValidPassword)
            {
                return this.FailCommonResult(MessageConstant.ACCOUNT_ERROR_0001);
            }

            // 用户未激活
            if (!user.IsActive)
            {
                return this.FailCommonResult(MessageConstant.ACCOUNT_ERROR_0012);
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
                        return this.FailCommonResult(MessageConstant.ACCOUNT_ERROR_0007);
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
                        return this.FailCommonResult(MessageConstant.ACCOUNT_ERROR_0007);
                    }

                default:
                    return this.FailCommonResult(MessageConstant.ACCOUNT_ERROR_0004);
            }
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        [HttpPost]
        [CommonResultResponseType(typeof(CommonResult<LoginOutputModel>))]
        public async Task<CommonResult> Refresh([FromBody] RefreshInputModel inputModel)
        {
            // 查找刷新凭证
            var refreshToken = _dbContext.RefreshTokens.FirstOrDefault(token => token.UserName == inputModel.UserName);
            if (refreshToken == null)
            {
                return this.FailCommonResult(MessageConstant.ACCOUNT_ERROR_0008);
            }

            // 账号被被人登录
            if (refreshToken.Content != inputModel.RefreshToken)
            {
                return this.FailCommonResult(MessageConstant.ACCOUNT_ERROR_0009);
            }

            // 刷新token已过期
            if (refreshToken.ExpireTime < DateTime.Now)
            {
                return this.FailCommonResult(MessageConstant.ACCOUNT_ERROR_0011);
            }

            // 验证token
            var userInfo = _jwtFactory.ValidToken(inputModel.JwtToken);
            if (userInfo == null)
            {
                return this.FailCommonResult(MessageConstant.ACCOUNT_ERROR_0010);
            }

            // 取得用户
            var user = await _userManager.FindByNameAsync(inputModel.UserName);
            if (user != null)
            {
                // 生成jwttoken
                var token = _jwtFactory.GenerateJwtToken(user.UserName);
                var identifies = await GetUserFrontRightsAsync(user);
                return this.SuccessCommonResult(
                    string.Empty, string.Empty,
                    new LoginOutputModel(token, user.UserName, _jwtOption.Expires, identifies, refreshToken.Content)
                ); ;
            }
            return this.FailCommonResult(MessageConstant.ACCOUNT_ERROR_0001);
        }

        /// <summary>
        /// 生成token返回结果
        /// </summary>
        private async Task<CommonResult> MakeLoginResultAsync(SnippetAdminUser user)
        {
            // 生成刷新token,移除旧token
            var refreshToken = _dbContext.RefreshTokens.FirstOrDefault(token => token.UserName == user.UserName);
            if (refreshToken != null)
            {
                _dbContext.Remove(refreshToken);
            }
            var tokenContent = Guid.NewGuid().ToString("N");
            _dbContext.RefreshTokens.Add(new RefreshToken
            {
                UserName = user.UserName,
                Content = tokenContent,
                CreatedTime = DateTime.Now,
                ExpireTime = DateTime.Now.AddHours(_jwtOption.RefreshExpireSpan),
            });
            _dbContext.SaveChanges();

            // 生成jwttoken
            var token = _jwtFactory.GenerateJwtToken(user.UserName);
            var identifies = await GetUserFrontRightsAsync(user);
            return this.SuccessCommonResult(
                MessageConstant.ACCOUNT_INFO_0001,
                new LoginOutputModel(token, user.UserName, _jwtOption.Expires, identifies, tokenContent)
            );
        }

        private async Task<string[]> GetUserFrontRightsAsync(SnippetAdminUser user)
        {
            // 取得前端页面元素权限
            var roles = await _userManager.GetRolesAsync(user);
            var elementIds = (from role in _dbContext.Roles
                              from element in _dbContext.Elements
                              from rc in _dbContext.RoleClaims
                              where
                                 role.IsActive &&
                                 element.Id.ToString() == rc.ClaimValue &&
                                 rc.ClaimType == ClaimConstant.RoleRight &&
                                 rc.RoleId == role.Id &&
                                 roles.Contains(role.Name)
                              select element.Id).Distinct().ToList();

            return (from element in _dbContext.Elements
                    from elementTree in _dbContext.ElementTrees
                    where element.Id == elementTree.Ancestor &&
                          elementIds.Contains(elementTree.Descendant)
                    select element.Identity).Distinct().ToArray();
        }
    }
}