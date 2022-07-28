using Microsoft.AspNetCore.Mvc;
using Orleans;
using SnippetAdmin.Core.FileStore;
using SnippetAdmin.Grains;

namespace SnippetAdmin.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class TestController : ControllerBase
    {
        [HttpPost]
        public async Task<CommonResult> TestGrain([FromServices] IClusterClient client)
        {
            var testGrain = client.GetGrain<ITest>(1);
            await testGrain.Do();
            return CommonResult.Success("处理完毕");
        }

        [HttpPost]
        public async Task<CommonResult> TestUpload(IFormFile file, [FromServices] IFileStoreService fileStoreService)
        {
            await fileStoreService.SaveFromStreamAsync(file.OpenReadStream(), file.FileName);
            return CommonResult.Success("");
        }

        [HttpGet]
        public async Task<IActionResult> TestDownload([FromQuery] string fileName, [FromServices] IFileStoreService fileStoreService)
        {
            var file = await fileStoreService.GetFileStreamAsync(fileName);
            return File(file, "application/octet-stream", fileName.Split('/').Last());
        }
    }
}
