namespace SnippetAdmin.Endpoint.Models.Scheduler.Job
{
	public record AddJobInputModel
	{
		public string? Type { get; set; }

		public string? Describe { get; set; }

		public string? Cron { get; set; }
	}
}
