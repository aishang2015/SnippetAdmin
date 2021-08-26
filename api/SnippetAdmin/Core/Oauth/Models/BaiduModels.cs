namespace SnippetAdmin.Core.Oauth.Models
{
    public class BaiduAccessTokenResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public string session_key { get; set; }
        public string session_secret { get; set; }
    }

    public class BaiduUserInfo
    {
        public string openid { get; set; }
        public string uname { get; set; }
        public string portrait { get; set; }
    }
}