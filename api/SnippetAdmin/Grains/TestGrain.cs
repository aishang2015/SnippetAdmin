using Orleans;

namespace SnippetAdmin.Grains
{
    public interface ITest : IGrainWithIntegerKey
    {
        Task Do();
    }

    public class TestGrain : Grain, ITest
    {
        private readonly ILogger<TestGrain> _logger;

        public TestGrain(ILogger<TestGrain> logger)
        {
            _logger = logger;
        }

        public async Task Do()
        {
            _logger.LogInformation("开始执行业务逻辑");
            await Task.Delay(10000);
            _logger.LogInformation("业务逻辑执行完毕");
        }
    }
}
