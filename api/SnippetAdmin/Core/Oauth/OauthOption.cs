namespace SnippetAdmin.Core.Oauth
{
    public class OauthOption
    {
        public AppOption Github { get; set; }
        public AppOption Baidu { get; set; }
    }

    public class AppOption
    {
        public string AppId { get; set; }

        public string AppSecret { get; set; }

        public string RedirectUri { get; set; }
    }
}