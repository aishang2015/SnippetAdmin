namespace SnippetAdmin.Endpoint.Models.RBAC.User
{
	public record RemoveOrgMemberInputModel
	{
		public int OrgId { get; set; }

		public int UserId { get; set; }
	}
}