using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnippetAdmin.Endpoint.Models.System.AccessLog
{
    public record GetSysAccessLogOutputModel
    {
        public int Id { get; set; }
        public string TraceIdentifier { get; set; }
        public string Module { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public int UserId { get; set; }
        public string RealName { get; set; }
        public string RemoteIp { get; set; }
        public DateTime AccessedTime { get; set; }
        public long ElapsedTime { get; set; }
        public string RequestBody { get; set; }
        public int StatusCode { get; set; }
        public string ResponseBody { get; set; }
    }
}
