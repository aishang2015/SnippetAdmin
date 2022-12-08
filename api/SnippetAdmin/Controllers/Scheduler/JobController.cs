using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quartz;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Attributes;
using SnippetAdmin.Core.Helpers;
using SnippetAdmin.Data;
using SnippetAdmin.Endpoint.Apis.Scheduler;
using SnippetAdmin.Endpoint.Models.Scheduler.Job;
using SnippetAdmin.Jobs;
using SnippetAdmin.Quartz;
using System.ComponentModel;

namespace SnippetAdmin.Controllers.Scheduler
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[ApiExplorerSettings(GroupName = "v1")]
	public class JobController : ControllerBase, IJobApi
	{
		private readonly SnippetAdminDbContext _dbContext;

		private readonly IQuartzService _quartzService;

		private readonly IMapper _mapper;

		public JobController(
			SnippetAdminDbContext dbContext,
			IQuartzService quartzService,
			IMapper mapper)
		{
			_dbContext = dbContext;
			_quartzService = quartzService;
			_mapper = mapper;
		}

		/// <summary>
		/// 查询任务类型列表
		/// </summary>
		[HttpPost]
		[CommonResultResponseType<List<string>>]
		[Description("查询任务类型列表")]
		public CommonResult<List<string>> GetJobTypeList()
		{
			var typeNameList = ReflectionHelper
				.GetSubClass<SnippetAdminJob>()
				.Where(c => !c.IsAbstract)
				.Select(t => t.FullName)
				.ToList();
			return CommonResult.Success(typeNameList);
		}

		/// <summary>
		/// 查询任务列表
		/// </summary>
		[HttpPost]
		[CommonResultResponseType<PagedOutputModel<GetJobsOutputModel>>]
		[Description("查询任务列表")]
		public async Task<CommonResult<PagedOutputModel<GetJobsOutputModel>>> GetJobs(GetJobsInputModel inputModel)
		{
			var query = _dbContext.Jobs;
			var data = await query
						.OrderByDescending(j => j.CreateTime)
						.Skip(inputModel.SkipCount)
						.Take(inputModel.TakeCount)
						.ToListAsync();

			var result = new PagedOutputModel<GetJobsOutputModel>
			{
				Total = await query.CountAsync(),
				Data = _mapper.Map<List<GetJobsOutputModel>>(data)
			};

			//result.Data.AsParallel().ForAll(job =>
			//{
			//    if (job.IsActive)
			//    {
			//        job.NextTime = new CronExpression(job.Cron)
			//            .GetNextValidTimeAfter(DateTime.UtcNow).Value.DateTime;
			//    }
			//});

			return CommonResult.Success(result);
		}

		/// <summary>
		/// 激活任务
		/// </summary>
		[HttpPost]
		[CommonResultResponseType]
		[Description("激活任务")]
		public async Task<CommonResult> ActiveJob(ActiveJobInputModel inputModel)
		{
			var job = _dbContext.Jobs.Find(inputModel.Id);
			if (job == null)
			{
				return CommonResult.Fail(MessageConstant.JOB_ERROR_0001);
			}

			job.IsActive = inputModel.IsActive;
			_dbContext.Update(job);

			if (inputModel.IsActive)
			{
				await _quartzService.Resume(job.Name);
			}
			else
			{
				await _quartzService.Pause(job.Name);
			}
			await _dbContext.SaveChangesAsync();

			return CommonResult.Success(inputModel.IsActive ?
				MessageConstant.JOB_INFO_0004 :
				MessageConstant.JOB_INFO_0005);
		}

		/// <summary>
		/// 取得任务详情
		/// </summary>
		[HttpPost]
		[CommonResultResponseType<GetJobOutputModel>]
		[Description("取得任务详情")]
		public async Task<CommonResult<GetJobOutputModel>> GetJob(GetJobInputModel inputModel)
		{
			var job = await _dbContext.Jobs.FindAsync(inputModel.Id);
			return CommonResult.Success(new GetJobOutputModel
			{
				Id = job.Id,
				Type = job.Type,
				Cron = job.Cron,
				Describe = job.Describe,
				Name = job.Name,
			});
		}

		/// <summary>
		/// 更新任务
		/// </summary>
		[HttpPost]
		[CommonResultResponseType]
		[Description("更新任务")]
		public async Task<CommonResult> UpdateJob(UpdateJobInputModel inputModel)
		{
			if (_dbContext.Jobs.Any(j => j.Name == inputModel.Name && j.Id != inputModel.Id))
			{
				return CommonResult.Fail(MessageConstant.JOB_ERROR_0003);
			}

			var now = DateTime.UtcNow;
			var isValid = CronExpression.IsValidExpression(inputModel.Cron);

			// 如果取不到下次时间，则直接抛出异常
			if (!isValid)
			{
				return CommonResult.Fail(MessageConstant.JOB_ERROR_0002);
			}

			await _quartzService.Remove(inputModel.Name);
			await _quartzService.Add(new Quartz.Models.JobDetail
			{
				JobType = Type.GetType(inputModel.Type),
				JobName = inputModel.Name,
				JobDescribe = inputModel.Describe,
				CronExpression = inputModel.Cron
			});
			var job = _dbContext.Jobs.Find(inputModel.Id);
			job.Type = inputModel.Type;
			job.Cron = inputModel.Cron;
			job.Describe = inputModel.Describe;
			job.Name = inputModel.Name;
			_dbContext.Update(job);
			await _dbContext.SaveChangesAsync();
			return CommonResult.Success(MessageConstant.JOB_INFO_0003);
		}

		/// <summary>
		/// 删除任务
		/// </summary>
		[HttpPost]
		[CommonResultResponseType]
		[Description("删除任务")]
		public async Task<CommonResult> DeleteJob(DeleteJobInputModel inputModel)
		{
			var job = _dbContext.Jobs.Find(inputModel.Id);
			var jobRecords = _dbContext.JobRecords.Where(r => r.JobName == job.Name).ToList();
			_dbContext.Jobs.Remove(job);
			_dbContext.JobRecords.RemoveRange(jobRecords);
			await _quartzService.Remove(job.Name);
			await _dbContext.SaveChangesAsync();

			// 删除后把任务取消
			return CommonResult.Success(MessageConstant.JOB_INFO_0002);
		}

		/// <summary>
		/// 添加任务
		/// </summary>
		[HttpPost]
		[CommonResultResponseType]
		[Description("添加任务")]
		public async Task<CommonResult> AddJob(AddJobInputModel inputModel)
		{
			if (_dbContext.Jobs.Any(j => j.Name == inputModel.Name))
			{
				return CommonResult.Fail(MessageConstant.JOB_ERROR_0003);
			}

			await _quartzService.Add(new Quartz.Models.JobDetail
			{
				JobType = Type.GetType(inputModel.Type),
				JobName = inputModel.Name,
				JobDescribe = inputModel.Describe,
				CronExpression = inputModel.Cron
			});
			_dbContext.Add(new Data.Entity.Scheduler.Job
			{
				Type = inputModel.Type,
				Name = inputModel.Name,
				Cron = inputModel.Cron,
				Describe = inputModel.Describe,
				IsActive = true,
				CreateTime = DateTime.Now
			});
			await _dbContext.SaveChangesAsync();
			return CommonResult.Success(MessageConstant.JOB_INFO_0001);
		}

		/// <summary>
		/// 运行任务
		/// </summary>
		[HttpPost]
		[CommonResultResponseType]
		[Description("运行任务")]
		public async Task<CommonResult> RunJob(RunJobInputModel inputModel)
		{
			var job = await _dbContext.Jobs.FindAsync(inputModel.Id);
			await _quartzService.Trigger(job.Name);
			return CommonResult.Success(MessageConstant.JOB_INFO_0006);
		}

		/// <summary>
		/// 获取任务名称
		/// </summary>
		[HttpPost]
		[CommonResultResponseType<List<string>>]
		[Description("获取任务名称")]
		public async Task<CommonResult<List<string>>> GetJobNames()
		{
			var result = await _dbContext.Jobs.Select(j => j.Name).ToListAsync();
			return CommonResult.Success(result);
		}
	}
}
