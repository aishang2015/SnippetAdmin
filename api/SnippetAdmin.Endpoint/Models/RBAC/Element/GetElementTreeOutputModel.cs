namespace SnippetAdmin.Endpoint.Models.RBAC.Element
{
	public record GetElementTreeOutputModel
	{
		public string? Title { get; set; }

		public int Value { get; set; }

		public int Type { get; set; }

		public int Key { get; set; }

		public List<GetElementTreeOutputModel> Children { get; set; }
	}
}