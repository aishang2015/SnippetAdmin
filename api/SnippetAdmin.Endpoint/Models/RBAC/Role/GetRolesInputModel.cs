using SnippetAdmin.CommonModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnippetAdmin.Endpoint.Models.RBAC.Role
{
    public record GetRolesInputModel : PagedInputModel
    {
        public string? Name { get; set; }

        public string? Code { get; set; }
    }
}
