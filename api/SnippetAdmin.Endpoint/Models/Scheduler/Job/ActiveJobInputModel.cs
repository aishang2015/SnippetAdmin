namespace SnippetAdmin.Endpoint.Models.Scheduler.Job
{
    public record ActiveJobInputModel
    {
        public int Id { get; set; }

        public bool IsActive { get; set; }
    }
}
