namespace SnippetAdmin.Endpoint.Models.Dynamic
{
	public record GetColumnsOutputModel
	{
		public string PropertyName { get; set; }

		public string PropertyDescribe { get; set; }

		public string PropertyType { get; set; }
	}
}
