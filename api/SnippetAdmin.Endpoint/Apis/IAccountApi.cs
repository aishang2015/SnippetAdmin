using SnippetAdmin.CommonModel;
using SnippetAdmin.Endpoint.Models.Account;

namespace SnippetAdmin.Controllers
{
    public interface IAccountApi
    {
        /// <summary>
        /// 登录操作
        /// </summary>
        public Task<CommonResult> Login(LoginInputModel inputModel);

        /// <summary>
        /// 获取当前用户的信息
        /// </summary>
        public Task<CommonResult> GetCurrentUserInfo();

        /// <summary>
        /// 第三方登录
        /// </summary>
        public Task<CommonResult> ThirdPartyLogin(ThirdPartyLoginInputModel model);

        /// <summary>
        /// 绑定第三方账号
        /// </summary>
        public Task<CommonResult> BindingThirdPartyAccount(BindingThirdPartyAccountInputModel inputModel);

        /// <summary>
        /// 刷新token
        /// </summary>
        public Task<CommonResult> Refresh(RefreshInputModel inputModel);
    }
}