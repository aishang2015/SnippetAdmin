namespace SnippetAdmin.Endpoint.Models.Scheduler.Job
{
    public record GetJobsOutputModel
    {
        public int Id { get; set; }

        public string Type { get; set; }

        public string Describe { get; set; }

        public string Cron { get; set; }

        public bool IsActive { get; set; }

        public DateTime? NextTime { get; set; }

        public DateTime? LastTime { get; set; }

        public DateTime? CreateTime { get; set; }
    }
}
