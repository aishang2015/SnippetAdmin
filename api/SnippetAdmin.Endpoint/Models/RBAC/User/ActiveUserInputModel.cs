namespace SnippetAdmin.Endpoint.Models.RBAC.User
{
	public record ActiveUserInputModel
	{
		public int Id { get; set; }

		public bool IsActive { get; set; }
	}
}