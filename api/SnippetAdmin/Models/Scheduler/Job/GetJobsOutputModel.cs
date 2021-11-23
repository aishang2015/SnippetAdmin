namespace SnippetAdmin.Models.Scheduler.Job
{
    public class GetJobsOutputModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Describe { get; set; }

        public string Cron { get; set; }

        public bool IsActive { get; set; }

        public DateTime? NextTime { get; set; }

        public DateTime? LastTime { get; set; }

        public DateTime? CreateTime { get; set; }
    }
}
