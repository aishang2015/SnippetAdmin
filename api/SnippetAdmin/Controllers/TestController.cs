using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using SnippetAdmin.Business.Grains.Interfaces;
using SnippetAdmin.Models;

namespace SnippetAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
