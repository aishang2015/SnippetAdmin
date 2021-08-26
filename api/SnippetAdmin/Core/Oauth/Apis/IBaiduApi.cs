using Refit;
using SnippetAdmin.Core.Oauth.Models;
using System.Threading.Tasks;

namespace SnippetAdmin.Core.Oauth.Apis
{
    [Headers("User-Agent:Refit", "Accept:application/json")]
    public interface IBaiduApi
    {
        [Post("/oauth/2.0/token?grant_type=authorization_code&code={code}&client_id={appId}&client_secret={appSecret}&redirect_uri={redirectUri}")]
        public Task<BaiduAccessTokenResponse> GetAccessTokenAsync(string code, string appId, string appSecret, string redirectUri);

        [Post("/rest/2.0/passport/users/getLoggedInUser?access_token={access_token}")]
        public Task<BaiduUserInfo> GetUserAsync(string access_token);
    }
}