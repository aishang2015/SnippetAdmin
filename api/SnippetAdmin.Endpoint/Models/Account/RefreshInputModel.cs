﻿namespace SnippetAdmin.Endpoint.Models.Account
{
    public record RefreshInputModel
    {
        public string UserName { get; set; }

        public string JwtToken { get; set; }
    }
}
