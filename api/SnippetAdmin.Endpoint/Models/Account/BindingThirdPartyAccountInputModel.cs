namespace SnippetAdmin.Endpoint.Models.Account
{
	public record BindingThirdPartyAccountInputModel(
		string UserName,
		string Password,
		string ThirdPartyType,              // 第三方登录类型
		string ThirdPartyInfoCacheKey);     // 第三方获取的信息缓存
}