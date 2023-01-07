namespace SnippetAdmin.Endpoint.Models.RBAC.Element
{
	public record CreateElementInputModel
	{
		public int? UpId { get; set; }

		public string Name { get; set; }

		public int? Type { get; set; }

		public string Identity { get; set; }

		public string AccessApi { get; set; }

		public int Sorting { get; set; }
	}
}