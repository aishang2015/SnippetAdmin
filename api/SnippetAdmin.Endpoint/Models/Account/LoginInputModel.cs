namespace SnippetAdmin.Endpoint.Models.Account
{
    public record LoginInputModel(string UserName, string Password, string CaptchaCode, string CaptchaKey);
}