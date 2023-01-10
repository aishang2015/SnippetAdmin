namespace SnippetAdmin.Endpoint.Models.System.Setting
{
	public class GetSettingsOutputModel
	{
		public string Icon { get; set; }

		public string Name { get; set; }

		public string Describe { get; set; }

		public string Code { get; set; }

		public List<Setting> Settings { get; set; }
	}

	public class Setting
	{
		public string Icon { get; set; }

		public string Name { get; set; }

		public string Describe { get; set; }

		public string Code { get; set; }

		public string Value { get; set; }

		public int InputType { get; set; }

		public string Options { get; set; }

		public int Index { get; set; }

		public int? Min { get; set; }

		public int? Max { get; set; }

		public string Regex { get; set; }
	}
}
