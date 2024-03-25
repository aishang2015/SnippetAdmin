using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Core.Attributes;
using SnippetAdmin.Core.Extensions;
using SnippetAdmin.Data;
using SnippetAdmin.Endpoint.Apis.Scheduler;
using SnippetAdmin.Endpoint.Models.Scheduler.JobRecord;
using System.ComponentModel;

namespace SnippetAdmin.Controllers.Scheduler
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class JobRecordController : ControllerBase, IJobRecordApi
    {

        private readonly SnippetAdminDbContext _dbContext;

        public JobRecordController(SnippetAdminDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 查询任务记录
        /// </summary>
        [HttpPost]
        [CommonResultResponseType<PagedOutputModel<GetJobRecordsOutputModel>>]
        [Description("查询任务记录")]
        public async Task<CommonResult<PagedOutputModel<GetJobRecordsOutputModel>>> GetJobRecords(GetJobRecordsInputModel inputModel)
        {
            var q = from jr in _dbContext.JobRecords
                    join j in _dbContext.Jobs on jr.JobName equals j.Key
                    orderby jr.Id descending
                    select new GetJobRecordsOutputModel
                    {
                        Id = jr.Id,
                        JobType = j.Type,
                        Describe = j.Describe,
                        BeginTime = jr.BeginTime,
                        EndTime = jr.EndTime,
                        Duration = jr.Duration == null ? "" : Sec2Min(jr.Duration.Value),
                        Infomation = jr.Infomation,
                        JobState = (int)jr.JobState
                    };

            var resultQuery = q
                .AndIf(inputModel.JobState != null, jr => jr.JobState == inputModel.JobState)
                .AndIf(!string.IsNullOrEmpty(inputModel.jobType), jr => jr.JobType == inputModel.jobType);

            var result = new PagedOutputModel<GetJobRecordsOutputModel>
            {
                Total = await resultQuery.CountAsync(),
                Data = await resultQuery.Skip(inputModel.SkipCount)
                    .Take(inputModel.TakeCount).ToListAsync()
            };

            return CommonResult.Success(result);
        }

        /// <summary>
        /// 删除任务记录
        /// </summary>
        [HttpPost]
        [CommonResultResponseType]
        [Description("删除任务记录")]
        public async Task<CommonResult> RemoveJobRecords(RemoveJobRecordsInputModel inputModel)
        {
            var jobRecords = _dbContext.JobRecords.Where(jr => inputModel.RecordIds.Contains(jr.Id)).ToList();
            _dbContext.JobRecords.RemoveRange(jobRecords);
            await _dbContext.SaveChangesAsync();
            return CommonResult.Success();
        }

        /// <summary>
        /// 秒转时间
        /// </summary>
        private static string Sec2Min(long sec)
        {
            long millisecond = sec % 1000;
            sec = sec / 1000;
            if (sec < 0)
                sec = 0;
            long miao = sec % 60;
            sec = sec - miao;
            sec /= 60;
            long fen = sec % 60;
            sec -= fen;
            long shi = sec / 60;
            return string.Format("{0:00}:{1:00}:{2:00}.{3:000}", shi, fen, miao, millisecond);
        }
    }
}
