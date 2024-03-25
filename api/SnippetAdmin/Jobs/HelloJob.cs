using SnippetAdmin.Constants;
using SnippetAdmin.Data;

namespace SnippetAdmin.Jobs
{
    public class HelloJob : SnippetAdminJob
    {
        public HelloJob(SnippetAdminDbContext dbContext) : base(dbContext) { }

        public override async Task<CommonResult> DoExecute()
        {
            await Task.Delay(new Random().Next(1000, 10000));
            Console.WriteLine("hello " + DateTime.Now);

            return CommonResult.Success(string.Empty, "Hello任务处理成功！");
        }

    }
}
