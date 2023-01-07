using SnippetAdmin.Data;

namespace SnippetAdmin.Jobs
{
	public class HelloJob : SnippetAdminJob
	{

		public HelloJob(SnippetAdminDbContext dbContext) : base(dbContext)
		{
		}

		public override async Task DoExecute()
		{
			await Task.Delay(5000);
			Console.WriteLine("hello " + DateTime.Now);
		}

	}
}
