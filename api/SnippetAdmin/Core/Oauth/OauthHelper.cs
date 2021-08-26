using Microsoft.Extensions.Options;
using SnippetAdmin.Core.Oauth.Apis;
using SnippetAdmin.Core.Oauth.Models;
using System.Threading.Tasks;

namespace SnippetAdmin.Core.Oauth
{
    public class OauthHelper
    {
        private readonly OauthOption _oauthOption;

        private readonly IGithubAuthApi _githubAuthApi;

        private readonly IGithubApi _githubApi;

        private readonly IBaiduApi _baiduApi;

        public OauthHelper(
            IOptions<OauthOption> options,
            IGithubAuthApi githubAuthApi,
            IGithubApi githubApi,
            IBaiduApi baiduApi)
        {
            _oauthOption = options.Value;
            _githubAuthApi = githubAuthApi;
            _githubApi = githubApi;
            _baiduApi = baiduApi;
        }

        #region Github

        public async Task<GithubUserInfo> GetGithubUserInfoAsync(string code)
        {
            // https://docs.github.com/en/developers/apps/authorizing-oauth-apps
            var responseObj = await _githubAuthApi.GetAccessTokenAsync(
                _oauthOption.Github.AppId, _oauthOption.Github.AppSecret, code);

            return await _githubApi.GetUserAsync($"token {responseObj.access_token}");
        }

        #endregion Github

        #region Baidu

        public async Task<BaiduUserInfo> GetBaiduUserInfoAsync(string code)
        {
            // http://developer.baidu.com/wiki/index.php?title=docs/oauth/authorization
            var responseObj = await _baiduApi.GetAccessTokenAsync(code,
                _oauthOption.Baidu.AppId, _oauthOption.Baidu.AppSecret, _oauthOption.Baidu.RedirectUri);

            return await _baiduApi.GetUserAsync(responseObj.access_token);
        }

        #endregion Baidu
    }
}