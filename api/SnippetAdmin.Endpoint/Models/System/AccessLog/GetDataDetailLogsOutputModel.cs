using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnippetAdmin.Endpoint.Models.System.AccessLog
{
    public class GetDataDetailLogsOutputModel
    {
        public int Id { get; set; }

        public string? EntityName { get; set; }

        public string? Operation { get; set; }

        public DateTime OperateTime { get; set; }

        public List<DataDetailModel> DataDetailList { get; set; }
    }

    public class DataDetailModel
    {
        public string? PropertyName { get; set; } = null!;

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }
    }
}
