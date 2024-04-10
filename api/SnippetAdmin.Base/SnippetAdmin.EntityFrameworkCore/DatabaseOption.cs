namespace SnippetAdmin.EntityFrameworkCore
{
	public class DatabaseOption
	{
		public string Type { get; set; } = null!;

		public string Version { get; set; } = null!;

        public string Connection { get; set; } = null!;
    }
}