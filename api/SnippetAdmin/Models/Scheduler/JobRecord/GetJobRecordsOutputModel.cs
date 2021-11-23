namespace SnippetAdmin.Models.Scheduler.JobRecord
{
    public class GetJobRecordsOutputModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Describe { get; set; }

        public string TriggerMode { get; set; }

        public int JobState { get; set; }

        public string Duration { get; set; }

        public DateTime? BeginTime { get; set; }

        public string Infomation { get; set; }
    }
}
