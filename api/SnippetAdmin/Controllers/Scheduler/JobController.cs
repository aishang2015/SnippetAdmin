using AutoMapper;
using Cronos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Attributes;
using SnippetAdmin.Core.Scheduler;
using SnippetAdmin.Data;
using SnippetAdmin.Endpoint.Apis.Scheduler;
using SnippetAdmin.Endpoint.Models;
using SnippetAdmin.Endpoint.Models.Common;
using SnippetAdmin.Endpoint.Models.Scheduler.Job;

namespace SnippetAdmin.Controllers.Scheduler
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class JobController : ControllerBase, IJobApi
    {
        private readonly SnippetAdminDbContext _dbContext;

        private readonly JobSchedulerService _jobSchedulerService;

        private readonly IMapper _mapper;

        public JobController(
            SnippetAdminDbContext dbContext,
            JobSchedulerService jobSchedulerService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _jobSchedulerService = jobSchedulerService;
            _mapper = mapper;
        }

        [HttpPost]
        [CommonResultResponseType(typeof(PagedOutputModel<GetJobsOutputModel>))]
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

            result.Data.AsParallel().ForAll(job =>
            {
                if (job.IsActive)
                {
                    job.NextTime = CronExpression.Parse(job.Cron, CronFormat.IncludeSeconds)
                        .GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local);
                }
            });

            return CommonResult.Success(result);
        }

        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> ActiveJob(ActiveJobInputModel inputModel)
        {
            var job = _dbContext.Jobs.Find(inputModel.Id);
            if (job == null)
            {
                return CommonResult.Fail(MessageConstant.JOB_ERROR_0001);
            }

            job.IsActive = inputModel.IsActive;
            _dbContext.Update(job);
            await _dbContext.SaveChangesAsync();

            if (inputModel.IsActive)
            {
                _jobSchedulerService.ActivateJob(job.Name);
            }
            else
            {
                _jobSchedulerService.PauseJob(job.Name);
            }

            return CommonResult.Success(inputModel.IsActive ?
                MessageConstant.JOB_INFO_0004 :
                MessageConstant.JOB_INFO_0005);
        }

        [HttpPost]
        [CommonResultResponseType(typeof(GetJobOutputModel))]
        public async Task<CommonResult<GetJobOutputModel>> GetJob(GetJobInputModel inputModel)
        {
            var job = await _dbContext.Jobs.FindAsync(inputModel.Id);
            return CommonResult.Success(new GetJobOutputModel
            {
                Id = job.Id,
                Cron = job.Cron,
                Describe = job.Describe,
                Name = job.Name,
            });
        }

        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> UpdateJob(UpdateJobInputModel inputModel)
        {
            var now = DateTime.UtcNow;
            var nextTime = CronExpression.Parse(inputModel.Cron, CronFormat.IncludeSeconds)
                .GetNextOccurrence(now, TimeZoneInfo.Local);

            // 如果取不到下次时间，则直接抛出异常
            if (nextTime == null)
            {
                return CommonResult.Fail(MessageConstant.JOB_ERROR_0002);
            }

            _dbContext.Jobs.Update(new Data.Entity.Scheduler.Job
            {
                Id = inputModel.Id,
                Cron = inputModel.Cron,
                Describe = inputModel.Describe,
                Name = inputModel.Name
            });
            await _dbContext.SaveChangesAsync();
            return CommonResult.Success(MessageConstant.JOB_INFO_0003);
        }

        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> DeleteJob(DeleteJobInputModel inputModel)
        {
            var job = _dbContext.Jobs.Find(inputModel.Id);
            var jobRecords = _dbContext.JobRecords.Where(r => r.JobId == job.Id).ToList();
            _dbContext.Jobs.Remove(job);
            _dbContext.JobRecords.RemoveRange(jobRecords);
            await _dbContext.SaveChangesAsync();

            _jobSchedulerService.CancelJob(job.Name);

            // 删除后把任务取消
            return CommonResult.Success(MessageConstant.JOB_INFO_0002);
        }

        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> AddJob(AddJobInputModel inputModel)
        {
            _dbContext.Add(new Data.Entity.Scheduler.Job
            {
                Name = inputModel.Name,
                Cron = inputModel.Cron,
                Describe = inputModel.Describe,
            });
            await _dbContext.SaveChangesAsync();
            return CommonResult.Success(MessageConstant.JOB_INFO_0001);
        }

        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> RunJob(RunJobInputModel inputModel)
        {
            var job = await _dbContext.Jobs.FindAsync(inputModel.Id);
            _jobSchedulerService.ActiveJobOnce(job);
            return CommonResult.Success(MessageConstant.JOB_INFO_0006);
        }

        [HttpPost]
        [CommonResultResponseType(typeof(List<string>))]
        public async Task<CommonResult<List<string>>> GetJobNames()
        {
            var result = await _dbContext.Jobs.Select(j => j.Name).ToListAsync();
            return CommonResult.Success(result);
        }
    }
}
