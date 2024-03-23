using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnippetAdmin.Endpoint.Models.System.AccessLog
{
    public record GetDataDetailLogsInputModel
    {
        public string TraceIdentifier { get; set; }
    }
}
