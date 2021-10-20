using System;

namespace SnippetAdmin.Models.Account
{
    public record LoginOutputModel(string AccessToken, string UserName, DateTime Expire, string[] identifies);
}