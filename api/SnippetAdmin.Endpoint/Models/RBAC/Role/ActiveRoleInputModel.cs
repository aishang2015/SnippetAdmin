namespace SnippetAdmin.Endpoint.Models.RBAC.Role
{
	public record ActiveRoleInputModel
	{
		public int Id { get; set; }

		public bool IsActive { get; set; }
	}
}