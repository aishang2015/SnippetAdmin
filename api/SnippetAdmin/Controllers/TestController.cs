using Microsoft.AspNetCore.Mvc;
using Orleans;
using SnippetAdmin.Grains;
using SnippetAdmin.Models;

namespace SnippetAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class TestController : ControllerBase
    {
        [HttpPost]
        public async Task<CommonResult> TestGrain([FromServices] IClusterClient client)
        {
            var testGrain = client.GetGrain<ITest>(1);
            await testGrain.Do();
            return this.SuccessCommonResult("处理完毕");
        }
    }
}
