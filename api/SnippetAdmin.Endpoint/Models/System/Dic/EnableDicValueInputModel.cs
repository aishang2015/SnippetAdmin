using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnippetAdmin.Endpoint.Models.System.Dic
{
    public record EnableDicValueInputModel
    {
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
    }
}
