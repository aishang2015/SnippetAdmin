using SnippetAdmin.Models.Common;

namespace SnippetAdmin.Models.Scheduler.JobRecord
{
    public class GetJobRecordsInputModel : PagedInputModel
    {
        public int? JobState { get; set; }

        public string JobName { get; set; }
    }
}
