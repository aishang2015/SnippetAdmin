namespace SnippetAdmin.Endpoint.Models.Dynamic
{
	public record GetDynamicInfoOutputModel
	{
		public string Group { get; set; }

		public List<DynamicInfoGroup> DynamicInfoGroups { get; set; }
	}

	public record DynamicInfoGroup
	{
		public string Name { get; set; }

		public string EntityName { get; set; }
	}
}
