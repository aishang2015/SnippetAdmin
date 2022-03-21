using AutoMapper;
using Cronos;
using Microsoft.AspNetCore.Mvc;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Attribute;
using SnippetAdmin.Core.Scheduler;
using SnippetAdmin.Data;
using SnippetAdmin.Models;
using SnippetAdmin.Models.Common;
using SnippetAdmin.Models.Scheduler.Job;

namespace SnippetAdmin.Controllers.Scheduler
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class JobController : ControllerBase
    {
        private readonly SnippetAdminDbContext _dbContext;

        public JobController(SnippetAdminDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        [CommonResultResponseType(typeof(PagedOutputModel<GetJobsOutputModel>))]
        public CommonResult GetJobs(GetJobsInputModel inputModel,
            [FromServices] IMapper mapper)
        {
            var query = _dbContext.Jobs;
            var data = query
                        .OrderByDescending(j => j.CreateTime)
                        .Skip(inputModel.SkipCount)
                        .Take(inputModel.TakeCount)
                        .ToList();

            var result = new PagedOutputModel<GetJobsOutputModel>
            {
                Total = query.Count(),
                Data = mapper.Map<List<GetJobsOutputModel>>(data)
            };

            result.Data.AsParallel().ForAll(job =>
            {
                if (job.IsActive)
                {
                    job.NextTime = CronExpression.Parse(job.Cron, CronFormat.IncludeSeconds)
                        .GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local);
                }
            });

            return this.SuccessCommonResult(result);
        }

        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> ActiveJob(ActiveJobInputModel inputModel)
        {
            var job = _dbContext.Jobs.Find(inputModel.Id);
            if (job == null)
            {
                return this.FailCommonResult(MessageConstant.JOB_ERROR_0001);
            }

            job.IsActive = inputModel.IsActive;
            _dbContext.Update(job);
            await _dbContext.SaveChangesAsync();
            return this.SuccessCommonResult(inputModel.IsActive ?
                MessageConstant.JOB_INFO_0004 :
                MessageConstant.JOB_INFO_0005);
        }

        [HttpPost]
        [CommonResultResponseType(typeof(GetJobOutputModel))]
        public async Task<CommonResult> GetJob(GetJobInputModel inputModel)
        {
            var job = await _dbContext.Jobs.FindAsync(inputModel.Id);
            return this.SuccessCommonResult(new GetJobOutputModel
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
            _dbContext.Jobs.Update(new Data.Entity.Scheduler.Job
            {
                Id = inputModel.Id,
                Cron = inputModel.Cron,
                Describe = inputModel.Describe,
                Name = inputModel.Name
            });
            await _dbContext.SaveChangesAsync();
            return this.SuccessCommonResult(MessageConstant.JOB_INFO_0003);
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
            return this.SuccessCommonResult(MessageConstant.JOB_INFO_0002);
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
            return this.SuccessCommonResult(MessageConstant.JOB_INFO_0001);
        }

        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> RunJob(RunJobInputModel inputModel)
        {
            var job = await _dbContext.Jobs.FindAsync(inputModel.Id);
            JobSchedulerService.ActiveJobOnce(job);
            return this.SuccessCommonResult(MessageConstant.JOB_INFO_0006);
        }

        [HttpPost]
        [CommonResultResponseType(typeof(List<string>))]
        public CommonResult GetJobNames()
        {
            return this.SuccessCommonResult(_dbContext.Jobs.Select(j => j.Name));
        }
    }
}
