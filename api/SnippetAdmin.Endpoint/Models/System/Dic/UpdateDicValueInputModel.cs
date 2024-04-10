namespace SnippetAdmin.Endpoint.Models.System.Dic
{
	public class UpdateDicValueInputModel
	{
		public int Id { get; set; }

		public int TypeId { get; set; }

		public string? Name { get; set; }

		public string? Code { get; set; }

		public int Sorting { get; set; }
	}
}
