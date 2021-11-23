namespace SnippetAdmin.Models.Scheduler.Job
{
    public class AddJobInputModel
    {

        public string Name { get; set; }

        public string Describe { get; set; }

        public string Cron { get; set; }
    }
}
