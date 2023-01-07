namespace SnippetAdmin.Endpoint.Models.RBAC.User
{
	public record AddOrgMemberInputModel
	{
		public int OrgId { get; set; }

		public int[] UserIds { get; set; }
	}
}