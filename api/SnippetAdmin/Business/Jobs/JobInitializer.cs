using SnippetAdmin.Core.HostedService;
using SnippetAdmin.Core.Utils;
using SnippetAdmin.Data;

namespace SnippetAdmin.Business.Jobs
{
    public static class JobInitializer
    {
        public static Action<SnippetAdminDbContext> InitialJob { get => _initialJob; }

        private static Action<SnippetAdminDbContext> _initialJob =
           (dbcontext) =>
           {
               // 查找全局的ijob定义
               var jobTypes = ReflectionUtil.GetAssemblyTypes()
                    .Where(t => typeof(IJob).IsAssignableFrom(t));
               foreach (var jobType in jobTypes)
               {

                   // 查看ijob的特性定义
                   var attributeObj = jobType.GetCustomAttributes(typeof(SchedulerAttribute), false)
                        .FirstOrDefault();
                   if (attributeObj == null)
                   {
                       continue;
                   }

                   // 更新数据库内的数据
                   var schedulerAttribute = attributeObj as SchedulerAttribute;
                   var findJob = dbcontext.Jobs.FirstOrDefault(j => j.Name == jobType.FullName);
                   if (findJob == null)
                   {
                       dbcontext.Jobs.Add(new Data.Entity.Scheduler.Job
                       {
                           Cron = schedulerAttribute.Cron,
                           Describe = schedulerAttribute.Describe,
                           IsActive = schedulerAttribute.IsActive,
                           Name = jobType.FullName,
                           CreateTime = DateTime.Now,
                       });
                   }

                   // 即使代码attribute发生变化也以数据库储存的job信息为准
                   //else
                   //{
                   //    findJob.Cron = schedulerAttribute.Cron;
                   //    findJob.Describe = schedulerAttribute.Describe;
                   //    findJob.IsActive = schedulerAttribute.IsActive;
                   //    findJob.Name = jobType.FullName;
                   //    findJob.CreateTime = DateTime.Now;
                   //    dbcontext.Jobs.Update(findJob);
                   //}
                   dbcontext.SaveChanges();
               }

           };

    }
}
