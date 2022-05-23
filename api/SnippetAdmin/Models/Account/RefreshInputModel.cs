namespace SnippetAdmin.Models.Account
{
    public record RefreshInputModel
    {
        public string UserName { get; set; }

        public string JwtToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
