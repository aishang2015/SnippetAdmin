namespace SnippetAdmin.Endpoint.Models.Scheduler.JobRecord
{
	public record RemoveJobRecordsInputModel
	{
		public int[] RecordIds { get; set; }
	}
}
