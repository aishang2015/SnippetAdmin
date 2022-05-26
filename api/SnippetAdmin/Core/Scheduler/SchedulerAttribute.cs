namespace SnippetAdmin.Core.Scheduler
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SchedulerAttribute : System.Attribute
    {
        public string Cron { get; }
        public string Describe { get; }
        public bool IsActive { get; }
        public bool IsIgnore { get; }

        /// <summary>
        /// 调度器特性
        /// </summary>
        /// <param name="cron">cron表达式</param>
        /// <param name="describe">任务描述</param>
        /// <param name="isActive">任务初始状态，是否执行状态</param>
        /// <param name="isIgnore">是否会被扫描忽略，如果是true则不会被扫描后加入数据库</param>
        public SchedulerAttribute(string cron, string describe, bool isActive = false,
            bool isIgnore = false)
        {
            Cron = cron;
            Describe = describe;
            IsActive = isActive;
            IsIgnore = isIgnore;
        }
    }
}
