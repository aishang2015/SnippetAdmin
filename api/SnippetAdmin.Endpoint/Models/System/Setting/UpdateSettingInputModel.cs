using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnippetAdmin.Endpoint.Models.System.Setting
{
    public record UpdateSettingInputModel
    {
        public List<Setting> Settings { get; set; }
    }
}
