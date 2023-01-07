using SnippetAdmin.CommonModel;

namespace SnippetAdmin.Endpoint.Models.Scheduler.JobRecord
{
	public record GetJobRecordsInputModel : PagedInputModel
	{
		public int? JobState { get; set; }

		public string JobName { get; set; }
	}
}
