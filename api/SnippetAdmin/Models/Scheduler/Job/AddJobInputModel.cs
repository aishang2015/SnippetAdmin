namespace SnippetAdmin.Models.Scheduler.Job
{
    public record AddJobInputModel
    {

        public string Name { get; set; }

        public string Describe { get; set; }

        public string Cron { get; set; }
    }
}
