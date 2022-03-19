namespace SnippetAdmin.Core.Scheduler
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SchedulerAttribute : System.Attribute
    {
        public string Cron { get; }
        public string Describe { get; }
        public bool IsActive { get; }

        public SchedulerAttribute(string cron, string describe, bool isActive = false)
        {
            Cron = cron;
            Describe = describe;
            IsActive = isActive;
        }
    }
}
