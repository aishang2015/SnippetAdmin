using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnippetAdmin.Core.FileStore;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Entity;
using SnippetAdmin.Grains;
using SnippetAdmin.Jobs;
using SnippetAdmin.Quartz;

namespace SnippetAdmin.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[ApiExplorerSettings(GroupName = "v1")]
	[AllowAnonymous]
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

		[HttpGet]
		public async Task<IActionResult> TestQuartz1([FromServices] IQuartzService quartzService)
		{
			await quartzService.Add(new Quartz.Models.JobDetail
			{
				JobType = typeof(HelloJob),
				CronExpression = "0/10 * * * * ?",
				JobDescribe = "描述",
				JobKey = "这个任务"
			});
			return Ok();
		}

		[HttpGet]
		public async Task<IActionResult> TestSharding1([FromServices] SnippetAdminDbContext dbContext)
		{
			var guid = Guid.NewGuid();
			//await dbContext.CreateTable(guid.ToString(), typeof(TemporaryEntity));
			return Ok();
		}
		[HttpGet]

		public async Task<IActionResult> TestSharding2([FromServices] SnippetAdminDbContext dbContext)
		{
			var s = dbContext.SysShardings.FirstOrDefault();
			if (s != null)
			{
				var count = dbContext.Set<TemporaryEntity>(s.TableName).Count();
				return Ok(count);
			}
			return Ok();
		}
	}
}
