using SnippetAdmin.CommonModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnippetAdmin.Endpoint.Models.RBAC.Position
{
    public record GetPositionsInputModel : PagedInputModel
    {
        public string? Name { get; set; }
    }
}
