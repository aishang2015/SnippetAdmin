namespace SnippetAdmin.Endpoint.Models.RBAC.Position
{
	public record GetPositionOutputModel
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string Code { get; set; }

		public int Sorting { get; set; }
	}
}
