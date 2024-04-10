namespace SnippetAdmin.Endpoint.Models.Account
{
	public record RefreshInputModel
	{
		public string UserName { get; set; } = null!;	

		public string JwtToken { get; set; } = null!;
	}
}
