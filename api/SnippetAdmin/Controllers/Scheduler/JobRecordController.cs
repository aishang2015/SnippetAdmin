using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Core.Attributes;
using SnippetAdmin.Core.Extensions;
using SnippetAdmin.Data;
using SnippetAdmin.Endpoint.Apis.Scheduler;
using SnippetAdmin.Endpoint.Models;
using SnippetAdmin.Endpoint.Models.Common;
using SnippetAdmin.Endpoint.Models.Scheduler.JobRecord;

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

        [HttpPost]
        [CommonResultResponseType(typeof(PagedOutputModel<GetJobRecordsOutputModel>))]
        public async Task<CommonResult<PagedOutputModel<GetJobRecordsOutputModel>>> GetJobRecords(GetJobRecordsInputModel inputModel)
        {
            var q = from jr in _dbContext.JobRecords
                    join j in _dbContext.Jobs on jr.JobId equals j.Id
                    orderby jr.Id descending
                    select new GetJobRecordsOutputModel
                    {
                        Id = jr.Id,
                        Name = j.Name,
                        Describe = j.Describe,
                        BeginTime = jr.BeginTime,
                        Duration = jr.Duration == null ? "" : Sec2Min(jr.Duration.Value),
                        Infomation = jr.Infomation,
                        JobState = (int)jr.JobState,
                        TriggerMode = jr.TriggerMode.ToString()
                    };

            var resultQuery = q
                .AndIf(inputModel.JobState != null, jr => jr.JobState == inputModel.JobState)
                .AndIf(!string.IsNullOrEmpty(inputModel.JobName), jr => jr.Name == inputModel.JobName);

            var result = new PagedOutputModel<GetJobRecordsOutputModel>
            {
                Total = await resultQuery.CountAsync(),
                Data = await resultQuery.Skip(inputModel.SkipCount)
                    .Take(inputModel.TakeCount).ToListAsync()
            };

            return CommonResult.Success(result);
        }

        [HttpPost]
        [CommonResultResponseType]
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
            if (sec < 0)
                sec = 0;
            long miao = sec % 60;
            sec = sec - miao;
            sec /= 60;
            long fen = sec % 60;
            sec -= fen;
            long shi = sec / 60;
            return string.Format("{0:00}:{1:00}:{2:00}", shi, fen, miao);
        }
    }
}
