namespace SnippetAdmin.Endpoint.Models.RBAC.User
{
	public record SetUserPasswordInputModel
	{
		public int Id { get; set; }

		public string? Password { get; set; }

		public string? ConfirmPassword { get; set; }
	}
}