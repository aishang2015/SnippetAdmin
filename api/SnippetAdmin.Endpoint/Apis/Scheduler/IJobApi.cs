using SnippetAdmin.CommonModel;
using SnippetAdmin.Endpoint.Models.Scheduler.Job;

namespace SnippetAdmin.Endpoint.Apis.Scheduler
{
    public interface IJobApi
    {
        public Task<CommonResult<PagedOutputModel<GetJobsOutputModel>>> GetJobs(GetJobsInputModel inputModel);

        public Task<CommonResult> ActiveJob(ActiveJobInputModel inputModel);

        public Task<CommonResult<GetJobOutputModel>> GetJob(GetJobInputModel inputModel);

        public Task<CommonResult> UpdateJob(UpdateJobInputModel inputModel);

        public Task<CommonResult> DeleteJob(DeleteJobInputModel inputModel);

        public Task<CommonResult> AddJob(AddJobInputModel inputModel);

        public Task<CommonResult> RunJob(RunJobInputModel inputModel);

        public Task<CommonResult<List<string>>> GetJobNames();
    }
}
