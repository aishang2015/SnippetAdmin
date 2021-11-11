namespace SnippetAdmin.Models.Account
{
    public record LoginOutputModel(string AccessToken, string UserName, DateTime Expire,
        string[] identifies, string refreshToken);
}