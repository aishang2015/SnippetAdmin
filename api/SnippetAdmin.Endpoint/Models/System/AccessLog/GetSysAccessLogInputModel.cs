using SnippetAdmin.CommonModel;

namespace SnippetAdmin.Models.SysAccessLog
{
	public record GetSysAccessLogInputModel : PagedInputModel
    {
        public string Month { get; set; }
        public string ContainedModule { get; set; }
        public string EqualModule { get; set; }
        public string ContainedMethod { get; set; }
        public string EqualMethod { get; set; }
        public string ContainedPath { get; set; }
        public string EqualPath { get; set; }
        public Int32? UpperUserId { get; set; }
        public Int32? LowerUserId { get; set; }
        public Int32? EqualUserId { get; set; }
        public string ContainedRemoteIp { get; set; }
        public string EqualRemoteIp { get; set; }
        public DateTime? UpperAccessedTime { get; set; }
        public DateTime? LowerAccessedTime { get; set; }
        public DateTime? EqualAccessedTime { get; set; }
        public Int64? UpperElapsedTime { get; set; }
        public Int64? LowerElapsedTime { get; set; }
        public Int64? EqualElapsedTime { get; set; }
        public string ContainedRequestBody { get; set; }
        public string EqualRequestBody { get; set; }
        public Int32? UpperStatusCode { get; set; }
        public Int32? LowerStatusCode { get; set; }
        public Int32? EqualStatusCode { get; set; }
        public string ContainedResponseBody { get; set; }
        public string EqualResponseBody { get; set; }

    }
}