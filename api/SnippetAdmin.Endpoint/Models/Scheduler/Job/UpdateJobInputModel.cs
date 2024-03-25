namespace SnippetAdmin.Endpoint.Models.Scheduler.Job
{
	public record UpdateJobInputModel
	{
		public int Id { get; set; }

		public string Describe { get; set; }

		public string Cron { get; set; }
	}
}
