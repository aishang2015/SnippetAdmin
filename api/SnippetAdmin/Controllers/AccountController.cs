using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Attribute;
using SnippetAdmin.Core.Authentication;
using SnippetAdmin.Core.Method;
using SnippetAdmin.Core.Oauth;
using SnippetAdmin.Core.Oauth.Models;
using SnippetAdmin.Core.UserAccessor;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Entity.RBAC;
using SnippetAdmin.Models;
using SnippetAdmin.Models.Account;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SnippetAdmin.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<SnippetAdminUser> _userManager;

        private readonly OauthHelper _oauthHelper;

        private readonly IJwtFactory _jwtFactory;

        private readonly IMapper _mapper;

        private readonly IDistributedCache _cache;

        private readonly JwtOption _jwtOption;

        private readonly IUserAccessor _userAccessor;

        private readonly SnippetAdminDbContext _dbContext;

        public AccountController(
            UserManager<SnippetAdminUser> userManager,
            OauthHelper oauthHttpClient,
            IJwtFactory jwtFactory,
            IMapper mapper,
            IDistributedCache cache,
            IOptions<JwtOption> options,
            IUserAccessor userAccessor,
            SnippetAdminDbContext dbContext)
        {
            _userManager = userManager;
            _oauthHelper = oauthHttpClient;
            _jwtFactory = jwtFactory;
            _mapper = mapper;
            _cache = cache;
            _jwtOption = options.Value;
            _userAccessor = userAccessor;
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
            if (user != null)
            {
                // 检查密码
                var isValidPassword = await _userManager.CheckPasswordAsync(user, inputModel.Password);
                if (isValidPassword)
                {
                    return await MakeLoginResultAsync(user);
                }
            }
            return this.FailCommonResult(MessageConstant.ACCOUNT_ERROR_0001);
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
            var user = await _userManager.FindByNameAsync(_userAccessor.UserName);

            // 返回结果
            return this.SuccessCommonResult(MessageConstant.EMPTYTUPLE,
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
                        await _cache.SetObjectAsync(key, githubUserInfo, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                        });

                        // 让前端进入账号绑定页面
                        return this.SuccessCommonResult(MessageConstant.ACCOUNT_INFO_0002,
                            new ThirdPartyLoginOutputModel(null, null, CommonConstant.Github, githubUserInfo.name, key));
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
                        await _cache.SetObjectAsync(key, baiduUserInfo, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                        });

                        // 让前端进入账号绑定页面
                        return this.SuccessCommonResult(MessageConstant.ACCOUNT_INFO_0002,
                            new ThirdPartyLoginOutputModel(null, null, CommonConstant.Baidu, baiduUserInfo.uname, key));
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
            if (user != null)
            {
                // 检查密码
                var isValidPassword = await _userManager.CheckPasswordAsync(user, inputModel.Password);
                if (isValidPassword)
                {
                    // 账号验证通过则绑定用户的第三方账号信息
                    switch (inputModel.ThirdPartyType)
                    {
                        case CommonConstant.Github:
                            var githubUserInfo = await _cache.GetObjectAsync<GithubUserInfo>(inputModel.ThirdPartyInfoCacheKey);
                            if (githubUserInfo != null)
                            {
                                user.GithubId = githubUserInfo.id;
                                await _userManager.UpdateAsync(user);
                            }
                            else
                            {
                                return this.FailCommonResult(MessageConstant.ACCOUNT_ERROR_0007);
                            }
                            break;

                        case CommonConstant.Baidu:
                            var baiduUserInfo = await _cache.GetObjectAsync<BaiduUserInfo>(inputModel.ThirdPartyInfoCacheKey);
                            if (baiduUserInfo != null)
                            {
                                user.BaiduId = baiduUserInfo.openid;
                                await _userManager.UpdateAsync(user);
                            }
                            else
                            {
                                return this.FailCommonResult(MessageConstant.ACCOUNT_ERROR_0007);
                            }
                            break;

                        default:
                            return this.FailCommonResult(MessageConstant.ACCOUNT_ERROR_0004);
                    }

                    return await MakeLoginResultAsync(user);
                }
            }
            return this.FailCommonResult(MessageConstant.ACCOUNT_ERROR_0001);
        }

        /// <summary>
        /// 生成token返回结果
        /// </summary>
        private async Task<CommonResult> MakeLoginResultAsync(SnippetAdminUser user)
        {
            var token = _jwtFactory.GenerateJwtToken(user.UserName);
            var identifies = await GetUserFrontRightsAsync(user);
            return this.SuccessCommonResult(
                MessageConstant.ACCOUNT_INFO_0001,
                new LoginOutputModel(token, user.UserName, _jwtOption.Expires, identifies)
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