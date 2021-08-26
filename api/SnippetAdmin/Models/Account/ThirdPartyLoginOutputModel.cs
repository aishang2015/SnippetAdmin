namespace SnippetAdmin.Models.Account
{
    public record ThirdPartyLoginOutputModel(
        string AccessToken,
        string UserName,
        string ThirdPartyType,              // 第三方登录类型
        string ThirdPartyUserName,          // 第三方获取的用户名
        string ThirdPartyInfoCacheKey);     // 第三方获取的信息缓存
}