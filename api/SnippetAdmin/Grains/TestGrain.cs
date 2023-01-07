using SnippetAdmin.Data;

namespace SnippetAdmin.Grains
{
	public interface ITest : IGrainWithIntegerKey
	{
		Task Do();
	}

	public class TestGrain : Grain, ITest
	{
		private readonly ILogger<TestGrain> _logger;

		private readonly SnippetAdminDbContext _dbContext;

		public TestGrain(ILogger<TestGrain> logger,
			SnippetAdminDbContext dbContext)
		{
			_logger = logger;
			_dbContext = dbContext;
		}

		public async Task Do()
		{
			_logger.LogInformation("开始执行业务逻辑");
			await Task.Delay(10000);
			_logger.LogInformation("业务逻辑执行完毕");
		}
	}
}
