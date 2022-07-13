using SnippetAdmin.Endpoint.Models;
using SnippetAdmin.Endpoint.Models.Common;
using SnippetAdmin.Endpoint.Models.Scheduler.JobRecord;

namespace SnippetAdmin.Endpoint.Apis.Scheduler
{
    public interface IJobRecordApi
    {
        public Task<CommonResult<PagedOutputModel<GetJobRecordsOutputModel>>> GetJobRecords(
            GetJobRecordsInputModel inputModel);

        public Task<CommonResult> RemoveJobRecords(RemoveJobRecordsInputModel inputModel);
    }
}
